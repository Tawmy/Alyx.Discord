using System.Net;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Claim;
using Alyx.Discord.Core.Requests.Character.Sheet;
using Alyx.Discord.Core.Structs;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using MediatR;
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

    public static async Task AddClaimResponseAsync<T>(this BaseDiscordMessageBuilder<T> builder,
        CharacterClaimRequestResponse claimRequestResponse,
        IInteractionDataService interactionDataService,
        DiscordEmbedService embedService,
        string lodestoneId,
        IReadOnlyDictionary<ulong, Command> commands)
        where T : BaseDiscordMessageBuilder<T>
    {
        var buttonLodestone = CreateOpenLodestoneButton();
        var buttonConfirm = await CreateClaimConfirmButtonAsync(interactionDataService, lodestoneId);

        switch (claimRequestResponse.Status)
        {
            case CharacterClaimRequestStatus.AlreadyClaimedByUser:
                builder.AddEmbed(embedService.CreateError(
                    Messages.Commands.Character.Claim.AlreadyClaimed(commands, "character unclaim")));
                break;
            case CharacterClaimRequestStatus.AlreadyClaimedByDifferentUser:
                builder.AddEmbed(embedService.CreateError(Messages.Commands.Character.Claim.ClaimedBySomeoneElse));
                break;
            case CharacterClaimRequestStatus.ClaimAlreadyExistsForThisCharacter:
                builder.AddEmbed(CreateClaimInstructionsEmbed(embedService, claimRequestResponse.Code!, true));
                builder.AddActionRowComponent(buttonLodestone, buttonConfirm);
                break;
            case CharacterClaimRequestStatus.NewClaimCreated:
                builder.AddEmbed(CreateClaimInstructionsEmbed(embedService, claimRequestResponse.Code!, false));
                builder.AddActionRowComponent(buttonLodestone, buttonConfirm);
                break;
            case CharacterClaimRequestStatus.ClaimConfirmed:
                builder.AddEmbed(embedService.Create(
                    Messages.Commands.Character.Claim.ConfirmedDescription(commands, "character me"),
                    Messages.Commands.Character.Claim.ConfirmedTitle));
                break;
            case CharacterClaimRequestStatus.UserAlreadyHasMainCharacter:
                builder.AddEmbed(embedService.CreateError(
                    Messages.Commands.Character.Claim.AlreadyClaimedDifferent(commands, "character unclaim")));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(claimRequestResponse), claimRequestResponse, null);
        }
    }

    public static async Task CreateSheetAndSendFollowupAsync<T>(this BaseDiscordMessageBuilder<T> builder,
        ISender sender, IInteractionDataService interactionDataService, DiscordEmbedService embedService,
        string lodestoneId, bool forceRefresh, Func<BaseDiscordMessageBuilder<T>, Task> followupTask,
        CancellationToken cancellationToken = default)
        where T : BaseDiscordMessageBuilder<T>
    {
        CharacterSheet sheet;
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

            var errorEmbed = embedService.CreateError(Messages.Other.ServiceUnavailableDescription,
                Messages.Other.ServiceUnavailableTitle);
            builder.AddEmbed(errorEmbed);

            await followupTask(builder);
            return;
        }

        await using var stream = new MemoryStream();
        await sheet.Image.SaveAsync(stream, new WebpEncoder(), cancellationToken);
        stream.Seek(0, SeekOrigin.Begin);

        var timestamp = DateTime.UtcNow;
        var fileName = $"{timestamp:yyyy-MM-dd HH-mm} {lodestoneId}.webp";

        IList<DiscordButtonComponent> buttons = [CreateLodestoneLinkButton(lodestoneId)];

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

    #region AddClaimResponse

    /// <summary>
    ///     Create instructions for claiming character.
    /// </summary>
    /// <param name="embedService">Instance of embed service.</param>
    /// <param name="code">Code that user needs needs to add to their Lodestone profile.</param>
    /// <param name="isRetry">If this is not the first time user sees message, add additional note.</param>
    /// <returns>Discord embed with instructions.</returns>
    private static DiscordEmbed CreateClaimInstructionsEmbed(DiscordEmbedService embedService, string code,
        bool isRetry)
    {
        var description = Messages.Commands.Character.Claim.ClaimInstructionsDescription(code);

        if (isRetry)
        {
            description = $"**{Messages.Commands.Character.Claim.CodeNotFound}**\n\n{description}";
        }

        return embedService.Create(description, Messages.Commands.Character.Claim.ClaimInstructionsTitle);
    }

    private static async Task<DiscordButtonComponent> CreateClaimConfirmButtonAsync(
        IInteractionDataService interactionDataService, string lodestoneId)
    {
        var componentId = await interactionDataService.AddDataAsync(lodestoneId, ComponentIds.Button.ConfirmClaim);
        return new DiscordButtonComponent(DiscordButtonStyle.Primary, componentId, Messages.Buttons.ValidateCode);
    }

    private static DiscordLinkButtonComponent CreateOpenLodestoneButton()
    {
        const string url = "https://eu.finalfantasyxiv.com/lodestone/my/setting/profile/";
        return new DiscordLinkButtonComponent(url, Messages.Buttons.EditLodestoneProfile);
    }

    #endregion

    #region CreateSheetAndSendFollowupAsync

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