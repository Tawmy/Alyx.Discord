using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Enums;

namespace Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompany;

public class FreeCompanyGetFreeCompanyRequestHandler(INetStoneApiFreeCompany apiFc)
    : IRequestHandler<FreeCompanyGetFreeCompanyRequest, FreeCompanyDtoV3>
{
    public Task<FreeCompanyDtoV3> Handle(FreeCompanyGetFreeCompanyRequest request, CancellationToken cancellationToken)
    {
        return apiFc.GetAsync(request.LodestoneId, request.MaxAge, FallbackType.Any, cancellationToken);
    }
}