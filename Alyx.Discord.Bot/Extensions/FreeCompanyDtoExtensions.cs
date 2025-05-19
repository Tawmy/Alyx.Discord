using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.Extensions;

internal static class FreeCompanyDtoExtensions
{
    public static DiscordSectionComponent ToSectionComponent(this FreeCompanyDtoV3 fc, CachingService cachingService)
    {
        var homeWorldEmoji = cachingService.GetApplicationEmoji("homeWorld");

        return new DiscordSectionComponent(
            new DiscordTextDisplayComponent(
                $"""
                 # {fc.Name}
                 {fc.Tag}
                 -# {Formatter.Emoji(homeWorldEmoji)} {fc.World}
                 """),
            new DiscordThumbnailComponent("attachment://crest.webp"));
    }
}