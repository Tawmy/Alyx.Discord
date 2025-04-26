using System.Text;
using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Claim;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Services;

internal class CharacterClaimService(
    ISender sender,
    IInteractionDataService interactionDataService,
    CachingService cachingService)
{
    public async Task AddClaimResponseAsync<T>(BaseDiscordMessageBuilder<T> builder,
        CharacterClaimRequestResponse claimRequestResponse,
        string lodestoneId,
        IReadOnlyDictionary<ulong, Command> commands)
        where T : BaseDiscordMessageBuilder<T>
    {
        builder.EnableV2Components();

        switch (claimRequestResponse.Status)
        {
            case CharacterClaimRequestStatus.AlreadyClaimedByUser:
                builder.AddError(
                    Messages.Commands.Character.Claim.AlreadyClaimed(commands, "character unclaim"));
                break;
            case CharacterClaimRequestStatus.AlreadyClaimedByDifferentUser:
                builder.AddError(Messages.Commands.Character.Claim.ClaimedBySomeoneElse);
                break;
            case CharacterClaimRequestStatus.ClaimAlreadyExistsForThisCharacter:
                await AddClaimInstructionsAsync(builder, lodestoneId, claimRequestResponse.Code!, true);
                break;
            case CharacterClaimRequestStatus.NewClaimCreated:
                await AddClaimInstructionsAsync(builder, lodestoneId, claimRequestResponse.Code!);
                break;
            case CharacterClaimRequestStatus.ClaimConfirmed:
                builder.AddContainerComponent(new DiscordContainerComponent([
                    new DiscordTextDisplayComponent($"# {Messages.Commands.Character.Claim.ConfirmedTitle}"),
                    new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
                    new DiscordTextDisplayComponent(
                        Messages.Commands.Character.Claim.ConfirmedDescription(commands, "character me"))
                ]));
                break;
            case CharacterClaimRequestStatus.UserAlreadyHasMainCharacter:
                builder.AddError(
                    Messages.Commands.Character.Claim.AlreadyClaimedDifferent(commands, "character unclaim"));
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(claimRequestResponse), claimRequestResponse, null);
        }
    }

    private async Task AddClaimInstructionsAsync<T>(BaseDiscordMessageBuilder<T> builder, string lodestoneId,
        string code, bool isRetry = false)
        where T : BaseDiscordMessageBuilder<T>
    {
        var character = await sender.Send(new CharacterGetCharacterRequest(lodestoneId));

        List<DiscordComponent> components =
        [
            new DiscordTextDisplayComponent($"# {Messages.Commands.Character.Claim.ClaimInstructionsTitle}"),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            character.ToSectionComponent(cachingService),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large)
        ];

        var step1Subtext = new StringBuilder();

        if (isRetry)
        {
            step1Subtext.AppendLine($"**{Messages.Commands.Character.Claim.CodeNotFound}**");
            step1Subtext.AppendLine();
        }

        step1Subtext.AppendLine($"-# {Messages.Commands.Character.Claim.ClaimInstructionsPart1Subtext}");

        components.AddRange(
            new DiscordTextDisplayComponent($"### {Messages.Commands.Character.Claim.ClaimInstructionsDescription}"),
            new DiscordSeparatorComponent(spacing: DiscordSeparatorSpacing.Small),
            new DiscordSectionComponent(
                new DiscordTextDisplayComponent($"1. {Messages.Commands.Character.Claim.ClaimInstructionsPart1(code)}"),
                CreateOpenLodestoneButton()
            ),
            new DiscordTextDisplayComponent(step1Subtext.ToString()),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            new DiscordSectionComponent(
                new DiscordTextDisplayComponent($"2. {Messages.Commands.Character.Claim.ClaimInstructionsPart2}"),
                await CreateClaimConfirmButtonAsync(interactionDataService, lodestoneId)
            )
        );

        builder.AddContainerComponent(new DiscordContainerComponent(components));
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
}