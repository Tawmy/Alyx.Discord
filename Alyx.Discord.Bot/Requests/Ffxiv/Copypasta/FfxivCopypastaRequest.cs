using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Ffxiv.Copypasta;

internal record FfxivCopypastaRequest(SlashCommandContext Ctx, bool IsPrivate) : SlashCommandRequest(Ctx), IRequest;