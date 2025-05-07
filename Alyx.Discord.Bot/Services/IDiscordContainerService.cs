using DSharpPlus.Entities;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Bot.Services;

public interface IDiscordContainerService
{
    Task<DiscordContainerComponent> CreateContainerAsync(CharacterDtoV3 character);

    Task<DiscordContainerComponent> CreateContainerAsync(string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default);
}