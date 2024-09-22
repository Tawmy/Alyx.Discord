using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Core.Requests.Character.Sheet;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using SixLabors.ImageSharp.Formats.Webp;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

public class SelectCharacterHandler(ISender sender) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args)
    {
        await args.Interaction.DeferAsync(true);
        var selectedLodestoneId = args.Values.First();

        var sheet = await sender.Send(new CharacterSheetRequest(selectedLodestoneId));

        await using var stream = new MemoryStream();
        await sheet.SaveAsync(stream, new WebpEncoder());
        stream.Seek(0, SeekOrigin.Begin);

        var fileName = $"{DateTime.UtcNow:yyyy-MM-dd HH-mm} {selectedLodestoneId}.webp";

        var button = CreateLodestoneLinkButton(selectedLodestoneId);

        var builder = new DiscordFollowupMessageBuilder().AddFile(fileName, stream, true).AddComponents(button);

        await args.Interaction.CreateFollowupMessageAsync(builder);
    }

    private static DiscordLinkButtonComponent CreateLodestoneLinkButton(string characterId)
    {
        var url = $"https://eu.finalfantasyxiv.com/lodestone/character/{characterId}";
        return new DiscordLinkButtonComponent(url, "Open Lodestone profile");
    }
}