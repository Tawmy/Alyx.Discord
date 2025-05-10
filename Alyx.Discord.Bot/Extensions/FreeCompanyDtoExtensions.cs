using DSharpPlus;
using DSharpPlus.Entities;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.Extensions;

internal static class FreeCompanyDtoExtensions
{
    public static DiscordSectionComponent ToSectionComponent(this FreeCompanyDtoV3 fc, DiscordEmoji homeWorld)
    {
        var crestLayer = fc.CrestLayers.TopLayer ?? fc.CrestLayers.MiddleLayer ?? fc.CrestLayers.BottomLayer
            ?? throw new InvalidOperationException("One of the crest layers must not be null.");

        return new DiscordSectionComponent(
            new DiscordTextDisplayComponent(
                $"""
                 # {fc.Name}
                 {fc.Tag}
                 -# {Formatter.Emoji(homeWorld)} {fc.World}
                 """),
            new DiscordThumbnailComponent(crestLayer)); // TODO use uploaded crest once possible
    }
}