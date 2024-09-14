using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Commands;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

[Command("character")]
public class CharacterCommands(ISender sender)
{
    [Command("get")]
    public async Task GetAsync(CommandContext ctx, string name, string world)
    {
        var result = await sender.Send(new CharacterSearchRequest(name, world));
        await ctx.RespondAsync($"{result}.");
    }
}