using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Requests.UserContextMenu.CharacterSheet;

internal record UserContextMenuCharacterSheetRequest(SlashCommandContext Ctx, DiscordUser User)
    : UserContextMenuRequest(Ctx, User), IRequest;