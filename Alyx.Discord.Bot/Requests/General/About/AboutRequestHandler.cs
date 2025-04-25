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
        var builder = new DiscordInteractionResponseBuilder().EnableV2Components();

        List<DiscordComponent> components = [];

        var currentProcess = Process.GetCurrentProcess();
        var startedAt = currentProcess.StartTime.ToUniversalTime();

        components.Add(new DiscordSectionComponent(
            [
                new DiscordTextDisplayComponent($"# {request.Ctx.Client.CurrentUser.Username}"),
                new DiscordTextDisplayComponent("""
                                                My name's Alyx.
                                                I'm here to provide support.
                                                """)
            ],
            new DiscordThumbnailComponent(request.Ctx.Client.CurrentUser.AvatarUrl)
        ));

        components.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

        components.Add(new DiscordSectionComponent(
            new DiscordTextDisplayComponent($"### Version **{version.ToString(3)}**"),
            new DiscordLinkButtonComponent("https://wiki.tawmy.net/books/alyx/page/release-notes", "Release Notes")
        ));

        components.Add(new DiscordSectionComponent(
            new DiscordTextDisplayComponent($"""
                                             Started {Formatter.Timestamp(startedAt)}
                                             {Formatter.Timestamp(startedAt, TimestampFormat.ShortDateTime)}
                                             """),
            new DiscordLinkButtonComponent("https://alyx.status.tawmy.net", "Status")
        ));

        components.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

        var stats = await sender.Send(new StatisticsRequest(), cancellationToken);

        components.Add(
            new DiscordTextDisplayComponent($"""
                                             ### Statistics
                                             Claimed Characters: `{stats.ClaimedCharacters.ToString()}`
                                             """));

        if (cachingService.GetBannerUrl() is { } bannerUrl)
        {
            components.Add(new DiscordSeparatorComponent());
            var desc = $"Banner for {request.Ctx.Client.CurrentUser.Username}";
            components.Add(new DiscordMediaGalleryComponent(new DiscordMediaGalleryItem(bannerUrl, desc, false)));
        }
        else
        {
            components.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));
        }

        components.Add(new DiscordTextDisplayComponent(
            $"Created by {Formatter.MaskedUrl("Tawmy", new Uri("https://tawmy.dev"))}"));

        builder.AddContainerComponent(new DiscordContainerComponent(components));
        builder.AsEphemeral(request.IsPrivate);

        await request.Ctx.RespondAsync(builder);
    }
}