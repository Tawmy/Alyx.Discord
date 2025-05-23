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

internal class ButtonCharacterSheetFreeCompanyHandler(
    IInteractionDataService interactionDataService,
    HttpClient httpClient,
    [FromKeyedServices(FreeCompanyService.Key)]
    IDiscordContainerService<FreeCompanyDto> fcService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        FreeCompanyDto? freeCompany;
        try
        {
            freeCompany = await interactionDataService.GetDataAsync<FreeCompanyDto>(dataId);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db
            await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                .AddError(Messages.InteractionData.NotPersisted));
            return;
        }

        var container = await fcService.CreateContainerAsync(freeCompany, cancellationToken);
        var builder = new DiscordWebhookBuilder().EnableV2Components().AddContainerComponent(container);

        var crest = await freeCompany.CrestLayers.DownloadCrestAsync(httpClient);
        await using var _ = await builder.AddImageAsync(crest, Messages.FileNames.Crest, cancellationToken);

        await args.Interaction.EditOriginalResponseAsync(builder);
    }
}