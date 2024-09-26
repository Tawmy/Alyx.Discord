using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Unclaim;

internal record CharacterUnclaimRequest(SlashCommandContext Ctx) : SlashCommandRequest(Ctx), IRequest;