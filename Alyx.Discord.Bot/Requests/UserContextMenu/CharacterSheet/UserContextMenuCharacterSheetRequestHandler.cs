using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.UserContextMenu.CharacterSheet;

internal class UserContextMenuCharacterSheetRequestHandler(
    ISender sender,
    DiscordEmbedService embedService,
    IInteractionDataService interactionDataService) : IRequestHandler<UserContextMenuCharacterSheetRequest>
{
    public async Task Handle(UserContextMenuCharacterSheetRequest request, CancellationToken cancellationToken)
    {
        string lodestoneId;
        try
        {
            lodestoneId = await sender.Send(new GetMainCharacterIdRequest(request.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var embed = embedService.CreateError(Messages.UserContextMenus.CharacterSheet.NotFoundDescription,
                Messages.UserContextMenus.CharacterSheet.NotFoundTitle);
            await request.Ctx.RespondAsync(new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
            return;
        }

        await request.Ctx.DeferResponseAsync();

        var builder = new DiscordInteractionResponseBuilder();
        await builder.CreateSheetAndSendFollowupAsync(sender, interactionDataService, lodestoneId,
            async b => await request.Ctx.FollowupAsync(b), cancellationToken);
    }
}