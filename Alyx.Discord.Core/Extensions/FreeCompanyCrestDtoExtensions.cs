using NetStone.Common.DTOs.FreeCompany;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;

namespace Alyx.Discord.Core.Extensions;

public static class FreeCompanyCrestDtoExtensions
{
    public static async Task<Image> DownloadCrestAsync(this FreeCompanyCrestDto crest, HttpClient httpClient)
    {
        // TODO validate if free companies may skip one of these layers. If so, will prob throw exception.
        // TODO allow 128px crest size.

        var dataTopLayer = await httpClient.GetByteArrayAsync(crest.TopLayer);
        var dataMiddleLayer = await httpClient.GetByteArrayAsync(crest.MiddleLayer);
        var dataBottomLayer = await httpClient.GetByteArrayAsync(crest.BottomLayer);

        var topLayer = Image.Load<Rgba32>(dataTopLayer);
        var middleLayer = Image.Load<Rgba32>(dataMiddleLayer);
        var bottomLayer = Image.Load<Rgba32>(dataBottomLayer);

        bottomLayer.Mutate(x => x.DrawImage(middleLayer, 1));
        bottomLayer.Mutate(x => x.DrawImage(topLayer, 1));

        return bottomLayer;
    }
}