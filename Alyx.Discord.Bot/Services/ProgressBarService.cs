using System.Text;
using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Services;

internal class ProgressBarService(CachingService cachingService)
{
    /// <summary>
    ///     Create a progress bar with the given style.
    /// </summary>
    /// <param name="style">The emoji used depend on this progress bar style.</param>
    /// <param name="length">Length of the progress bar in emoji</param>
    /// <param name="progress">Progress, out of 100</param>
    /// <returns></returns>
    public string CreateProgressBar(ProgressBarStyle style, short length, short progress)
    {
        var emoji = GetEmoji(style);

        var percentage = decimal.Divide(progress, 100);
        var filledEmoji = decimal.Multiply(percentage, length);
        var filledEmojiRounded = Math.Round(filledEmoji);
        var emojiAdded = 0;

        var sb = new StringBuilder();

        sb.Append(filledEmoji > 0 ? emoji.FilledStart : emoji.EmptyStart);
        emojiAdded++;

        for (var i = 0; i < filledEmojiRounded - 2; i++)
        {
            sb.Append(emoji.FilledMiddle);
            emojiAdded++;
        }

        if (filledEmojiRounded > 1)
        {
            sb.Append(emoji.FilledEnd);
            emojiAdded++;
        }

        for (var i = 0; i < length - emojiAdded - 1; i++)
        {
            sb.Append(emoji.EmptyMiddle);
        }

        if (filledEmojiRounded < length)
        {
            sb.Append(emoji.EmptyEnd);
        }

        return sb.ToString();
    }

    private ProgressBarEmoji GetEmoji(ProgressBarStyle style)
    {
        var es = style switch
        {
            ProgressBarStyle.ClassJobLevel => "es",
            ProgressBarStyle.GrandCompanyAffinity => "fcAffinityEmptyStart",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        var em = style switch
        {
            ProgressBarStyle.ClassJobLevel => "em",
            ProgressBarStyle.GrandCompanyAffinity => "fcAffinityEmptyMiddle",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        var ee = style switch
        {
            ProgressBarStyle.ClassJobLevel => "ee",
            ProgressBarStyle.GrandCompanyAffinity => "fcAffinityEmptyEnd",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        var fs = style switch
        {
            ProgressBarStyle.ClassJobLevel => "fs",
            ProgressBarStyle.GrandCompanyAffinity => "fcAffinityFilledStart",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        var fm = style switch
        {
            ProgressBarStyle.ClassJobLevel => "fm",
            ProgressBarStyle.GrandCompanyAffinity => "fcAffinityFilledMiddle",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        var fe = style switch
        {
            ProgressBarStyle.ClassJobLevel => "fe",
            ProgressBarStyle.GrandCompanyAffinity => "fcAffinityFilledEnd",
            _ => throw new ArgumentOutOfRangeException(nameof(style), style, null)
        };

        return new ProgressBarEmoji
        {
            EmptyStart = cachingService.GetApplicationEmoji(es),
            EmptyMiddle = cachingService.GetApplicationEmoji(em),
            EmptyEnd = cachingService.GetApplicationEmoji(ee),
            FilledStart = cachingService.GetApplicationEmoji(fs),
            FilledMiddle = cachingService.GetApplicationEmoji(fm),
            FilledEnd = cachingService.GetApplicationEmoji(fe)
        };
    }

    private record ProgressBarEmoji
    {
        public required DiscordEmoji EmptyStart { get; init; }
        public required DiscordEmoji EmptyMiddle { get; init; }
        public required DiscordEmoji EmptyEnd { get; init; }
        public required DiscordEmoji FilledStart { get; init; }
        public required DiscordEmoji FilledMiddle { get; init; }
        public required DiscordEmoji FilledEnd { get; init; }
    }
}

internal enum ProgressBarStyle
{
    ClassJobLevel,
    GrandCompanyAffinity
}