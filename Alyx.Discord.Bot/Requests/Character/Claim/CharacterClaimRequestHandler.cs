using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Claim.CharacterClaimRequest;

namespace Alyx.Discord.Bot.Requests.Character.Claim;

internal class CharacterClaimRequestHandler(ISender sender, IDataPersistenceService dataPersistenceService)
    : IRequestHandler<CharacterClaimRequest>
{
    public async Task Handle(CharacterClaimRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(true);

        var builder = new DiscordInteractionResponseBuilder();

        ICollection<CharacterSearchPageResultDto> searchDtos;
        try
        {
            searchDtos = await sender.Send(new CharacterSearchRequest(request.Name, request.World), cancellationToken);
        }
        catch (NotFoundException)
        {
            var content = $"Could not find {request.Name} on {request.World}.";
            builder.WithContent(content);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        if (searchDtos.Count > 1)
        {
            // TODO move into extension method that replies with standardised error message
            var content = $"Multiple results found for {request.Name} on {request.World}. Please enter an exact name.";
            builder.WithContent(content);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        var lodestoneId = searchDtos.First().Id;

        var coreRequest = new CoreRequest(request.Ctx.Interaction.User.Id, lodestoneId);
        var characterClaimRequestResponse = await sender.Send(coreRequest, cancellationToken);

        builder.AddClaimResponse(characterClaimRequestResponse, dataPersistenceService, lodestoneId);

        await request.Ctx.FollowupAsync(builder);
    }
}