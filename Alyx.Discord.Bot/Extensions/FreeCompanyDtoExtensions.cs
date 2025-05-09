using DSharpPlus;
using DSharpPlus.Entities;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.Extensions;

internal static class FreeCompanyDtoExtensions
{
    public static DiscordSectionComponent ToSectionComponent(this FreeCompanyDtoV3 fc, DiscordEmoji homeWorld)
    {
        return new DiscordSectionComponent(
            new DiscordTextDisplayComponent(
                $"""
                 # {fc.Name}
                 {fc.Tag}
                 -# {Formatter.Emoji(homeWorld)} {fc.World}
                 """),
            new DiscordThumbnailComponent("attachment://crest.webp"));
    }
}