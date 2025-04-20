using MediatR;

namespace Alyx.Discord.Core.Requests.Character.SetLastForceRefresh;

public record SetLastForceRefreshRequest(string LodestoneId, DateTime LastForceRefresh) : IRequest;