using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Enums;

namespace Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompany;

public class FreeCompanyGetFreeCompanyRequestHandler(INetStoneApiFreeCompany apiFc)
    : IRequestHandler<FreeCompanyGetFreeCompanyRequest, FreeCompanyDto>
{
    public Task<FreeCompanyDto> Handle(FreeCompanyGetFreeCompanyRequest request, CancellationToken cancellationToken)
    {
        return apiFc.GetAsync(request.LodestoneId, request.MaxAge, FallbackTypeV4.Any, cancellationToken);
    }
}