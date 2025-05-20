using NetStone.Common.DTOs.FreeCompany;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Alyx.Discord.Core.Extensions;

public static class FreeCompanyCrestDtoExtensions
{
    public static async Task<Image> DownloadCrestAsync(this FreeCompanyCrestDto crest, HttpClient httpClient)
    {
        List<string> layerUrls = [];

        if (!string.IsNullOrEmpty(crest.BottomLayer))
        {
            layerUrls.Add(GetHighestQualityLayerUrls(crest.BottomLayer));
        }

        if (!string.IsNullOrEmpty(crest.MiddleLayer))
        {
            layerUrls.Add(GetHighestQualityLayerUrls(crest.MiddleLayer));
        }

        if (!string.IsNullOrEmpty(crest.TopLayer))
        {
            layerUrls.Add(GetHighestQualityLayerUrls(crest.TopLayer));
        }

        List<byte[]> layerByteArrays = [];

        foreach (var layerUrl in layerUrls)
        {
            layerByteArrays.Add(await httpClient.GetByteArrayAsync(layerUrl));
        }

        List<Image> layerImages = [];

        foreach (var layerByteArray in layerByteArrays)
        {
            layerImages.Add(Image.Load(layerByteArray));
        }

        foreach (var image in layerImages.Skip(1))
        {
            layerImages[0].Mutate(x => x.DrawImage(image, 1));
        }

        return layerImages[0];
    }

    private static string GetHighestQualityLayerUrls(string url)
    {
        return url.EndsWith("40x40.png", StringComparison.OrdinalIgnoreCase) ||
               url.EndsWith("64x64.png", StringComparison.OrdinalIgnoreCase)
            ? $"{url[..^9]}128x128.png"
            : url;
    }
}