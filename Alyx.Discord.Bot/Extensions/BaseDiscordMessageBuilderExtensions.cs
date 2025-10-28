using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Entities;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Bot.Extensions;

internal static class BaseDiscordMessageBuilderExtensions
{
    public static T AddError<T>(this BaseDiscordMessageBuilder<T> builder, string description, string? title = null)
        where T : BaseDiscordMessageBuilder<T>
    {
        builder.EnableV2Components();

        List<DiscordComponent> components = [];

        if (title is not null)
        {
            components.Add(new DiscordTextDisplayComponent($"## {title}"));
        }

        components.Add(new DiscordTextDisplayComponent(description));

        builder.AddContainerComponent(new DiscordContainerComponent(components, color: DiscordColor.Red));

        return (T)builder;
    }

    public static T AddTieBreakerSelect<T>(this BaseDiscordMessageBuilder<T> builder, DiscordSelectComponent select,
        int resultsTotal)
        where T : BaseDiscordMessageBuilder<T>
    {
        builder.EnableV2Components();
        builder.AddContainerComponent(new DiscordContainerComponent([
            new DiscordTextDisplayComponent($"### {Messages.Commands.Character.Get.SelectMenuTitle}"),
            new DiscordActionRowComponent([select]),
            new DiscordTextDisplayComponent($"-# {Messages.Commands.Character.Get.SelectMenuFooter(resultsTotal)}")
        ]));
        return (T)builder;
    }

    public static async Task<MemoryStream> AddImageAsync<T>(this BaseDiscordMessageBuilder<T> builder, Image image,
        string fileName, CancellationToken cancellationToken = default) where T : BaseDiscordMessageBuilder<T>
    {
        var stream = new MemoryStream();
        await image.SaveAsync(stream, new WebpEncoder(), cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        var fileNameWithExtension = $"{fileName}.webp";
        builder.AddFile(fileNameWithExtension, stream);
        return stream;
    }
}