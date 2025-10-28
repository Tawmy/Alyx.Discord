using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonCharacterGearHandler(
    IInteractionDataService interactionDataService,
    [FromKeyedServices(CharacterGearService.Key)]
    IDiscordContainerService<CharacterDto> gearService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        string lodestoneId;
        try
        {
            lodestoneId = await interactionDataService.GetDataAsync<string>(dataId, cancellationToken);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db 
            await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                .AddError(Messages.InteractionData.NotPersisted));
            return;
        }

        var container = await gearService.CreateContainerAsync(lodestoneId, cancellationToken: cancellationToken);
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}