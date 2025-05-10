using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Queries;

namespace Alyx.Discord.Core.Requests.FreeCompany.Search;

internal class FreeCompanySearchRequestHandler(INetStoneApiFreeCompany apiFc)
    : IRequestHandler<FreeCompanySearchRequest, ICollection<FreeCompanySearchPageResultDto>>
{
    public async Task<ICollection<FreeCompanySearchPageResultDto>> Handle(FreeCompanySearchRequest request,
        CancellationToken cancellationToken)
    {
        var query = new FreeCompanySearchQuery(request.Name, request.World);
        var searchPage = await apiFc.SearchAsync(query, cancellationToken: cancellationToken);
        return searchPage.Results.ToArray();
    }
}