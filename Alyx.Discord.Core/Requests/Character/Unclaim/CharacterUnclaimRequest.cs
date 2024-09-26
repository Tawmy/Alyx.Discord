using MediatR;

namespace Alyx.Discord.Core.Requests.Character.Unclaim;

public record CharacterUnclaimRequest(ulong DiscordId) : IRequest;