using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompany;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Enums;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot.Services;

internal class FreeCompanyService(
    ISender sender,
    AlyxConfiguration config,
    CachingService cachingService,
    IInteractionDataService interactionDataService)
    : IDiscordContainerService<FreeCompanyDtoV3>
{
    public const string Key = "fc";

    public async Task<DiscordContainerComponent> CreateContainerAsync(FreeCompanyDtoV3 fc,
        CancellationToken cancellationToken = default)
    {
        return new DiscordContainerComponent(await CreateComponentsAsync(fc, true));
    }

    public async Task<DiscordContainerComponent> CreateContainerAsync(string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var maxAge = forceRefresh ? 0 : config.NetStone.MaxAgeCharacter;
        var fc = await sender.Send(new FreeCompanyGetFreeCompanyRequest(lodestoneId, maxAge), cancellationToken);
        return new DiscordContainerComponent(await CreateComponentsAsync(fc, false));
    }

    private async Task<List<DiscordComponent>> CreateComponentsAsync(FreeCompanyDtoV3 fc, bool cachedFromSheet)
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

            var lastUpdatedStr = $"-# Last updated {Formatter.Timestamp(fc.LastUpdated.Value)}";

            var maxAgeFc = TimeSpan.FromMinutes(config.NetStone.MaxAgeFreeCompany);
            if (cachedFromSheet && DateTime.Now.Subtract(maxAgeFc) > fc.LastUpdated)
            {
                c.Add(new DiscordSectionComponent(
                    new DiscordTextDisplayComponent(
                        $"""
                         -# Free Company is from character sheet. It might be outdated.
                         {lastUpdatedStr}
                         """),
                    await CreateCharacterAttributesButtonAsync(fc)
                ));
            }
            else
            {
                c.Add(new DiscordTextDisplayComponent(lastUpdatedStr));
            }
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

    private async Task<DiscordButtonComponent> CreateCharacterAttributesButtonAsync(FreeCompanyDtoV3 fc)
    {
        var id = await interactionDataService.AddDataAsync(fc.Id, ComponentIds.Button.CharacterFreeCompany);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, id, Messages.Buttons.CurrentFreeCompany);
    }
}