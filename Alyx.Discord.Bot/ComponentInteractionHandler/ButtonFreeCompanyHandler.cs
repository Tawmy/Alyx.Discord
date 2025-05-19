using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Extensions;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonFreeCompanyHandler(
    IInteractionDataService interactionDataService,
    HttpClient httpClient,
    [FromKeyedServices(FreeCompanyService.Key)]
    IDiscordContainerService<FreeCompanyDtoV3> fcService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        string lodestoneId;
        try
        {
            lodestoneId = await interactionDataService.GetDataAsync<string>(dataId);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db
            await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                .AddError(Messages.InteractionData.NotPersisted));
            return;
        }

        var (container, freeCompany) = await fcService.RetrieveDataAndCreateContainerAsync(lodestoneId,
            cancellationToken: cancellationToken);

        var builder = new DiscordWebhookBuilder().EnableV2Components().AddContainerComponent(container);

        var crest = await freeCompany.CrestLayers.DownloadCrestAsync(httpClient);
        await using var _ = await builder.AddImageAsync(crest, Messages.FileNames.Crest, cancellationToken);

        await args.Interaction.EditOriginalResponseAsync(builder);
    }
}