using Alyx.Discord.Core.Services;
using MediatR;
using NetStone.Api.Client;
using NetStone.Common.Exceptions;
using SixLabors.ImageSharp;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

internal class CharacterSheetRequestHandler(NetStoneApiClient client, CharacterSheetService characterSheetService)
    : IRequestHandler<CharacterSheetRequest, Image>
{
    public async Task<Image> Handle(CharacterSheetRequest request, CancellationToken cancellationToken)
    {
        var id = request.LodestoneId;

        var taskCharacter = client.Character.GetAsync(id, cancellationToken: cancellationToken);
        var taskClassJobs = client.Character.GetClassJobsAsync(id, cancellationToken: cancellationToken);
        var taskMinions = client.Character.GetMinionsAsync(id, cancellationToken: cancellationToken);
        var taskMounts = client.Character.GetMountsAsync(id, cancellationToken: cancellationToken);

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

        return await characterSheetService.CreateCharacterSheetAsync(taskCharacter.Result,
            !taskClassJobs.IsFaulted ? taskClassJobs.Result : null,
            !taskMinions.IsFaulted ? taskMinions.Result : null,
            !taskMounts.IsFaulted ? taskMounts.Result : null);
    }
}