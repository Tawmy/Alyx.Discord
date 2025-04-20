using System.Diagnostics;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Core.Requests.General.Statistics;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Requests.General.About;

internal class AboutRequestHandler(ISender sender, CachingService cachingService, Version version)
    : IRequestHandler<AboutRequest>
{
    public async Task Handle(AboutRequest request, CancellationToken cancellationToken)
    {
        var builder = new DiscordInteractionResponseBuilder();

        var embed = new DiscordEmbedBuilder();
        embed.WithTitle(request.Ctx.Client.CurrentUser.Username);
        embed.WithDescription("""
                              My name's Alyx.
                              I'm here to provide support.
                              """);
        embed.WithColor(DiscordColor.Gold);

        embed.WithThumbnail(request.Ctx.Client.CurrentUser.AvatarUrl);

        if (cachingService.GetBannerUrl() is { } bannerUrl)
        {
            embed.WithImageUrl(bannerUrl);
        }

        embed.AddField("Version", version.ToString(3));

        var currentProcess = Process.GetCurrentProcess();
        var startedAt = currentProcess.StartTime.ToUniversalTime();
        var startedAtFormatted = $"""
                                  Started {Formatter.Timestamp(startedAt)}
                                  {Formatter.Timestamp(startedAt, TimestampFormat.ShortDateTime)}
                                  """;
        embed.AddField("Uptime", startedAtFormatted);

        var stats = await sender.Send(new StatisticsRequest(), cancellationToken);

        embed.AddField("Claimed Characters", stats.ClaimedCharacters.ToString());

        var links = $"""
                     {Formatter.MaskedUrl("Home", new Uri("https://alyx.tawmy.net"))}
                     {Formatter.MaskedUrl("Release Notes", new Uri("https://wiki.tawmy.net/books/alyx/page/release-notes"))}
                     {Formatter.MaskedUrl("Status", new Uri("https://alyx.status.tawmy.net"))}
                     """;
        embed.AddField("External links", links);

        embed.WithFooter("tawmy.dev", "https://tawmy.dev/avatar.webp");

        builder.AddEmbed(embed);
        builder.AsEphemeral(request.IsPrivate);

        await request.Ctx.RespondAsync(builder);
    }
}