using DSharpPlus.Commands.Processors.SlashCommands;

namespace Alyx.Discord.Bot.Requests;

internal abstract record SlashCommandRequest(SlashCommandContext Ctx);