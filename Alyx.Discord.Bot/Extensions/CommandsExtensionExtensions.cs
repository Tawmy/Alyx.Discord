using System.Collections.ObjectModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Trees;

namespace Alyx.Discord.Bot.Extensions;

internal static class CommandsExtensionExtensions
{
    public static IReadOnlyDictionary<ulong, Command> GetSlashCommandMapping(this CommandsExtension commandsExtension)
    {
        if (!commandsExtension.Processors.TryGetValue(typeof(SlashCommandProcessor), out var commandProcessor))
        {
            return ReadOnlyDictionary<ulong, Command>.Empty;
        }

        var slashCommandProcessor = (SlashCommandProcessor)commandProcessor;
        return slashCommandProcessor.ApplicationCommandMapping;
    }
}