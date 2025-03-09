using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Services;
using Alyx.Discord.Core.Structs;
using MediatR;
using NetStone.Api.Client;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

internal class CharacterSheetRequestHandler(
    AlyxConfiguration config,
    NetStoneApiClient client,
    CharacterSheetService characterSheetService)
    : IRequestHandler<CharacterSheetRequest, CharacterSheet>
{
    public async Task<CharacterSheet> Handle(CharacterSheetRequest request,
        CancellationToken cancellationToken)
    {
        var id = request.LodestoneId;

        var taskCharacter = client.Character.GetAsync(id, config.NetStone.MaxAgeCharacter, true, cancellationToken);
        var taskClassJobs = client.Character.GetClassJobsAsync(id, config.NetStone.MaxAgeClassJobs, true,
            cancellationToken);
        var taskMinions = client.Character.GetMinionsAsync(id, config.NetStone.MaxAgeMinions, true, cancellationToken);
        var taskMounts = client.Character.GetMountsAsync(id, config.NetStone.MaxAgeMounts, true, cancellationToken);

        try
        {
            await Task.WhenAll(taskCharacter, taskClassJobs, taskMinions, taskMounts);
        }
        catch (NotFoundException)
        {
            // TODO replace with Resilience, retry 1 or 2 times instead of proceeding without certain data
            // note: may throw NotFoundException when no mounts or if page private?
            if (taskCharacter.IsFaulted)
            {
                throw;
            }
        }

        FreeCompanyDto? freeCompany = null;
        if (taskCharacter.Result.FreeCompany is not null)
        {
            try
            {
                // TODO check if parser can be modified to return FC tag from character profile
                // It'd be nice if we could skip this step just to retrieve the FC tag
                freeCompany = await client.FreeCompany.GetAsync(taskCharacter.Result.FreeCompany.Id,
                    config.NetStone.MaxAgeFreeCompany, true, cancellationToken);
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
                taskCharacter.Result.FallbackReason)
        };

        if (!taskClassJobs.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Jobs", taskClassJobs.Result.LastUpdated ?? now,
                taskClassJobs.Result.FallbackUsed, taskClassJobs.Result.FallbackReason));
        }

        if (!taskMinions.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Minions", taskMinions.Result.LastUpdated ?? now,
                taskMinions.Result.FallbackUsed, taskMinions.Result.FallbackReason));
        }

        if (!taskMounts.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Mounts", taskMounts.Result.LastUpdated ?? now,
                taskMounts.Result.FallbackUsed, taskMounts.Result.FallbackReason));
        }

        if (freeCompany is not null)
        {
            metadata.Add(new SheetMetadata("Free Company", freeCompany.LastUpdated ?? now, freeCompany.FallbackUsed,
                freeCompany.FallbackReason));
        }

        return new CharacterSheet(image, metadata);
    }
}