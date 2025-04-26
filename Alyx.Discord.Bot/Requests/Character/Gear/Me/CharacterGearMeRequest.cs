using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Gear.Me;

internal record CharacterGearMeRequest(SlashCommandContext Ctx, bool ForceRefresh, bool IsPrivate)
    : SlashCommandRequest(Ctx), IRequest;