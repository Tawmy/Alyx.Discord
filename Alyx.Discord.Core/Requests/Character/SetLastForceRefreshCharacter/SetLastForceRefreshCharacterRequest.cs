using MediatR;

namespace Alyx.Discord.Core.Requests.Character.SetLastForceRefreshCharacter;

public record SetLastForceRefreshCharacterRequest(string LodestoneId, DateTime LastForceRefresh) : IRequest;