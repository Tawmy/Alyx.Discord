using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using NetStone.Common.Enums;

namespace Alyx.Discord.Bot.AutoCompleteProviders;

internal class ServerAutoCompleteProvider : IAutoCompleteProvider
{
    public ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
    {
        var servers = Enum.GetValues<Server>().Select(x => x.ToString());

        if (!string.IsNullOrWhiteSpace(context.UserInput))
        {
            servers = servers.Where(x => x.StartsWith(context.UserInput, StringComparison.OrdinalIgnoreCase));
        }

        servers = servers.OrderBy(x => x).Take(25);

        var choices = servers.Select(x => new DiscordAutoCompleteChoice(x, x));
        return ValueTask.FromResult(choices);
    }
}