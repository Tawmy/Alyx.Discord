using Alyx.Discord.Core.Configuration;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.EventHandlers;

/// <summary>
///     Event is fired when bot is fully ready.
/// </summary>
internal class GuildDownloadCompletedEventHandler(AlyxConfiguration config)
    : IEventHandler<GuildDownloadCompletedEventArgs>
{
    public Task HandleEventAsync(DiscordClient discordClient, GuildDownloadCompletedEventArgs eventArgs)
    {
        return config.StatusMessage is not null
            ? discordClient.UpdateStatusAsync(new DiscordActivity(config.StatusMessage, DiscordActivityType.Custom))
            : Task.CompletedTask;
    }
}