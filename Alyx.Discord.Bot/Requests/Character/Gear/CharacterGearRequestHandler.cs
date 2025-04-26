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
            CreateGearString(character.Gear, GearSlot.Head,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("head"))} Head"),
            CreateGearString(character.Gear, GearSlot.Body,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("body"))} Body"),
            CreateGearString(character.Gear, GearSlot.Hands,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("hands"))} Hands"),
            CreateGearString(character.Gear, GearSlot.Legs,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("legs"))} Legs"),
            CreateGearString(character.Gear, GearSlot.Feet,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("feet"))} Feet")
        ];

        c.Add(CreateGearTextDisplayComponent(gear2));
        c.Add(new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large));

        string?[] gear3 =
        [
            CreateGearString(character.Gear, GearSlot.Earrings,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("earrings"))} Earrings"),
            CreateGearString(character.Gear, GearSlot.Necklace,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("necklace"))} Necklace"),
            CreateGearString(character.Gear, GearSlot.Bracelets,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("bracelet"))} Bracelets"),
            CreateGearString(character.Gear, GearSlot.Ring1,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("ring"))} Ring 1"),
            CreateGearString(character.Gear, GearSlot.Ring2,
                $"{Formatter.Emoji(cachingService.GetApplicationEmoji("ring"))} Ring 2")
        ];

        c.Add(CreateGearTextDisplayComponent(gear3));

        return c;
    }

    private static string? CreateGearString(ICollection<CharacterGearDto> gearDtos,
        GearSlot slot, string displayString)
    {
        if (gearDtos.FirstOrDefault(x => x.Slot == slot) is not { } gear)
        {
            return null;
        }

        var sb = new StringBuilder();
        sb.AppendLine($"**{displayString}**");

        if (!string.IsNullOrEmpty(gear.GlamourName))
        {
            var pfx = "Glamour: ";
            sb.AppendLine(gear.GlamourDatabaseLink is not null
                ? $"{pfx}{Formatter.MaskedUrl(gear.GlamourName, new Uri(gear.GlamourDatabaseLink))}"
                : $"{pfx}{gear.GlamourName}");
        }

        sb.Append(gear.ItemDatabaseLink is not null
            ? Formatter.MaskedUrl(gear.StrippedItemName ?? gear.ItemName, new Uri(gear.ItemDatabaseLink))
            : gear.StrippedItemName ?? gear.ItemName);

        sb.AppendLine($" • {gear.ItemLevel}");

        return sb.ToString();
    }

    private static DiscordTextDisplayComponent CreateGearTextDisplayComponent(IEnumerable<string?> gears)
    {
        var gearStr = string.Join('\n', gears.Where(x => x is not null));
        return new DiscordTextDisplayComponent(gearStr);
    }
}