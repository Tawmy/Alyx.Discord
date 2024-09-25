using Alyx.Discord.Core.Configuration;
using MediatR;
using NetStone.Api.Client;

namespace Alyx.Discord.Core.Requests.Character.SheetMetadata;

public class CharacterSheetMetadataRequestHandler(AlyxConfiguration config, NetStoneApiClient client)
    : IRequestHandler<CharacterSheetMetadataRequest, ICollection<Structs.SheetMetadata>>
{
    public async Task<ICollection<Structs.SheetMetadata>> Handle(CharacterSheetMetadataRequest request,
        CancellationToken cancellationToken)
    {
        var id = request.LodestoneId;
        var taskCharacter = client.Character.GetAsync(id, config.NetStone.MaxAgeCharacter, cancellationToken);
        var taskClassJobs = client.Character.GetClassJobsAsync(id, config.NetStone.MaxAgeClassJobs, cancellationToken);
        var taskMinions = client.Character.GetMinionsAsync(id, config.NetStone.MaxAgeMinions, cancellationToken);
        var taskMounts = client.Character.GetMountsAsync(id, config.NetStone.MaxAgeMounts, cancellationToken);

        // TODO resilience
        await Task.WhenAll(taskCharacter, taskClassJobs, taskMinions, taskMounts);

        var list = new List<Structs.SheetMetadata>
        {
            new("Character", taskCharacter.Result.LastUpdated),
            new("ClassJobs", request.SheetTimestamp),
            new("Mounts", taskMounts.Result.LastUpdated ?? request.SheetTimestamp),
            new("Minions", taskMinions.Result.LastUpdated ?? request.SheetTimestamp)
        };

        return list;
    }
}