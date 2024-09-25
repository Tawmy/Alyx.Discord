using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Claim.CharacterClaimRequest;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonConfirmClaimHandler(ISender sender, IDataPersistenceService dataPersistenceService)
    : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        var lodestoneId = dataPersistenceService.GetData<string>(dataId);
        var characterClaimRequestResponse = await sender.Send(new CoreRequest(args.User.Id, lodestoneId));

        var builder = new DiscordFollowupMessageBuilder();
        builder.AddClaimResponse(characterClaimRequestResponse, dataPersistenceService, lodestoneId);

        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}