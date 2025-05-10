using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class SelectFreeCompanyHandler(
    ISender sender,
    IInteractionDataService interactionDataService,
    [FromKeyedServices("fc")] IDiscordContainerService<FreeCompanyDtoV3> fcService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();

        var container = await fcService.CreateContainerAsync(selectedLodestoneId, cancellationToken: cancellationToken);
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}