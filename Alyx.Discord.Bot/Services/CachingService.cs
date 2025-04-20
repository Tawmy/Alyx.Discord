namespace Alyx.Discord.Bot.Services;

internal class CachingService
{
    private string? _bannerUrl;

    public void CacheBannerUrl(string url)
    {
        _bannerUrl = url;
    }

    public string? GetBannerUrl()
    {
        return _bannerUrl;
    }
}