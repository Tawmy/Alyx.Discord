using MediatR;

namespace Alyx.Discord.Core.Requests.Character.Claim;

public record CharacterClaimRequest(ulong DiscordId, string LodestoneId) : IRequest<CharacterClaimRequestResponse>;