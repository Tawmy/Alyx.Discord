using System.Net;
using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Records;
using Alyx.Discord.Core.Requests.Character.Sheet;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Api.Sdk;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.Services;

internal class CharacterSheetService(
    ISender sender,
    IInteractionDataService interactionDataService,
    CachingService cachingService)
{
    public async Task CreateSheetAndSendFollowupAsync<T>(BaseDiscordMessageBuilder<T> builder, string lodestoneId,
        bool forceRefresh, Func<BaseDiscordMessageBuilder<T>, Task> respondTask,
        CancellationToken cancellationToken = default) where T : BaseDiscordMessageBuilder<T>
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
}