using Alyx.Discord.Bot.Commands;
using Alyx.Discord.Bot.ComponentInteractionHandler;
using Alyx.Discord.Bot.EventHandlers;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Requests.Character.Get;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot;

public static class DependencyInjection
{
    public static void AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(CharacterSearchRequest).Assembly,
                typeof(CharacterGetRequest).Assembly);
        });

        var token = configuration.GetGuardedConfiguration(EnvironmentVariables.BotToken);
        services.AddDiscordClient(token, DiscordIntents.AllUnprivileged);

        var debugGuildId = configuration.GetOptionalConfiguration<ulong>(EnvironmentVariables.DebugGuildId) ?? 0;
        services.AddCommandsExtension(x => x.AddCommands<CharacterCommands>(),
            new CommandsConfiguration { DebugGuildId = debugGuildId });

        services.AddComponentInteractionHandlers();
        services.ConfigureEventHandlers(x => x.AddEventHandlers<ComponentInteractionCreatedEventHandler>());

        services.AddHostedService<BotService>();
    }

    private static void AddComponentInteractionHandlers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IComponentInteractionHandler, SelectCharacterHandler>(ComponentIds.Select.Character);
    }
}