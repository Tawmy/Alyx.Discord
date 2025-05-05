using System.Net;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Sheet;
using Alyx.Discord.Core.Structs;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using Refit;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Bot.Extensions;

internal static class BaseDiscordMessageBuilderExtension
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

    public static async Task CreateSheetAndSendFollowupAsync<T>(this BaseDiscordMessageBuilder<T> builder,
        ISender sender, IInteractionDataService interactionDataService,
        string lodestoneId, bool forceRefresh, Func<BaseDiscordMessageBuilder<T>, Task> followupTask,
        CancellationToken cancellationToken = default)
        where T : BaseDiscordMessageBuilder<T>
    {
        CharacterSheetResponse sheet;
        try
        {
            sheet = await sender.Send(new CharacterSheetRequest(lodestoneId, forceRefresh), cancellationToken);
        }
        catch (ValidationApiException e)
        {
            if (e.StatusCode is not HttpStatusCode.ServiceUnavailable)
            {
                throw;
            }

            builder.AddError(Messages.Other.ServiceUnavailableDescription, Messages.Other.ServiceUnavailableTitle);

            await followupTask(builder);
            return;
        }

        await using var stream = new MemoryStream();
        await sheet.Image.SaveAsync(stream, new WebpEncoder(), cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var timestamp = DateTime.UtcNow;
        var fileName = $"{timestamp:yyyy-MM-dd HH-mm} {lodestoneId}.webp";

        List<DiscordButtonComponent> buttons =
        [
            CreateLodestoneLinkButton(lodestoneId),
            await CreateGearButtonAsync(interactionDataService, sheet.Character)
        ];

        if (sheet.MountsPublic)
        {
            buttons.Add(CreateLodestoneMountsButton(lodestoneId));
        }

        if (sheet.MinionsPublic)
        {
            buttons.Add(CreateLodestoneMinionsButton(lodestoneId));
        }

        buttons.Add(await CreateMetadataButtonAsync(interactionDataService, sheet.SheetMetadata));

        builder.AddFile(fileName, stream, true).AddActionRowComponent(buttons);

        if (CreateFallbackEmbedIfApplicable(sheet.SheetMetadata) is { } embed)
        {
            builder.AddEmbed(embed);
        }

        await followupTask(builder);
    }

    #region CreateSheetAndSendFollowupAsync

    private static DiscordLinkButtonComponent CreateLodestoneLinkButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.OpenLodestoneProfile);
    }

    private static async Task<DiscordButtonComponent> CreateGearButtonAsync(
        IInteractionDataService interactionDataService, CharacterDtoV3 character)
    {
        var componentId =
            await interactionDataService.AddDataAsync(character, ComponentIds.Button.CharacterSheetGear);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId, Messages.Buttons.Gear);
    }

    private static async Task<DiscordButtonComponent> CreateMetadataButtonAsync(
        IInteractionDataService interactionDataService, IEnumerable<SheetMetadata> metadata)
    {
        var componentId =
            await interactionDataService.AddDataAsync(metadata, ComponentIds.Button.CharacterSheetMetadata);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId,
            Messages.Buttons.CharacterSheetMetadata);
    }

    private static DiscordLinkButtonComponent CreateLodestoneMountsButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}/mount";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.Mounts);
    }

    private static DiscordLinkButtonComponent CreateLodestoneMinionsButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}/minion";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.Minions);
    }

    private static DiscordEmbed? CreateFallbackEmbedIfApplicable(IEnumerable<SheetMetadata> metadata)
    {
        if (metadata.All(x => !x.FallbackUsed))
        {
            // no fallback used, do not create embed
            return null;
        }

        return new DiscordEmbedBuilder
        {
            Color = DiscordColor.Red,
            Description = """
                          Updating some data from the Lodestone failed. Cached data is shown instead.
                          Sheet metadata will show which data failed to update.
                          """
        }.Build();
    }

    #endregion
}