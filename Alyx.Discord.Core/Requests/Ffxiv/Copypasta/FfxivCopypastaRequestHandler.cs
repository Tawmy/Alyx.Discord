using Alyx.Discord.Core.Services;
using MediatR;

namespace Alyx.Discord.Core.Requests.Ffxiv.Copypasta;

internal class FfxivCopypastaRequestHandler(ExternalResourceService externalResourceService)
    : IRequestHandler<FfxivCopypastaRequest, FfxivCopypastaRequestResponse>
{
    public async Task<FfxivCopypastaRequestResponse> Handle(FfxivCopypastaRequest request,
        CancellationToken cancellationToken)
    {
        var image = await externalResourceService.GetOtherImageAsync("copypasta");
        return new FfxivCopypastaRequestResponse(image);
    }
}