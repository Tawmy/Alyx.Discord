using DSharpPlus.Entities;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Bot.Extensions;

internal static class CharacterSearchPageResultDtoExtensions
{
    public static DiscordSelectComponent AsSelectComponent(this ICollection<CharacterSearchPageResultDto> dtos,
        string selectId)
    {
        var options = dtos
            .Take(25)
            .Select(x => new DiscordSelectComponentOption(x.Name, x.Id));

        return new DiscordSelectComponent(selectId, "Select Character", options, minOptions: 1, maxOptions: 1);
    }
}