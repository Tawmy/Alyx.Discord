using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Entities;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Bot.Extensions;

internal static class CharacterSearchPageResultDtoExtensions
{
    public static DiscordSelectComponent AsSelectComponent(this ICollection<CharacterSearchPageResultDto> dtos)
    {
        var options = dtos
            .Take(25)
            .Select(x => new DiscordSelectComponentOption(x.Name, x.Id));

        var suffix = dtos.Count > 25
            ? $"(Showing 25/{dtos.Count} results)"
            : $"({dtos.Count} results)";

        return new DiscordSelectComponent(ComponentIds.Select.Character, $"Select Character {suffix}", options,
            minOptions: 1, maxOptions: 1);
    }
}