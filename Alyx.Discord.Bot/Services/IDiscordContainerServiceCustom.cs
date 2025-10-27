using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Services;

public interface IDiscordContainerServiceCustom<T, in TCustom>
{
    Task<DiscordContainerComponent> CreateContainerAsync(TCustom customData, T entity,
        CancellationToken cancellationToken = default);

    Task<DiscordContainerComponent> CreateContainerAsync(TCustom customData, string lodestoneId,
        bool forceRefresh = false, CancellationToken cancellationToken = default);

    Task<(DiscordContainerComponent, T)> RetrieveDataAndCreateContainerAsync(TCustom customData, string lodestoneId,
        bool forceRefresh = false, CancellationToken cancellationToken = default);
}