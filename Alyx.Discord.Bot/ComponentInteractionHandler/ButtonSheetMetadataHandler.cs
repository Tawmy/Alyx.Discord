using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Structs;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonSheetMetadataHandler(
    DiscordEmbedService embedService,
    IDataPersistenceService dataPersistenceService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        if (!dataPersistenceService.TryGetData<IEnumerable<SheetMetadata>>(dataId, out var sheetMetadata))
        {
            // TODO potentially move this into dataPersistenceService or even discordEmbedService?
            var embed = embedService.CreateError(Messages.DataPersistence.NotPersisted);
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
            builder.AddField(entry.Title, Formatter.Timestamp(entry.LastUpdated), true);
        }

        return builder.Build();
    }
}