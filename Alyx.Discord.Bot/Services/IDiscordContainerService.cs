using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Services;

public interface IDiscordContainerService<T>
{
    Task<DiscordContainerComponent> CreateContainerAsync(T entity);

    Task<DiscordContainerComponent> CreateContainerAsync(string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default);
}