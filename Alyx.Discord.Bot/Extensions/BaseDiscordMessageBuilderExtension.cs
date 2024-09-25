using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Claim;
using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Extensions;

internal static class BaseDiscordMessageBuilderExtension
{
    public static BaseDiscordMessageBuilder<T> AddClaimResponse<T>(this BaseDiscordMessageBuilder<T> builder,
        CharacterClaimRequestResponse claimRequestResponse,
        IDataPersistenceService dataPersistenceService,
        DiscordEmbedService embedService,
        string lodestoneId)
        where T : BaseDiscordMessageBuilder<T>
    {
        var buttonLodestone = CreateOpenLodestoneButton();
        var buttonConfirm = CreateClaimConfirmButton(dataPersistenceService, lodestoneId);

        switch (claimRequestResponse.Status)
        {
            case CharacterClaimRequestStatus.AlreadyClaimedByUser:
                builder.AddEmbed(embedService.CreateError(Messages.Commands.Character.Claim.AlreadyClaimed));
                break;
            case CharacterClaimRequestStatus.AlreadyClaimedByDifferentUser:
                builder.AddEmbed(embedService.CreateError(Messages.Commands.Character.Claim.ClaimedBySomeoneElse));
                break;
            case CharacterClaimRequestStatus.ClaimAlreadyExistsForThisCharacter:
                builder.AddEmbed(CreateClaimInstructionsEmbed(embedService, claimRequestResponse.Code!));
                builder.AddComponents(buttonLodestone, buttonConfirm);
                break;
            case CharacterClaimRequestStatus.NewClaimCreated:
                builder.AddEmbed(CreateClaimInstructionsEmbed(embedService, claimRequestResponse.Code!));
                builder.AddComponents(buttonLodestone, buttonConfirm);
                break;
            case CharacterClaimRequestStatus.ClaimConfirmed:
                builder.AddEmbed(embedService.Create(Messages.Commands.Character.Claim.ConfirmedDescription,
                    Messages.Commands.Character.Claim.ConfirmedTitle));
                break;
            case CharacterClaimRequestStatus.UserAlreadyHasMainCharacter:
                builder.AddEmbed(embedService.CreateError(Messages.Commands.Character.Claim.AlreadyClaimedDifferent));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(claimRequestResponse), claimRequestResponse, null);
        }

        return builder;
    }

    private static DiscordEmbed CreateClaimInstructionsEmbed(DiscordEmbedService embedService, string code)
    {
        var description = Messages.Commands.Character.Claim.ClaimInstructionsDescription(code);
        return embedService.Create(description, Messages.Commands.Character.Claim.ClaimInstructionsTitle);
    }

    private static DiscordButtonComponent CreateClaimConfirmButton(IDataPersistenceService dataPersistenceService,
        string lodestoneId)
    {
        var componentId = dataPersistenceService.AddData(lodestoneId, ComponentIds.Button.ConfirmClaim);
        return new DiscordButtonComponent(DiscordButtonStyle.Primary, componentId, "Validate Code");
    }

    private static DiscordLinkButtonComponent CreateOpenLodestoneButton()
    {
        const string url = "https://eu.finalfantasyxiv.com/lodestone/my/setting/profile/";
        return new DiscordLinkButtonComponent(url, "Edit Lodestone Profile");
    }
}