using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus;
using DSharpPlus.Entities;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.Extensions;

internal static class FreeCompanyDtoExtensions
{
    public static DiscordSectionComponent ToSectionComponent(this FreeCompanyDto fc, CachingService cachingService)
    {
        var homeWorldEmoji = cachingService.GetApplicationEmoji("homeWorld");

        return new DiscordSectionComponent(
            new DiscordTextDisplayComponent(
                $"""
                 # {fc.Name}
                 {fc.Tag}
                 -# {Formatter.Emoji(homeWorldEmoji)} {fc.World}
                 """),
            new DiscordThumbnailComponent($"attachment://{Messages.FileNames.Crest}.webp"));
    }
}