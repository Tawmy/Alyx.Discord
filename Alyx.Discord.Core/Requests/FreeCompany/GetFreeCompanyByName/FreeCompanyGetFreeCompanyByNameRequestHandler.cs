using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompanyByName;

public class FreeCompanyGetFreeCompanyByNameRequestHandler(INetStoneApiFreeCompany apiFc)
    : IRequestHandler<FreeCompanyGetFreeCompanyByNameRequest, FreeCompanyDtoV3>
{
    public Task<FreeCompanyDtoV3> Handle(FreeCompanyGetFreeCompanyByNameRequest request,
        CancellationToken cancellationToken)
    {
        return apiFc.GetByNameAsync(request.Name, request.World, cancellationToken);
    }
}