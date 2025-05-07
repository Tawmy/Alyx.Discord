using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Attributes.Me;

internal record CharacterAttributesMeRequest(SlashCommandContext Ctx, bool ForceRefresh, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;