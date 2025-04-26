using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.EventHandlers;

/// <summary>
///     Event is fired early during startup -> we can be sure its code is run before commands are processed.
/// </summary>
internal class ClientStartedEventHandler(CachingService cachingService) : IEventHandler<ClientStartedEventArgs>
{
    public async Task HandleEventAsync(DiscordClient sender, ClientStartedEventArgs eventArgs)
    {
        cachingService.CacheBannerUrl(sender.CurrentUser.BannerUrl);
        cachingService.CacheApplicationEmojis(await sender.GetApplicationEmojisAsync());
    }
}