using Alyx.Discord.Bot.Interfaces;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using NetStone.Api.Client;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

public class SelectCharacterHandler(NetStoneApiClient client) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args)
    {
        await args.Interaction.DeferAsync(true);
        var selected = args.Values.First();
        var dto = await client.Character.GetAsync(selected);
        var builder = new DiscordFollowupMessageBuilder().WithContent($"{dto.Name} ({dto.Server}).");
        await args.Interaction.CreateFollowupMessageAsync(builder);
    }
}