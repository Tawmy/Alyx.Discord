using System.Diagnostics;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
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

        var claimedCharactersLength = stats.ClaimedCharacters.ToString().Length;
        var sheetsRequestedLength = stats.SheetsRequested.ToString().Length;
        var maxLength = Math.Max(claimedCharactersLength, sheetsRequestedLength);

        components.Add(
            new DiscordTextDisplayComponent(
                $"""
                 ## Statistics
                 ```
                 Claimed Characters     {Padding(claimedCharactersLength, maxLength)}{stats.ClaimedCharacters}
                 Sheets Requested       {Padding(sheetsRequestedLength, maxLength)}{stats.SheetsRequested}
                 ```
                 """));

        components.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

        components.Add(new DiscordTextDisplayComponent(
            $"""
             ## Materials
             {Messages.Commands.General.About.Squex}
             ## Libraries
             -# {Formatter.MaskedUrl("Autofac", new Uri("https://github.com/autofac/Autofac"))} • MIT License
             -# {Formatter.MaskedUrl("DSharpPlus", new Uri("https://github.com/DSharpPlus/DSharpPlus"))} • MIT License
             -# {Formatter.MaskedUrl("EFCore.NamingConventions", new Uri("https://github.com/efcore/EFCore.NamingConventions"))} • Apache-2.0 License
             -# {Formatter.MaskedUrl("efcore.pg", new Uri("https://github.com/npgsql/efcore.pg"))} • PostgreSQL License
             -# {Formatter.MaskedUrl("EntityFramework.Exceptions", new Uri("https://github.com/Giorgi/EntityFramework.Exceptions"))} • Apache-2.0 License
             -# {Formatter.MaskedUrl("FluentValidation", new Uri("https://github.com/FluentValidation/FluentValidation"))} • Apache-2.0 License
             -# {Formatter.MaskedUrl("ImageSharp.Drawing", new Uri("https://github.com/SixLabors/ImageSharp.Drawing"))} • Six Labors Split License
             -# {Formatter.MaskedUrl("MediatR", new Uri("https://github.com/jbogard/MediatR"))} • Apache-2.0 License

             Built on top of .NET and many Microsoft libraries
             -# All licensed under the MIT License
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
            $"-# Created by {Formatter.MaskedUrl("Tawmy", new Uri("https://tawmy.dev"))}"));

        builder.AddContainerComponent(new DiscordContainerComponent(components));
        builder.AsEphemeral(request.IsPrivate);

        await request.Ctx.RespondAsync(builder);

        return;

        string Padding(int length, int max)
        {
            return new string(' ', max - length);
        }
    }
}