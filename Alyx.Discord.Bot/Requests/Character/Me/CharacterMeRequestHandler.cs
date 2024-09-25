using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Me;

internal class CharacterMeRequestHandler(
    ISender sender,
    DiscordEmbedService embedService,
    IDataPersistenceService dataPersistenceService)
    : IRequestHandler<CharacterMeRequest>
{
    public async Task Handle(CharacterMeRequest request, CancellationToken cancellationToken)
    {
        string lodestoneId;
        try
        {
            lodestoneId = await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var embed = embedService.CreateError(Messages.Commands.Character.Me.NotFoundDescription,
                Messages.Commands.Character.Me.NotFoundTitle);
            await request.Ctx.RespondAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
            return;
        }

        await request.Ctx.DeferResponseAsync();

        var builder = new DiscordInteractionResponseBuilder();
        await builder.CreateSheetAndSendFollowupAsync(sender, dataPersistenceService, lodestoneId,
            async b => await request.Ctx.FollowupAsync(b), cancellationToken);
    }
}