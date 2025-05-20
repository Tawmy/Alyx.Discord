using MediatR;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompanyByName;

public record FreeCompanyGetFreeCompanyByNameRequest(string Name, string World) : IRequest<FreeCompanyDto>;