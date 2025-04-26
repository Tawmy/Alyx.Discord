using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Claim.CharacterClaimRequest;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonConfirmClaimHandler(
    ISender sender,
    IInteractionDataService interactionDataService,
    CharacterClaimService claimService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId, IReadOnlyDictionary<ulong, Command> commands)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        string? lodestoneId;
        try
        {
            lodestoneId = await interactionDataService.GetDataAsync<string>(dataId);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db 
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddError(Messages.InteractionData.NotPersisted).AsEphemeral());
            return;
        }

        var characterClaimRequestResponse = await sender.Send(new CoreRequest(args.User.Id, lodestoneId));

        var builder = new DiscordFollowupMessageBuilder();
        await claimService.AddClaimResponseAsync(builder, characterClaimRequestResponse, lodestoneId, commands);

        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}