using MediatR;

namespace Alyx.Discord.Core.Requests.Character.GetLastForceRefresh;

public record GetLastForceRefreshRequest(string LodestoneId) : IRequest<GetLastForceRefreshRequestResponse>;