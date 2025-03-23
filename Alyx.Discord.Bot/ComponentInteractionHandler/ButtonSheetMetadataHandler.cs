using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Structs;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonSheetMetadataHandler(
    IInteractionDataService interactionDataService,
    DiscordEmbedService embedService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        IEnumerable<SheetMetadata> sheetMetadata;
        try
        {
            sheetMetadata = await interactionDataService.GetDataAsync<IEnumerable<SheetMetadata>>(dataId);
        }
        catch (InvalidOperationException)
        {
            // this can be removed long-term, only here to not break functionality from before data was persisted to db 
            var embed = embedService.CreateError(Messages.InteractionData.NotPersisted);
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
            return;
        }

        var builder = new DiscordInteractionResponseBuilder().AsEphemeral();
        builder.AddEmbed(CreateMetadataEmbed(sheetMetadata));

        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }

    private static DiscordEmbed CreateMetadataEmbed(IEnumerable<SheetMetadata> metadata)
    {
        var builder = new DiscordEmbedBuilder();

        builder.WithTitle(Messages.Events.SheetMetadata.Title);
        builder.WithDescription(Messages.Events.SheetMetadata.Description);

        foreach (var entry in metadata)
        {
            var message = $"{Formatter.Timestamp(entry.LastUpdated)}";
            if (entry.FallbackUsed)
            {
                message += "\r*Cache Fallback*";
                message += $"\r{new string(entry.FallbackReason?.Take(140).ToArray())}";
            }

            builder.AddField(entry.Title, message, !entry.FallbackUsed);
        }

        return builder.Build();
    }
}