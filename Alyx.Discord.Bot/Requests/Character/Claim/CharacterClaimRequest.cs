using DSharpPlus.Commands.Processors.SlashCommands;
using MediatR;

namespace Alyx.Discord.Bot.Requests.Character.Claim;

public record CharacterClaimRequest(SlashCommandContext Ctx, string Name, string World) : IRequest;