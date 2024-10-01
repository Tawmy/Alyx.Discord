using Alyx.Discord.Core.Services;
using MediatR;

namespace Alyx.Discord.Core.Requests.Ffxiv.Copypasta;

internal class FfxivCopypastaRequestHandler(ExternalResourceService externalResourceService)
    : IRequestHandler<FfxivCopypastaRequest, FfxivCopypastaRequestResponse>
{
    public Task<FfxivCopypastaRequestResponse> Handle(FfxivCopypastaRequest request,
        CancellationToken cancellationToken)
    {
        var image = externalResourceService.GetOtherImage("copypasta");
        return Task.FromResult(new FfxivCopypastaRequestResponse(image));
    }
}