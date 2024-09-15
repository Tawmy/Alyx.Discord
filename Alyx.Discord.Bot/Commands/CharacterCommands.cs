using System.ComponentModel;
using Alyx.Discord.Bot.AutoCompleteProviders;
using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

[Command("character")]
public class CharacterCommands(ISender sender)
{
    [Command("get")]
    [Description(Messages.Commands.Character.Get.Description)]
    public async Task GetAsync(SlashCommandContext ctx,
        [Description(Messages.Commands.Parameters.CharacterName)]
        string name,
        [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
        [Description(Messages.Commands.Parameters.CharacterWorld)]
        string world,
        [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)
    {
        await ctx.DeferResponseAsync(isPrivate);

        var searchDtos = await sender.Send(new CharacterSearchRequest(name, world));

        DiscordInteractionResponseBuilder builder;
        if (isPrivate && searchDtos.Count > 1)
        {
            var select = searchDtos.AsSelectComponent();
            var content = Messages.Commands.Character.Get.SelectMenu(searchDtos.Count);
            builder = new DiscordInteractionResponseBuilder().AddComponents(select).WithContent(content);
        }
        else
        {
            var firstSearchDto = searchDtos.First();
            var content = $"{firstSearchDto.Id} - {firstSearchDto.Name}";
            builder = new DiscordInteractionResponseBuilder().WithContent(content);
        }

        await ctx.FollowupAsync(builder);
    }
}