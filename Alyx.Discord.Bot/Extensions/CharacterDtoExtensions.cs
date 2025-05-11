using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.Entities;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot.Extensions;

internal static class CharacterDtoExtensions
{
    public static DiscordSectionComponent ToSectionComponent(this CharacterDtoV3 character,
        CachingService cachingService, bool fullPortrait = false)
    {
        var jobPrefix = cachingService.TryGetApplicationEmoji(character.ActiveClassJob.ToString(), out var jobIcon)
            ? $"{jobIcon} "
            : string.Empty;
        var job = character.ActiveClassJob.GetSpaceSeparatedDisplayString();
        var homeWorldEmoji = cachingService.GetApplicationEmoji("homeWorld");

        return new DiscordSectionComponent(
            new DiscordTextDisplayComponent($"""
                                             # {character.Name}
                                             {jobPrefix}{job} • Lv. {character.ActiveClassJobLevel}
                                             -# {Formatter.Emoji(homeWorldEmoji)} {character.Server}
                                             """),
            new DiscordThumbnailComponent(fullPortrait ? character.Portrait : character.Avatar)
        );
    }
}