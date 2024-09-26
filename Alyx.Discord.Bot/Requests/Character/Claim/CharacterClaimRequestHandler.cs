using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Claim.CharacterClaimRequest;

namespace Alyx.Discord.Bot.Requests.Character.Claim;

internal class CharacterClaimRequestHandler(
    ISender sender,
    IDataPersistenceService dataPersistenceService,
    DiscordEmbedService embedService) : IRequestHandler<CharacterClaimRequest>
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
            var description = Messages.Commands.Character.Get.CharacterNotFound(request.Name, request.World);
            builder.AddEmbed(embedService.CreateError(description));
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        if (searchDtos.Count > 1)
        {
            var description = Messages.Commands.Character.Claim.MultipleResults(request.Name, request.World);
            builder.AddEmbed(embedService.Create(description));
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        var lodestoneId = searchDtos.First().Id;

        var coreRequest = new CoreRequest(request.Ctx.Interaction.User.Id, lodestoneId);
        var characterClaimRequestResponse = await sender.Send(coreRequest, cancellationToken);

        builder.AddClaimResponse(characterClaimRequestResponse, dataPersistenceService, embedService, lodestoneId);

        await request.Ctx.FollowupAsync(builder);
    }
}