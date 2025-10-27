using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.Services.CharacterJobs;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonCharacterSheetClassJobsHandler(
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

        ClassJobInteractionData? interactionData;
        try
        {
            interactionData = await interactionDataService.GetDataAsync<ClassJobInteractionData>(dataId);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db 
            await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                .AddError(Messages.InteractionData.NotPersisted));
            return;
        }

        var container = await classJobsService.CreateContainerAsync(interactionData.Role,
            (interactionData.Character, interactionData.ClassJobs), cancellationToken);
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}

internal record ClassJobInteractionData
{
    public required Role Role { get; init; }
    public required CharacterDto Character { get; init; }
    public required CharacterClassJobOuterDto ClassJobs { get; init; }
}