using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class SelectCharacterForGearHandler(CharacterGearService gearService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient client, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();

        var container = await gearService.CreateGearContainerAsync(selectedLodestoneId,
            cancellationToken: cancellationToken);
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components().AddContainerComponent(container);
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}