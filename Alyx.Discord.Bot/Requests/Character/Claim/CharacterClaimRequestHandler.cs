using Alyx.Discord.Core.Requests.Character.Claim;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Claim.CharacterClaimRequest;

namespace Alyx.Discord.Bot.Requests.Character.Claim;

internal class CharacterClaimRequestHandler(ISender sender)
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

        var coreRequest = new CoreRequest(request.Ctx.Interaction.User.Id, searchDtos.First().Id);
        var response = await sender.Send(coreRequest, cancellationToken);

        switch (response.Status)
        {
            case CharacterClaimRequestStatus.AlreadyClaimedByUser:
                builder.WithContent("You've already claimed this character.");
                break;
            case CharacterClaimRequestStatus.AlreadyClaimedByDifferentUser:
                builder.WithContent("This character has already been claimed by someone else.");
                break;
            case CharacterClaimRequestStatus.ClaimAlreadyExistsForThisCharacter:
                CreateClaimInstructions(builder, response.Code!);
                break;
            case CharacterClaimRequestStatus.NewClaimCreated:
                CreateClaimInstructions(builder, response.Code!);
                break;
            case CharacterClaimRequestStatus.ClaimConfirmed:
                builder.WithContent("Claim confirmed! You can now request your character sheet using /character me.");
                break;
            case CharacterClaimRequestStatus.UserAlreadyHasMainCharacter:
                builder.WithContent("You've already claimed a different character.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(response), response, null);
        }

        await request.Ctx.FollowupAsync(builder);
    }

    private static void CreateClaimInstructions(DiscordInteractionResponseBuilder builder, string code)
    {
        // TODO more detailed instructions
        builder.WithContent($"Please put code `{code}` into your Lodestone profile description.");
    }
}