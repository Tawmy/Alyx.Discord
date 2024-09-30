using Alyx.Discord.Bot.Requests.UserContextMenu.CharacterSheet;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

public class UserContextMenuCommands(ISender sender)
{
    [Command("Character Sheet")]
    [SlashCommandTypes(DiscordApplicationCommandType.UserContextMenu)]
    public Task ShowSheetAsync(SlashCommandContext ctx, DiscordUser user)
    {
        return sender.Send(new UserContextMenuCharacterSheetRequest(ctx, user));
    }
}