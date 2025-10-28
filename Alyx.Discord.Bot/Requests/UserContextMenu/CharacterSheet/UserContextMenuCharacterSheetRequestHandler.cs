using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.UserContextMenu.CharacterSheet;

internal class UserContextMenuCharacterSheetRequestHandler(ISender sender, CharacterSheetService sheetService)
    : IRequestHandler<UserContextMenuCharacterSheetRequest>
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
            var errorBuilder = new DiscordInteractionResponseBuilder().AddError(
                Messages.UserContextMenus.CharacterSheet.NotFoundDescription(request.GetSlashCommandMapping(),
                    "character claim", "ffxiv copypasta"), Messages.UserContextMenus.CharacterSheet.NotFoundTitle);
            await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
            return;
        }

        await request.Ctx.DeferResponseAsync();

        var builder = new DiscordInteractionResponseBuilder();
        await sheetService.CreateSheetAndSendFollowupAsync(builder, lodestoneId, false,
            async b => await request.Ctx.RespondAsync(b), cancellationToken);
    }
}