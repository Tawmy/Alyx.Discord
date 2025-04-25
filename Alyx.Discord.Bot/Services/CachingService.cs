using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Services;

internal class CachingService
{
    private IReadOnlyList<DiscordEmoji> _applicationEmojis = [];
    private string? _bannerUrl;

    public void CacheBannerUrl(string url)
    {
        _bannerUrl = url;
    }

    public void CacheApplicationEmojis(IReadOnlyList<DiscordEmoji> emojis)
    {
        _applicationEmojis = emojis;
    }

    public string? GetBannerUrl()
    {
        return _bannerUrl;
    }

    public IReadOnlyList<DiscordEmoji> GetApplicationEmojis()
    {
        return _applicationEmojis;
    }

    public DiscordEmoji GetApplicationEmoji(string name)
    {
        if (_applicationEmojis.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) is not
            { } emoji)
        {
            throw new InvalidOperationException($"Emoji {name} not found");
        }

        return emoji;
    }

    public bool TryGetApplicationEmoji(string name, out DiscordEmoji? emoji)
    {
        emoji = _applicationEmojis.FirstOrDefault(x => x.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        return emoji is not null;
    }
}