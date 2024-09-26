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
        var componentIdSplit = args.Id.Split('/');
        var componentId = componentIdSplit[0];
        var dataId = componentIdSplit.Length > 1 ? componentIdSplit[1] : null;

        var handler = serviceProvider.GetRequiredKeyedService<IComponentInteractionHandler>(componentId);
        return handler.HandleAsync(sender, args, dataId);
    }
}