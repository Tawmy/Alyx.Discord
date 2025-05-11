using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Services;
using Alyx.Discord.Core.Structs;
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

        var taskCharacter = apiCharacter.GetAsync(id, request.ForceRefresh ? 0 : config.NetStone.MaxAgeCharacter,
            FallbackType.Any, cancellationToken);
        var taskClassJobs = apiCharacter.GetClassJobsAsync(id,
            request.ForceRefresh ? 0 : config.NetStone.MaxAgeClassJobs, FallbackType.Any, cancellationToken);
        var taskMinions = apiCharacter.GetMinionsAsync(id, request.ForceRefresh ? 0 : config.NetStone.MaxAgeMinions,
            FallbackType.Any, cancellationToken);
        var taskMounts = apiCharacter.GetMountsAsync(id, request.ForceRefresh ? 0 : config.NetStone.MaxAgeMounts,
            FallbackType.Any, cancellationToken);

        try
        {
            await Task.WhenAll(taskCharacter, taskClassJobs, taskMinions, taskMounts);
        }
        catch (NotFoundException)
        {
            // do nothing
        }

        FreeCompanyDtoV3? freeCompany = null;
        if (taskCharacter.Result.FreeCompany is not null)
        {
            try
            {
                // character page does not return FC tag, so we need to queue full free company
                // returning tag of cached fc is not a good idea either because it might be outdated
                freeCompany = await apiFreeCompany.GetAsync(taskCharacter.Result.FreeCompany.Id,
                    request.ForceRefresh ? 0 : config.NetStone.MaxAgeFreeCompany, FallbackType.Any, cancellationToken);
            }
            catch (NotFoundException)
            {
                // do nothing
            }
        }

        var image = await characterSheetService.CreateCharacterSheetAsync(taskCharacter.Result,
            !taskClassJobs.IsFaulted ? taskClassJobs.Result : null,
            !taskMinions.IsFaulted ? taskMinions.Result : null,
            !taskMounts.IsFaulted ? taskMounts.Result : null,
            freeCompany);

        var now = DateTime.UtcNow;

        var metadata = new List<SheetMetadata>
        {
            new("Character", taskCharacter.Result.LastUpdated ?? now, taskCharacter.Result.FallbackUsed,
                CreateFallbackMessage(taskCharacter.Result.FallbackReason))
        };

        if (!taskClassJobs.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Jobs", taskClassJobs.Result.LastUpdated ?? now,
                taskClassJobs.Result.FallbackUsed, CreateFallbackMessage(taskClassJobs.Result.FallbackReason)));
        }

        if (!taskMinions.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Minions", taskMinions.Result.LastUpdated ?? now,
                taskMinions.Result.FallbackUsed, CreateFallbackMessage(taskMinions.Result.FallbackReason)));
        }

        if (!taskMounts.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Mounts", taskMounts.Result.LastUpdated ?? now,
                taskMounts.Result.FallbackUsed, CreateFallbackMessage(taskMounts.Result.FallbackReason)));
        }

        if (freeCompany is not null)
        {
            metadata.Add(new SheetMetadata("Free Company", freeCompany.LastUpdated ?? now, freeCompany.FallbackUsed,
                CreateFallbackMessage(freeCompany.FallbackReason)));
        }

        return new CharacterSheetResponse(image, metadata, !taskMinions.IsFaulted, !taskMounts.IsFaulted,
            taskCharacter.Result, freeCompany);
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