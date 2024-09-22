using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Services;
using MediatR;
using NetStone.Api.Client;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Exceptions;
using SixLabors.ImageSharp;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

internal class CharacterSheetRequestHandler(
    AlyxConfiguration config,
    NetStoneApiClient client,
    CharacterSheetService characterSheetService)
    : IRequestHandler<CharacterSheetRequest, Image>
{
    public async Task<Image> Handle(CharacterSheetRequest request,
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

        return await characterSheetService.CreateCharacterSheetAsync(taskCharacter.Result,
            !taskClassJobs.IsFaulted ? taskClassJobs.Result : null,
            !taskMinions.IsFaulted ? taskMinions.Result : null,
            !taskMounts.IsFaulted ? taskMounts.Result : null,
            freeCompany);
    }
}