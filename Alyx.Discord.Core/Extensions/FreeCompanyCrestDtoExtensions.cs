using NetStone.Common.DTOs.FreeCompany;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace Alyx.Discord.Core.Extensions;

public static class FreeCompanyCrestDtoExtensions
{
    public static async Task<Image> DownloadCrestAsync(this FreeCompanyCrestDto crest, HttpClient httpClient)
    {
        // TODO validate if free companies may skip one of these layers. If so, will prob throw exception.
        // TODO allow 128px crest size.

        List<string> layerUrls = [];

        if (!string.IsNullOrEmpty(crest.BottomLayer))
        {
            layerUrls.Add(crest.BottomLayer);
        }

        if (!string.IsNullOrEmpty(crest.MiddleLayer))
        {
            layerUrls.Add(crest.MiddleLayer);
        }

        if (!string.IsNullOrEmpty(crest.TopLayer))
        {
            layerUrls.Add(crest.TopLayer);
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
}