using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class SelectCharacterHandler(ISender sender, IDataPersistenceService dataPersistenceService)
    : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();

        var builder = new DiscordFollowupMessageBuilder();
        await builder.CreateSheetAndSendFollowupAsync(sender, dataPersistenceService, selectedLodestoneId,
            async b => await args.Interaction.CreateFollowupMessageAsync((DiscordFollowupMessageBuilder)b));
    }
}