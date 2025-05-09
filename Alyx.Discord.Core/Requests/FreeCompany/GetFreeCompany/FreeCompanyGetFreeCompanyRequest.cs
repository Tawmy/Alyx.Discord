using MediatR;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompany;

public record FreeCompanyGetFreeCompanyRequest(string LodestoneId, int? MaxAge = null) : IRequest<FreeCompanyDtoV3>;