using System.Text;
using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;
using NetStone.Common.Exceptions;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot.Services;

internal class CharacterAttributesService(
    ISender sender,
    AlyxConfiguration config,
    CachingService cachingService,
    IInteractionDataService interactionDataService)
    : IDiscordContainerService<CharacterDtoV3>
{
    public const string Key = "attributes";

    public async Task<DiscordContainerComponent> CreateContainerAsync(CharacterDtoV3 character,
        CancellationToken cancellationToken = default)
    {
        return new DiscordContainerComponent(await CreateComponentsAsync(character, true));
    }

    public async Task<DiscordContainerComponent> CreateContainerAsync(string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var maxAge = forceRefresh ? 0 : config.NetStone.MaxAgeCharacter;
        var character = await sender.Send(new CharacterGetCharacterRequest(lodestoneId, maxAge), cancellationToken);
        return new DiscordContainerComponent(await CreateComponentsAsync(character, false));
    }

    private async Task<List<DiscordComponent>> CreateComponentsAsync(CharacterDtoV3 character, bool cachedFromSheet)
    {
        const int lineLength = 28;

        var hpMpStr = new StringBuilder("```");
        hpMpStr.AppendLine(CreateLine(nameof(CharacterAttribute.Hp).ToUpper(),
            character.Attributes[CharacterAttribute.Hp].ToString()));

        if (character.ActiveClassJob.IsDiscipleOfHand())
        {
            hpMpStr.AppendLine(CreateLine(nameof(CharacterAttribute.Cp).ToUpper(),
                character.Attributes[CharacterAttribute.Cp].ToString()));
        }
        else if (character.ActiveClassJob.IsDiscipleOfLand())
        {
            hpMpStr.AppendLine(CreateLine(nameof(CharacterAttribute.Gp).ToUpper(),
                character.Attributes[CharacterAttribute.Gp].ToString()));
        }
        else
        {
            hpMpStr.AppendLine(CreateLine(nameof(CharacterAttribute.Mp).ToUpper(),
                character.Attributes[CharacterAttribute.Mp].ToString()));
        }

        hpMpStr.AppendLine("```");

        List<DiscordComponent> c =
        [
            character.ToSectionComponent(cachingService),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            new DiscordTextDisplayComponent(hpMpStr.ToString()),
            new DiscordSeparatorComponent(),
            new DiscordTextDisplayComponent(
                $"""
                 ### {cachingService.GetApplicationEmoji("attributes")} {Messages.Xiv.Attributes.Same}
                 ```
                 {CreateLine(CharacterAttribute.Strength.GetDisplayName(), character.Attributes[CharacterAttribute.Strength].ToString())}
                 {CreateLine(CharacterAttribute.Dexterity.GetDisplayName(), character.Attributes[CharacterAttribute.Dexterity].ToString())}
                 {CreateLine(CharacterAttribute.Vitality.GetDisplayName(), character.Attributes[CharacterAttribute.Vitality].ToString())}
                 {CreateLine(CharacterAttribute.Intelligence.GetDisplayName(), character.Attributes[CharacterAttribute.Intelligence].ToString())}
                 {CreateLine(CharacterAttribute.Mind.GetDisplayName(), character.Attributes[CharacterAttribute.Mind].ToString())}
                 ```
                 """),
            new DiscordSeparatorComponent(),
            new DiscordTextDisplayComponent(
                $"""
                 ### {cachingService.GetApplicationEmoji("offensiveProperties")} {Messages.Xiv.Attributes.OffensiveProperties}
                 ```
                 {CreateLine(CharacterAttribute.CriticalHitRate.GetDisplayName(), character.Attributes[CharacterAttribute.CriticalHitRate].ToString())}
                 {CreateLine(CharacterAttribute.Determination.GetDisplayName(), character.Attributes[CharacterAttribute.Determination].ToString())}
                 {CreateLine(CharacterAttribute.DirectHitRate.GetDisplayName(), character.Attributes[CharacterAttribute.DirectHitRate].ToString())}
                 ```
                 """),
            new DiscordSeparatorComponent(),
            new DiscordTextDisplayComponent(
                $"""
                 ### {cachingService.GetApplicationEmoji("defensiveProperties")} {Messages.Xiv.Attributes.DefensiveProperties}
                 ```
                 {CreateLine(CharacterAttribute.Defense.GetDisplayName(), character.Attributes[CharacterAttribute.Defense].ToString())}
                 {CreateLine(CharacterAttribute.MagicDefense.GetDisplayName(), character.Attributes[CharacterAttribute.MagicDefense].ToString())}
                 ```
                 """),
            new DiscordSeparatorComponent(),
            new DiscordTextDisplayComponent(
                $"""
                 ### {cachingService.GetApplicationEmoji("physicalProperties")} {Messages.Xiv.Attributes.PhysicalProperties}
                 ```
                 {CreateLine(CharacterAttribute.AttackPower.GetDisplayName(), character.Attributes[CharacterAttribute.AttackPower].ToString())}
                 {CreateLine(CharacterAttribute.SkillSpeed.GetDisplayName(), character.Attributes[CharacterAttribute.SkillSpeed].ToString())}
                 ```
                 """),
            new DiscordSeparatorComponent()
        ];

        if (character.ActiveClassJob.IsDiscipleOfHand())
        {
            c.Add(new DiscordTextDisplayComponent(
                $"""
                 ### {cachingService.GetApplicationEmoji("crafting")} {Messages.Xiv.Attributes.Crafting}
                 ```
                 {CreateLine(CharacterAttribute.Craftsmanship.GetDisplayName(), character.Attributes[CharacterAttribute.Craftsmanship].ToString())}
                 {CreateLine(CharacterAttribute.Control.GetDisplayName(), character.Attributes[CharacterAttribute.Control].ToString())}
                 ```
                 """));
        }
        else if (character.ActiveClassJob.IsDiscipleOfLand())
        {
            c.Add(new DiscordTextDisplayComponent(
                $"""
                 ### {cachingService.GetApplicationEmoji("gathering")} {Messages.Xiv.Attributes.Gathering}
                 ```
                 {CreateLine(CharacterAttribute.Gathering.GetDisplayName(), character.Attributes[CharacterAttribute.Gathering].ToString())}
                 {CreateLine(CharacterAttribute.Perception.GetDisplayName(), character.Attributes[CharacterAttribute.Perception].ToString())}
                 ```
                 """));
        }
        else
        {
            c.AddRange(new DiscordTextDisplayComponent(
                    $"""
                     ### {cachingService.GetApplicationEmoji("mentalProperties")} {Messages.Xiv.Attributes.MentalProperties}
                     ```
                     {CreateLine(CharacterAttribute.AttackMagicPotency.GetDisplayName(), character.Attributes[CharacterAttribute.AttackMagicPotency].ToString())}
                     {CreateLine(CharacterAttribute.HealingMagicPotency.GetDisplayName(), character.Attributes[CharacterAttribute.HealingMagicPotency].ToString())}
                     {CreateLine(CharacterAttribute.SpellSpeed.GetDisplayName(), character.Attributes[CharacterAttribute.SpellSpeed].ToString())}
                     ```
                     """),
                new DiscordSeparatorComponent(),
                new DiscordTextDisplayComponent(
                    $"""
                     ### {cachingService.GetApplicationEmoji("role")} {Messages.Xiv.Attributes.Role}
                     ```
                     {CreateLine(CharacterAttribute.Tenacity.GetDisplayName(), character.Attributes[CharacterAttribute.Tenacity].ToString())}
                     {CreateLine(CharacterAttribute.Piety.GetDisplayName(), character.Attributes[CharacterAttribute.Piety].ToString())}
                     {CreateLine(CharacterAttribute.SpellSpeed.GetDisplayName(), character.Attributes[CharacterAttribute.SpellSpeed].ToString())}
                     ```
                     """),
                new DiscordSeparatorComponent(),
                new DiscordTextDisplayComponent(
                    $"""
                     ### {cachingService.GetApplicationEmoji("gear")} {Messages.Xiv.Attributes.Gear}
                     ```
                     {CreateLine(Messages.Xiv.Attributes.AverageItemLevel, character.Gear.GetAvarageItemLevel().ToString())}
                     ```
                     """)
            );
        }

        if (character.LastUpdated is not null)
        {
            c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

            var lastUpdatedStr = $"-# Last updated {Formatter.Timestamp(character.LastUpdated.Value)}";

            var maxAgeCharacter = TimeSpan.FromMinutes(config.NetStone.MaxAgeCharacter);
            if (cachedFromSheet && DateTime.Now.Subtract(maxAgeCharacter) > character.LastUpdated)
            {
                c.Add(new DiscordSectionComponent(
                    new DiscordTextDisplayComponent(
                        $"""
                         -# {Messages.InteractionData.CachedFromSheet("Attributes", true)}
                         {lastUpdatedStr}
                         """),
                    await CreateCharacterAttributesButtonAsync(character)
                ));
            }
            else
            {
                c.Add(new DiscordTextDisplayComponent(lastUpdatedStr));
            }
        }

        if (character.FallbackUsed)
        {
            c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

            c.Add(new DiscordTextDisplayComponent(
                $"""
                 {Messages.Other.RefreshFailed}
                 -# {Messages.Other.RefreshFailedDescription}
                 -# {CreateFallbackMessage(character.FallbackReason)}
                 """
            ));
        }

        return c;

        string CreateLine(string key, string? value)
        {
            ArgumentNullException.ThrowIfNull(value);

            var padding = lineLength - key.Length - value.Length;

            if (padding < 1)
            {
                throw new InvalidOperationException("Padding is too small, must be at least 1");
            }

            var paddingStr = new string(' ', padding);

            return $"{key}{paddingStr}{value}";
        }
    }

    private async Task<DiscordButtonComponent> CreateCharacterAttributesButtonAsync(CharacterDtoV3 character)
    {
        var id = await interactionDataService.AddDataAsync(character.Id, ComponentIds.Button.CharacterAttributes);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, id, Messages.Buttons.CurrentAttributes);
    }

    private static string? CreateFallbackMessage(string? fallbackReason)
    {
        if (fallbackReason is null)
        {
            return null;
        }

        return fallbackReason.Equals(nameof(ParsingFailedException), StringComparison.OrdinalIgnoreCase)
            ? Messages.Other.ServiceUnavailableDescription
            : fallbackReason;
    }
}