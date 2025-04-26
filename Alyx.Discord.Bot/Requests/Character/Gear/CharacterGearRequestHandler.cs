using System.Text;
using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Enums;
using NetStone.Common.Exceptions;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot.Requests.Character.Gear;

internal class CharacterGearRequestHandler(ISender sender, AlyxConfiguration config, CachingService cachingService)
    : IRequestHandler<CharacterGearRequest>
{
    public async Task Handle(CharacterGearRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        ICollection<CharacterSearchPageResultDto> searchDtos;
        DiscordInteractionResponseBuilder builder;
        try
        {
            searchDtos = await sender.Send(new CharacterSearchRequest(request.Name, request.World), cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.Character.Get.CharacterNotFound(request.Name, request.World);
            builder = new DiscordInteractionResponseBuilder().AddError(description);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        if (request.IsPrivate && searchDtos.Count > 1)
        {
            // TODO show select menu
            return;
        }

        var first = searchDtos.FirstOrDefault(x =>
            x.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase)) ?? searchDtos.First();

        var character = await sender.Send(new CharacterGetCharacterRequest(
            first.Id, config.NetStone.MaxAgeCharacter), cancellationToken);

        await request.Ctx.FollowupAsync(new DiscordFollowupMessageBuilder().EnableV2Components()
            .AddContainerComponent(new DiscordContainerComponent(CreateComponents(character))));
    }

    private List<DiscordComponent> CreateComponents(CharacterDtoV3 character)
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
}