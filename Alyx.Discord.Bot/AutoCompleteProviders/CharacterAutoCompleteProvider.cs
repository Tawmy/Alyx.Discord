using Alyx.Discord.Core.Requests.OptionHistory.Get;
using Alyx.Discord.Db.Models;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.AutoCompleteProviders;

internal class CharacterAutoCompleteProvider(ISender sender) : IAutoCompleteProvider
{
    public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext context)
    {
        var request = new OptionHistoryGetRequest(context.User.Id, HistoryType.Character);
        var recentNames = (await sender.Send(request)).AsEnumerable();

        if (!string.IsNullOrWhiteSpace(context.UserInput))
        {
            recentNames = recentNames.Where(x => x.StartsWith(context.UserInput, StringComparison.OrdinalIgnoreCase));
        }

        return recentNames.Select(x => new DiscordAutoCompleteChoice(x, x));
    }
}