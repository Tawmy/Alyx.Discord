using MediatR;

namespace Alyx.Discord.Core.Requests.Character.SetLastForceRefreshFreeCompany;

public record SetLastForceRefreshFreeCompanyRequest(string LodestoneId, DateTime LastForceRefresh) : IRequest;