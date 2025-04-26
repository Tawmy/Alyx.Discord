using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.Interfaces;

public interface IComponentInteractionHandler
{
    Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId,
        IReadOnlyDictionary<ulong, Command> commands, CancellationToken cancellationToken = default);
}