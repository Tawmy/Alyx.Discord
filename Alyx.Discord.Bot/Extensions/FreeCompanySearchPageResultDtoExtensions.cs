using DSharpPlus.Entities;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.Extensions;

internal static class FreeCompanySearchPageResultDtoExtensions
{
    public static DiscordSelectComponent AsSelectComponent(this ICollection<FreeCompanySearchPageResultDto> dtos,
        string selectId)
    {
        var options = dtos
            .Take(25)
            .Select(x => new DiscordSelectComponentOption(x.Name, x.Id));

        return new DiscordSelectComponent(selectId, "Select Free Company", options, minOptions: 1, maxOptions: 1);
    }
}