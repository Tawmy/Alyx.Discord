using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.SheetMetadata;
using Alyx.Discord.Core.Structs;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using Humanizer;
using MediatR;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonSheetMetadataHandler(
    ISender sender,
    DiscordEmbedService embedService,
    IDataPersistenceService dataPersistenceService) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        ArgumentNullException.ThrowIfNull(dataId);

        if (!dataPersistenceService.TryGetData<PersistentData.SheetMetadata>(dataId, out var sheetMetadata))
        {
            // TODO potentially move this into dataPersistenceService or even discordEmbedService?
            var embed = embedService.CreateError(Messages.DataPersistence.NotPersisted);
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral());
            return;
        }

        await args.Interaction.DeferAsync(true);

        var cacheMetadata =
            await sender.Send(
                new CharacterSheetMetadataRequest(sheetMetadata.LodestoneId, sheetMetadata.OriginalTimestamp));

        var builder = new DiscordFollowupMessageBuilder();
        builder.AddEmbed(CreateMetadataEmbed(cacheMetadata));

        await args.Interaction.CreateFollowupMessageAsync(builder);
    }

    private static DiscordEmbed CreateMetadataEmbed(ICollection<SheetMetadata> metadata)
    {
        var builder = new DiscordEmbedBuilder();

        builder.WithTitle(Messages.Events.SheetMetadata.Title);
        builder.WithDescription(Messages.Events.SheetMetadata.Description);

        foreach (var entry in metadata)
        {
            builder.AddField(entry.Title, entry.LastUpdated.Humanize(), true);
        }

        return builder.Build();
    }
}