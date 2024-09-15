using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using NetStone.Common.Enums;

namespace Alyx.Discord.Bot.AutoCompleteProviders;

public class ServerAutoCompleteProvider : IAutoCompleteProvider
{
    public ValueTask<IReadOnlyDictionary<string, object>> AutoCompleteAsync(AutoCompleteContext context)
    {
        var servers = Enum.GetValues<Server>().Select(x => x.ToString());

        if (!string.IsNullOrWhiteSpace(context.UserInput))
        {
            servers = servers.Where(x => x.StartsWith(context.UserInput, StringComparison.OrdinalIgnoreCase));
        }

        servers = servers.OrderBy(x => x).Take(25);

        var dict = servers.Select((x, i) => (x, i)).ToDictionary(x => x.x, object (x) => x.x);
        return ValueTask.FromResult<IReadOnlyDictionary<string, object>>(dict);
    }
}