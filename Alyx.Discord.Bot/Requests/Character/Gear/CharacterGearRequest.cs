using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Gear;

internal record CharacterGearRequest(SlashCommandContext Ctx, string Name, string World, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;