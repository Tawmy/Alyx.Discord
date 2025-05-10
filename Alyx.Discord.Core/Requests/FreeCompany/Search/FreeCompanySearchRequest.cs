using MediatR;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Core.Requests.FreeCompany.Search;

public record FreeCompanySearchRequest(string Name, string World)
    : IRequest<ICollection<FreeCompanySearchPageResultDto>>;