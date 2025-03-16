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

        var taskCharacter = client.Character.GetAsync(id, config.NetStone.MaxAgeCharacter, cancellationToken);
        var taskClassJobs = client.Character.GetClassJobsAsync(id, config.NetStone.MaxAgeClassJobs, cancellationToken);
        var taskMinions = client.Character.GetMinionsAsync(id, config.NetStone.MaxAgeMinions, cancellationToken);
        var taskMounts = client.Character.GetMountsAsync(id, config.NetStone.MaxAgeMounts, cancellationToken);

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
                    config.NetStone.MaxAgeFreeCompany, cancellationToken);
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

        var metadata = new List<SheetMetadata> { new("Character", taskCharacter.Result.LastUpdated) };
        if (!taskClassJobs.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Jobs", taskClassJobs.Result.LastUpdated ?? now));
        }

        if (!taskMinions.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Minions", taskMinions.Result.LastUpdated ?? now));
        }

        if (!taskMounts.IsFaulted)
        {
            metadata.Add(new SheetMetadata("Mounts", taskMounts.Result.LastUpdated ?? now));
        }

        if (freeCompany is not null)
        {
            metadata.Add(new SheetMetadata("Free Company", freeCompany.LastUpdated));
        }

        return new CharacterSheet(image, metadata, !taskMinions.IsFaulted, !taskMounts.IsFaulted);
    }
}