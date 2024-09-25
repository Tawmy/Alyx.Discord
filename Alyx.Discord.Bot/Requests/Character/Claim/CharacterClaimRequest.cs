using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Claim;

internal record CharacterClaimRequest(SlashCommandContext Ctx, string Name, string World)
    : SlashCommandRequest(Ctx), IRequest;