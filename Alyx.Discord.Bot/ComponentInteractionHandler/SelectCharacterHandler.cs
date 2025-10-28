using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class SelectCharacterHandler(CharacterSheetService sheetService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId, IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default)
    {
        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();

        var builder = new DiscordWebhookBuilder();
        await sheetService.CreateSheetAndSendFollowupAsync(builder, selectedLodestoneId, false,
            async b => await args.Interaction.EditOriginalResponseAsync((DiscordWebhookBuilder)b), cancellationToken);
    }
}