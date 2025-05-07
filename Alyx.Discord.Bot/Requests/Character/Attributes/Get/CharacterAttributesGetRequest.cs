using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Attributes.Get;

internal record CharacterAttributesGetRequest(SlashCommandContext Ctx, string Name, string World, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;