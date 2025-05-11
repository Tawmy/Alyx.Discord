using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompany;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Enums;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot.Services;

internal class FreeCompanyService(ISender sender, AlyxConfiguration config, CachingService cachingService)
    : IDiscordContainerService<FreeCompanyDtoV3>
{
    public const string Key = "fc";

    public Task<DiscordContainerComponent> CreateContainerAsync(FreeCompanyDtoV3 fc,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(new DiscordContainerComponent(CreateComponents(fc, cancellationToken)));
    }

    public async Task<DiscordContainerComponent> CreateContainerAsync(string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var maxAge = forceRefresh ? 0 : config.NetStone.MaxAgeCharacter;
        var fc = await sender.Send(new FreeCompanyGetFreeCompanyRequest(lodestoneId, maxAge), cancellationToken);
        return await CreateContainerAsync(fc, cancellationToken);
    }

    private List<DiscordComponent> CreateComponents(FreeCompanyDtoV3 fc, CancellationToken cancellationToken = default)
    {
        var maelstrom = fc.Reputation.First(x => x.GrandCompany is GrandCompany.Maelstrom);
        var flames = fc.Reputation.First(x => x.GrandCompany is GrandCompany.ImmortalFlames);
        var adders = fc.Reputation.First(x => x.GrandCompany is GrandCompany.OrderOfTheTwinAdder);

        List<DiscordComponent> c =
        [
            fc.ToSectionComponent(cachingService)
        ];

        if (!string.IsNullOrWhiteSpace(fc.Slogan))
        {
            c.Add(new DiscordTextDisplayComponent(fc.Slogan));
        }

        c.AddRange(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            new DiscordTextDisplayComponent(
                $"""
                 Formed: {Formatter.Timestamp(fc.Formed, TimestampFormat.ShortDate)}
                 Members: **{fc.ActiveMemberCount}**
                 Rank: **{fc.Rank}**
                 """),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large)
        );

        if (fc.Focus.Any())
        {
            c.Add(CreateFocusComponent(fc));
            c.Add(new DiscordSeparatorComponent());
        }

        if (fc.Estate is not null)
        {
            c.Add(new DiscordTextDisplayComponent(
                $"""
                 ### Estate
                 **{fc.Estate.Name}**
                 {fc.Estate.Greeting}
                 -# {fc.Estate.Plot}
                 """));
            c.Add(new DiscordSeparatorComponent());
        }

        if (fc.RankingWeekly is not null || fc.RankingMonthly is not null)
        {
            c.Add(new DiscordTextDisplayComponent(
                $"""
                 ### Ranking
                 Weekly Rank: {fc.RankingWeekly}
                 Monthly Rank: {fc.RankingMonthly}
                 """));
            c.Add(new DiscordSeparatorComponent());
        }

        c.AddRange(new DiscordTextDisplayComponent("### Reputation"),
            new DiscordTextDisplayComponent(
                $"""
                 -# {Formatter.Emoji(cachingService.GetApplicationEmoji("maelstrom"))} {maelstrom.GrandCompany.GetDisplayName()}
                 {maelstrom.Rank}
                 """),
            new DiscordSeparatorComponent(),
            new DiscordTextDisplayComponent(
                $"""
                 -# {Formatter.Emoji(cachingService.GetApplicationEmoji("adders"))} {adders.GrandCompany.GetDisplayName()}
                 {adders.Rank}
                 """),
            new DiscordSeparatorComponent(),
            new DiscordTextDisplayComponent(
                $"""
                 -# {Formatter.Emoji(cachingService.GetApplicationEmoji("flames"))} {flames.GrandCompany.GetDisplayName()}
                 {flames.Rank}
                 """));

        if (fc.LastUpdated is not null)
        {
            c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));
            c.Add(new DiscordTextDisplayComponent($"-# Last updated {Formatter.Timestamp(fc.LastUpdated.Value)}"));
        }

        return c;
    }

    private DiscordTextDisplayComponent CreateFocusComponent(FreeCompanyDtoV3 fc)
    {
        var focusStrings = fc.Focus.Select(x =>
            $"{Formatter.Emoji(cachingService.GetApplicationEmoji(x.Type.ToString().ToLowerInvariant()))} {x.Name}");

        var text = $"""
                    ### Focus
                    {string.Join(" ", focusStrings)}
                    """;
        return new DiscordTextDisplayComponent(text);
    }
}