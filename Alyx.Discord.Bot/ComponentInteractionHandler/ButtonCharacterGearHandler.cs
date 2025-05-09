using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.InteractionData.Get;
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
    IDiscordContainerService gearService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        await args.Interaction.DeferAsync(true);

        CharacterDtoV3? character = null;
        string? lodestoneId = null;
        try
        {
            // full character is cached in version 1.8.0 and onwards, so gear button will show gear from cache
            // doing this will make sure gear shown will match what's shown in sheet
            character = await interactionDataService.GetDataAsync<CharacterDtoV3>(dataId);
        }
        catch (TypeMismatchException)
        {
            // version 1.7.0 used NetStone API to retrieve character again, which would cause gear to be refreshed
            lodestoneId = await interactionDataService.GetDataAsync<string>(dataId);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db 
            await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder()
                .AddError(Messages.InteractionData.NotPersisted));
            return;
        }

        var container = character is not null
            ? await gearService.CreateContainerAsync(character)
            : lodestoneId is not null
                ? await gearService.CreateContainerAsync(lodestoneId, cancellationToken: cancellationToken)
                : throw new InvalidOperationException("Lodestone ID must not be null");

        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}