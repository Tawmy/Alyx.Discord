using System.Net;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Records;
using Alyx.Discord.Core.Requests.Character.Sheet;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Api.Sdk;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Bot.Extensions;

internal static class BaseDiscordMessageBuilderExtensions
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
        ISender sender, IInteractionDataService interactionDataService, CachingService cachingService,
        string lodestoneId, bool forceRefresh, Func<BaseDiscordMessageBuilder<T>, Task> respondTask,
        CancellationToken cancellationToken = default)
        where T : BaseDiscordMessageBuilder<T>
    {
        builder.EnableV2Components();

        CharacterSheetResponse sheet;
        try
        {
            sheet = await sender.Send(new CharacterSheetRequest(lodestoneId, forceRefresh), cancellationToken);
        }
        catch (NetStoneException e)
        {
            switch (e.HttpStatusCode)
            {
                case HttpStatusCode.ServiceUnavailable:
                    builder.AddError(Messages.Other.ServiceUnavailableDescription,
                        Messages.Other.ServiceUnavailableTitle);
                    break;
                case HttpStatusCode.InternalServerError:
                    builder.AddError(Messages.Other.NetStoneApiServerErrorDescription,
                        Messages.Other.NetStoneApiServerErrorTitle);
                    break;
                default:
                    throw;
            }

            await respondTask(builder);
            return;
        }

        var timestamp = DateTime.UtcNow;
        var fileName = $"{timestamp:yyyy-MM-dd_HH-mm}_{lodestoneId}";
        await using var _ = await builder.AddImageAsync(sheet.Image, fileName, cancellationToken);

        builder.AddMediaGalleryComponent(new DiscordMediaGalleryItem($"attachment://{fileName}.webp"));

        if (CreateFallbackContainerIfApplicable(sheet.SheetMetadata) is { } fallbackContainer)
        {
            builder.AddContainerComponent(fallbackContainer);
        }

        List<DiscordButtonComponent> buttons =
        [
            await CreateMoreButtonAsync(interactionDataService, cachingService, sheet.MountsPublic, sheet.MinionsPublic,
                sheet.Character, sheet.ClassJobs, sheet.FreeCompany),
            CreateLodestoneLinkButton(lodestoneId),
            await CreateMetadataButtonAsync(interactionDataService, sheet.SheetMetadata)
        ];

        builder.AddActionRowComponent(buttons);

        await respondTask(builder);
    }

    public static async Task<MemoryStream> AddImageAsync<T>(this BaseDiscordMessageBuilder<T> builder, Image image,
        string fileName, CancellationToken cancellationToken = default) where T : BaseDiscordMessageBuilder<T>
    {
        var stream = new MemoryStream();
        await image.SaveAsync(stream, new WebpEncoder(), cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);
        var fileNameWithExtension = $"{fileName}.webp";
        builder.AddFile(fileNameWithExtension, stream);
        return stream;
    }

    #region CreateSheetAndSendFollowupAsync

    private static async Task<DiscordButtonComponent> CreateMoreButtonAsync(
        IInteractionDataService interactionDataService, CachingService cachingService, bool mountsPublic,
        bool minionsPublic, CharacterDto character, CharacterClassJobOuterDto classJobs, FreeCompanyDto? freeCompany)
    {
        var sheetCache = new SheetCache
        {
            MountsPublic = mountsPublic,
            MinionsPublic = minionsPublic,
            Character = character,
            ClassJobs = classJobs,
            FreeCompany = freeCompany
        };

        var componentId = await interactionDataService.AddDataAsync(sheetCache, ComponentIds.Button.CharacterSheetMore);
        var emoji = cachingService.GetApplicationEmoji("sheetMore");

        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId,
            Messages.Buttons.CharacterSheetMore, emoji: new DiscordComponentEmoji(emoji));
    }

    private static DiscordLinkButtonComponent CreateLodestoneLinkButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.OpenLodestoneProfile);
    }

    private static async Task<DiscordButtonComponent> CreateMetadataButtonAsync(
        IInteractionDataService interactionDataService, IEnumerable<SheetMetadata> metadata)
    {
        var componentId =
            await interactionDataService.AddDataAsync(metadata, ComponentIds.Button.CharacterSheetMetadata);
        return new DiscordButtonComponent(DiscordButtonStyle.Secondary, componentId,
            Messages.Buttons.CharacterSheetMetadata);
    }

    private static DiscordContainerComponent? CreateFallbackContainerIfApplicable(IEnumerable<SheetMetadata> metadata)
    {
        if (metadata.All(x => !x.FallbackUsed))
        {
            // no fallback used, do not create embed
            return null;
        }

        return new DiscordContainerComponent(
        [
            new DiscordTextDisplayComponent(
                """
                Updating some data from the Lodestone failed. Cached data is shown instead.
                Sheet metadata will show which data failed to update.
                """)
        ], color: DiscordColor.Red);
    }

    #endregion
}