using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class SelectCharacterHandler(
    ISender sender,
    IInteractionDataService interactionDataService,
    DiscordEmbedService embedService)
    : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId, IReadOnlyDictionary<ulong, Command> commands)
    {
        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();

        var builder = new DiscordFollowupMessageBuilder();
        await builder.CreateSheetAndSendFollowupAsync(sender, interactionDataService, embedService, selectedLodestoneId,
            false, async b => await args.Interaction.CreateFollowupMessageAsync((DiscordFollowupMessageBuilder)b));
    }
}