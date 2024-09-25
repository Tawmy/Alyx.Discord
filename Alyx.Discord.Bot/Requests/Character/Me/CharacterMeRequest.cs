using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Me;

internal record CharacterMeRequest(SlashCommandContext Ctx) : SlashCommandRequest(Ctx), IRequest;