using DSharpPlus;
using DSharpPlus.EventArgs;

namespace Alyx.Discord.Bot.Interfaces;

public interface IComponentInteractionHandler
{
    Task HandleAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args, string? dataId);
}