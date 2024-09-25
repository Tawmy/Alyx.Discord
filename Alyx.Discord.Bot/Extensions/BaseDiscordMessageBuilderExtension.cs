using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Claim;
using DSharpPlus.Entities;

namespace Alyx.Discord.Bot.Extensions;

internal static class BaseDiscordMessageBuilderExtension
{
    public static BaseDiscordMessageBuilder<T> AddClaimResponse<T>(this BaseDiscordMessageBuilder<T> builder,
        CharacterClaimRequestResponse claimRequestResponse, IDataPersistenceService dataPersistenceService,
        string lodestoneId) where T : BaseDiscordMessageBuilder<T>
    {
        var buttonLodestone = CreateOpenLodestoneButton();
        var buttonConfirm = CreateClaimConfirmButton(dataPersistenceService, lodestoneId);
        
        switch (claimRequestResponse.Status)
        {
            case CharacterClaimRequestStatus.AlreadyClaimedByUser:
                builder.WithContent("You've already claimed this character.");
                break;
            case CharacterClaimRequestStatus.AlreadyClaimedByDifferentUser:
                builder.WithContent("This character has already been claimed by someone else.");
                break;
            case CharacterClaimRequestStatus.ClaimAlreadyExistsForThisCharacter:
                CreateClaimInstructions(builder, claimRequestResponse.Code!);
                builder.AddComponents(buttonLodestone, buttonConfirm);
                break;
            case CharacterClaimRequestStatus.NewClaimCreated:
                CreateClaimInstructions(builder, claimRequestResponse.Code!);
                builder.AddComponents(buttonLodestone, buttonConfirm);
                break;
            case CharacterClaimRequestStatus.ClaimConfirmed:
                builder.WithContent("Claim confirmed! You can now request your character sheet using /character me.");
                break;
            case CharacterClaimRequestStatus.UserAlreadyHasMainCharacter:
                builder.WithContent("You've already claimed a different character.");
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(claimRequestResponse), claimRequestResponse, null);
        }

        return builder;
    }

    private static void CreateClaimInstructions<T>(BaseDiscordMessageBuilder<T> builder, string code)
        where T : BaseDiscordMessageBuilder<T>
    {
        // TODO more detailed instructions
        builder.WithContent($"Please put code `{code}` into your Lodestone profile description.");
    }

    private static DiscordButtonComponent CreateClaimConfirmButton(IDataPersistenceService dataPersistenceService, string lodestoneId)
    {
        var componentId = dataPersistenceService.AddData(lodestoneId, ComponentIds.Button.ConfirmClaim);
        return new DiscordButtonComponent(DiscordButtonStyle.Primary, componentId, "Confirm Claim");
    }

    private static DiscordLinkButtonComponent CreateOpenLodestoneButton() 
    {
        const string url = "https://eu.finalfantasyxiv.com/lodestone/my/setting/profile/";
        return new DiscordLinkButtonComponent(url, "Edit Lodestone Profile");
    }
}