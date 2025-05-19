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

internal class CharacterGearService(
    ISender sender,
    AlyxConfiguration config,
    CachingService cachingService,
    IInteractionDataService interactionDataService)
    : IDiscordContainerService<CharacterDto>
{
    public const string Key = "gear";

    public async Task<DiscordContainerComponent> CreateContainerAsync(CharacterDto character,
        CancellationToken cancellationToken = default)
    {
        return new DiscordContainerComponent(await CreateComponentsAsync(character, true));
    }

    public async Task<DiscordContainerComponent> CreateContainerAsync(string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var (container, _) = await RetrieveDataAndCreateContainerAsync(lodestoneId, forceRefresh, cancellationToken);
        return container;
    }

    public async Task<(DiscordContainerComponent, CharacterDto)> RetrieveDataAndCreateContainerAsync(
        string lodestoneId, bool forceRefresh = false,
        CancellationToken cancellationToken = default)
    {
        var maxAge = forceRefresh ? 0 : config.NetStone.MaxAgeCharacter;
        var character = await sender.Send(new CharacterGetCharacterRequest(lodestoneId, maxAge), cancellationToken);
        var container = new DiscordContainerComponent(await CreateComponentsAsync(character, false));
        return (container, character);
    }

    private async Task<List<DiscordComponent>> CreateComponentsAsync(CharacterDto character, bool cachedFromSheet)
    {
        List<DiscordComponent> c =
        [
            character.ToSectionComponent(cachingService, true),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            new DiscordTextDisplayComponent($"### Avg. Item Level: {character.Gear.GetAvarageItemLevel()}"),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large)
        ];

        string?[] gear1 =
        [
            CreateGearString(character.Gear, GearSlot.MainHand, "Main Hand"),
            CreateGearString(character.Gear, GearSlot.OffHand, "Off Hand")
        ];

        c.Add(CreateGearTextDisplayComponent(gear1));
        c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

        string?[] gear2 =
        [
            CreateGearString(character.Gear, GearSlot.Head, "head"),
            CreateGearString(character.Gear, GearSlot.Body, "body"),
            CreateGearString(character.Gear, GearSlot.Hands, "hands"),
            CreateGearString(character.Gear, GearSlot.Legs, "legs"),
            CreateGearString(character.Gear, GearSlot.Feet, "feet")
        ];

        c.Add(CreateGearTextDisplayComponent(gear2));
        c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

        string?[] gear3 =
        [
            CreateGearString(character.Gear, GearSlot.Earrings, "earrings"),
            CreateGearString(character.Gear, GearSlot.Necklace, "necklace"),
            CreateGearString(character.Gear, GearSlot.Bracelets, "bracelet"),
            CreateGearString(character.Gear, GearSlot.Ring1, "ring"),
            CreateGearString(character.Gear, GearSlot.Ring2, "ring")
        ];

        c.Add(CreateGearTextDisplayComponent(gear3));

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
                         -# {Messages.InteractionData.CachedFromSheet("Gear")}
                         {lastUpdatedStr}
                         """),
                    await CreateCharacterGearButtonAsync(character)
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
    }

    private string? CreateGearString(ICollection<CharacterGearDto> gearDtos,
        GearSlot slot, string emoji)
    {
        if (gearDtos.FirstOrDefault(x => x.Slot == slot) is not { } gear)
        {
            return null;
        }

        var sb = new StringBuilder();

        if (cachingService.TryGetApplicationEmoji(emoji, out var discordEmoji) && discordEmoji is not null)
        {
            sb.Append($"{Formatter.Emoji(discordEmoji)} ");
        }

        sb.Append("**");
        sb.Append(gear.ItemDatabaseLink is not null
            ? Formatter.MaskedUrl(gear.StrippedItemName ?? gear.ItemName, new Uri(gear.ItemDatabaseLink))
            : gear.StrippedItemName ?? gear.ItemName);
        sb.Append("**");

        sb.AppendLine($" • {gear.ItemLevel}");

        if (!string.IsNullOrEmpty(gear.GlamourName))
        {
            var pfx = $"{Formatter.Emoji(cachingService.GetApplicationEmoji("glamour"))} ";
            sb.AppendLine(gear.GlamourDatabaseLink is not null
                ? $"{pfx}{Formatter.MaskedUrl(gear.GlamourName, new Uri(gear.GlamourDatabaseLink))}"
                : $"{pfx}{gear.GlamourName}");
        }

        return sb.ToString();
    }

    private static DiscordTextDisplayComponent CreateGearTextDisplayComponent(IEnumerable<string?> gears)
    {
        var gearStr = string.Join('\n', gears.Where(x => x is not null));
        return new DiscordTextDisplayComponent(gearStr);
    }

    private async Task<DiscordButtonComponent> CreateCharacterGearButtonAsync(CharacterDto character)
    {
        var id = await interactionDataService.AddDataAsync(character.Id, ComponentIds.Button.CharacterGear);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, id, Messages.Buttons.CurrentGear);
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