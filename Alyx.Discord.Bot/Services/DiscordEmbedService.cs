using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Services;

internal class DiscordEmbedService
{
    public DiscordEmbed Create(string description,
        string? title = null,
        string? author = null,
        Uri? url = null,
        Uri? imageUrl = null,
        Thumbnail? thumbnail = null)
    {
        var builder = new DiscordEmbedBuilder();
        builder.WithColor(DiscordColor.Blurple);

        builder.WithDescription(description);

        if (title is not null)
        {
            builder.WithTitle(title);
        }

        if (author is not null)
        {
            builder.WithAuthor(author);
        }

        if (url is not null)
        {
            builder.WithUrl(url);
        }

        if (imageUrl is not null)
        {
            builder.WithImageUrl(imageUrl);
        }

        if (thumbnail is not null)
        {
            builder.WithThumbnail(thumbnail.Value.Url, thumbnail.Value.Width, thumbnail.Value.Height);
        }

        return builder.Build();
    }

    public DiscordEmbed CreateError(string description, string? title = null)
    {
        var builder = new DiscordEmbedBuilder();
        builder.WithColor(DiscordColor.Red);

        builder.WithDescription(description);

        if (title is not null)
        {
            builder.WithTitle(title);
        }

        return builder.Build();
    }

    public readonly record struct Thumbnail(Uri Url, int Width = 0, int Height = 0);
}