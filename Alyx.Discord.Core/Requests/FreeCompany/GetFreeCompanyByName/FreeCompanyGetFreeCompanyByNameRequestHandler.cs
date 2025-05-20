using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompanyByName;

public class FreeCompanyGetFreeCompanyByNameRequestHandler(INetStoneApiFreeCompany apiFc)
    : IRequestHandler<FreeCompanyGetFreeCompanyByNameRequest, FreeCompanyDto>
{
    public Task<FreeCompanyDto> Handle(FreeCompanyGetFreeCompanyByNameRequest request,
        CancellationToken cancellationToken)
    {
        return apiFc.GetByNameAsync(request.Name, request.World, cancellationToken);
    }
}