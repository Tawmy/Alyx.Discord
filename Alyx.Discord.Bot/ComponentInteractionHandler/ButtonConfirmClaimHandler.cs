using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Claim.CharacterClaimRequest;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonConfirmClaimHandler(
    ISender sender,
    IDataPersistenceService dataPersistenceService,
    DiscordEmbedService embedService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        if (!dataPersistenceService.TryGetData<string>(dataId, out var lodestoneId))
        {
            // TODO potentially move this into dataPersistenceService or even discordEmbedService?
            var embed = embedService.CreateError(Messages.DataPersistence.NotPersisted);
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
            return;
        }

        await args.Interaction.DeferAsync(true);

        var characterClaimRequestResponse = await sender.Send(new CoreRequest(args.User.Id, lodestoneId));

        var builder = new DiscordFollowupMessageBuilder();
        builder.AddClaimResponse(characterClaimRequestResponse, dataPersistenceService, embedService, lodestoneId);

        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}