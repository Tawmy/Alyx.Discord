using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Requests;

internal record UserContextMenuRequest(SlashCommandContext Ctx, DiscordUser User) : SlashCommandRequest(Ctx);