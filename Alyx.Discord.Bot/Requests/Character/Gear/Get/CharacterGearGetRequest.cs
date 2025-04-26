using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Gear.Get;

internal record CharacterGearGetRequest(SlashCommandContext Ctx, string Name, string World, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;