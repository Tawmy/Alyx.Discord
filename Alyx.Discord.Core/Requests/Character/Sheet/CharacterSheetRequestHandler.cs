using System.Collections.Concurrent;
using System.Diagnostics;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Records;
using Alyx.Discord.Core.Services;
using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Enums;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

internal class CharacterSheetRequestHandler(
    AlyxConfiguration config,
    INetStoneApiCharacter apiCharacter,
    INetStoneApiFreeCompany apiFreeCompany,
    CharacterSheetService characterSheetService)
    : IRequestHandler<CharacterSheetRequest, CharacterSheetResponse>
{
    public async Task<CharacterSheetResponse> Handle(CharacterSheetRequest request,
        CancellationToken cancellationToken)
    {
        var id = request.LodestoneId;

        ConcurrentDictionary<Task, Stopwatch> tasks = [];

        var taskCharacter = apiCharacter.GetAsync(id, request.ForceRefresh ? 0 : config.NetStone.MaxAgeCharacter,
            FallbackTypeV4.Any, cancellationToken);
        var stopwatchCharacter = new Stopwatch();
        stopwatchCharacter.Start();
        tasks.GetOrAdd(taskCharacter, stopwatchCharacter);

        var taskClassJobs = apiCharacter.GetClassJobsAsync(id,
            request.ForceRefresh ? 0 : config.NetStone.MaxAgeClassJobs, FallbackTypeV4.Any, cancellationToken);
        var stopwatchClassJobs = new Stopwatch();
        stopwatchClassJobs.Start();
        tasks.GetOrAdd(taskClassJobs, stopwatchClassJobs);

        var taskMinions = apiCharacter.GetMinionsAsync(id, request.ForceRefresh ? 0 : config.NetStone.MaxAgeMinions,
            FallbackTypeV4.Any, cancellationToken);
        var stopwatchMinions = new Stopwatch();
        stopwatchMinions.Start();
        tasks.GetOrAdd(taskMinions, stopwatchMinions);

        var taskMounts = apiCharacter.GetMountsAsync(id, request.ForceRefresh ? 0 : config.NetStone.MaxAgeMounts,
            FallbackTypeV4.Any, cancellationToken);
        var stopwatchMounts = new Stopwatch();
        stopwatchMounts.Start();
        tasks.GetOrAdd(taskMounts, stopwatchMounts);

        await foreach (var task in Task.WhenEach(tasks.Select(x => x.Key))
                           .WithCancellation(cancellationToken))
        {
            tasks.TryGetValue(task, out var stopwatch);
            stopwatch?.Stop();
        }

        try
        {
            await Task.WhenAll(tasks.Select(x => x.Key));
        }
        catch (NotFoundException)
        {
            // do nothing
        }

        FreeCompanyDto? freeCompany = null;
        var freeCompanyStopwatch = new Stopwatch();
        if (taskCharacter.Result.FreeCompany is not null)
        {
            try
            {
                // character page does not return FC tag, so we need to queue full free company
                // returning tag of cached fc is not a good idea either because it might be outdated
                freeCompanyStopwatch.Start();
                freeCompany = await apiFreeCompany.GetAsync(taskCharacter.Result.FreeCompany.Id,
                    request.ForceRefresh ? 0 : config.NetStone.MaxAgeFreeCompany, FallbackTypeV4.Any,
                    cancellationToken);
            }
            catch (NotFoundException)
            {
                // do nothing
            }
        }

        freeCompanyStopwatch.Stop();

        var image = await characterSheetService.CreateCharacterSheetAsync(taskCharacter.Result,
            !taskClassJobs.IsFaulted ? taskClassJobs.Result : null,
            !taskMinions.IsFaulted ? taskMinions.Result : null,
            !taskMounts.IsFaulted ? taskMounts.Result : null,
            freeCompany);

        var now = DateTime.UtcNow;

        var metadata = new List<SheetMetadata>
        {
            new("Character", stopwatchCharacter.Elapsed, taskCharacter.Result.LastUpdated ?? now,
                taskCharacter.Result.FallbackUsed,
                CreateFallbackMessage(taskCharacter.Result.FallbackReason))
        };

        if (!taskClassJobs.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Jobs", stopwatchClassJobs.Elapsed,
                taskClassJobs.Result.LastUpdated ?? now, taskClassJobs.Result.FallbackUsed,
                CreateFallbackMessage(taskClassJobs.Result.FallbackReason)));
        }

        if (!taskMinions.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Minions", stopwatchMinions.Elapsed,
                taskMinions.Result.LastUpdated ?? now, taskMinions.Result.FallbackUsed,
                CreateFallbackMessage(taskMinions.Result.FallbackReason)));
        }

        if (!taskMounts.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Mounts", stopwatchMounts.Elapsed, taskMounts.Result.LastUpdated ?? now,
                taskMounts.Result.FallbackUsed, CreateFallbackMessage(taskMounts.Result.FallbackReason)));
        }

        if (freeCompany is not null)
        {
            metadata.Add(new SheetMetadata("Free Company", freeCompanyStopwatch.Elapsed, freeCompany.LastUpdated ?? now,
                freeCompany.FallbackUsed, CreateFallbackMessage(freeCompany.FallbackReason)));
        }

        return new CharacterSheetResponse(image, metadata, !taskMinions.IsFaulted, !taskMounts.IsFaulted,
            taskCharacter.Result, taskClassJobs.Result, freeCompany);
    }

    private static string? CreateFallbackMessage(string? fallbackReason)
    {
        if (fallbackReason is null)
        {
            return null;
        }

        return fallbackReason.Equals(nameof(ParsingFailedException), StringComparison.OrdinalIgnoreCase)
            ? "Profile set to private or Lodestone under maintenance"
            : fallbackReason;
    }
}