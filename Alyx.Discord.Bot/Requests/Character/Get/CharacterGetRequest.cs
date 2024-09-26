using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Get;

internal record CharacterGetRequest(SlashCommandContext Ctx, string Name, string World, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;