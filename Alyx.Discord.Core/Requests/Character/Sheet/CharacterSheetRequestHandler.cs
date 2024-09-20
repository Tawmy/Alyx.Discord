using Alyx.Discord.Core.Services;
using MediatR;
using NetStone.Api.Client;
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
        await Task.WhenAll(taskCharacter, taskClassJobs, taskMinions, taskMounts);

        return await characterSheetService.CreateCharacterSheetAsync(taskCharacter.Result, taskClassJobs.Result,
            taskMinions.Result, taskMounts.Result);
    }
}