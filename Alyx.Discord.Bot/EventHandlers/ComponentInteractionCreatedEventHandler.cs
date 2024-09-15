using Alyx.Discord.Bot.Interfaces;
using DSharpPlus;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.DependencyInjection;

namespace Alyx.Discord.Bot.EventHandlers;

public class ComponentInteractionCreatedEventHandler(IServiceProvider serviceProvider)
    : IEventHandler<ComponentInteractionCreatedEventArgs>
{
    public Task HandleEventAsync(DiscordClient sender, ComponentInteractionCreatedEventArgs args)
    {
        var handler = serviceProvider.GetRequiredKeyedService<IComponentInteractionHandler>(args.Id);
        return handler.HandleAsync(sender, args);
    }
}