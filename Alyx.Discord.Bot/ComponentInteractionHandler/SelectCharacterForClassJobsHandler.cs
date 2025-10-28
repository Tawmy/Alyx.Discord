using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.Services.CharacterJobs;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class SelectCharacterForClassJobsHandler(
    IInteractionDataService interactionDataService,
    [FromKeyedServices(CharacterClassJobsService.Key)]
    IDiscordContainerServiceCustom<(CharacterDto, CharacterClassJobOuterDto), Role> classJobsService)
    : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();
        var role = await interactionDataService.GetDataAsync<Role>(dataId, cancellationToken);

        var container = await classJobsService.CreateContainerAsync(role, selectedLodestoneId,
            cancellationToken: cancellationToken);
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}