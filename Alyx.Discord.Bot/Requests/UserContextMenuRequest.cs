using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Requests;

public record UserContextMenuRequest(SlashCommandContext Ctx, DiscordUser User);