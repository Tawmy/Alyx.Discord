using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.General.About;

internal record AboutRequest(SlashCommandContext Ctx, bool IsPrivate) : SlashCommandRequest(Ctx), IRequest;