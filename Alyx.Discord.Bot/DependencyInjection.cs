using Alyx.Discord.Bot.Commands;
using Alyx.Discord.Bot.ComponentInteractionHandler;
using Alyx.Discord.Bot.EventHandlers;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Requests.Character.Get;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.Services.CharacterJobs;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.MessageCommands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.UserCommands;
using DSharpPlus.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Extensions;

namespace Alyx.Discord.Bot;

using CoreRequest = CharacterSearchRequest;
using BotRequest = CharacterGetRequest;

public static class DependencyInjection
{
    public static void AddBotServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<IInteractionDataService, InteractionDataService>();
        services.AddSingleton<CachingService>();
        services.AddSingleton<ProgressBarService>();
        services.AddSingleton<CharacterClaimService>();
        services.AddKeyedSingleton<IDiscordContainerService<CharacterDto>, CharacterGearService>(
            CharacterGearService.Key);
        services.AddKeyedSingleton<IDiscordContainerService<CharacterDto>, CharacterAttributesService>(
            CharacterAttributesService.Key);
        services.AddKeyedSingleton<IDiscordContainerService<FreeCompanyDto>, FreeCompanyService>(
            FreeCompanyService.Key);
        services.AddKeyedSingleton<IDiscordContainerServiceCustom<(CharacterDto, CharacterClassJobOuterDto), Role>,
            CharacterClassJobsService>(CharacterClassJobsService.Key);

        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssemblies(typeof(CoreRequest).Assembly, typeof(BotRequest).Assembly);
            // Generic handlers are registered in Alyx.Discord.Api.Extensions.ConfigureHostBuilderExtension
        });

        var token = configuration.GetGuardedConfiguration(EnvironmentVariables.BotToken);
        services.AddDiscordClient(token, DiscordIntents.AllUnprivileged).Configure<DiscordConfiguration>(x =>
        {
            x.LogUnknownAuditlogs = false;
            x.LogUnknownEvents = false;
        });

        var debugGuildId = configuration.GetOptionalConfiguration<ulong>(EnvironmentVariables.DebugGuildId) ?? 0;
        services.AddCommandsExtension((_, x) =>
            {
                x.AddProcessor<SlashCommandProcessor>();
                x.AddProcessor<UserCommandProcessor>();
                x.AddProcessor<MessageCommandProcessor>();

                x.AddCommands<GeneralCommands>();
                x.AddCommands<CharacterCommands>();
                x.AddCommands<FreeCompanyCommands>();
                x.AddCommands<FfxivCommands>();

                // using generic type does not work if class isn't a command
                x.AddCommands(typeof(UserContextMenuCommands));
            },
            new CommandsConfiguration { DebugGuildId = debugGuildId, RegisterDefaultCommandProcessors = false });

        services.AddComponentInteractionHandlers();
        services.ConfigureEventHandlers(x =>
        {
            x.AddEventHandlers<ClientStartedEventHandler>();
            x.AddEventHandlers<GuildDownloadCompletedEventHandler>();
            x.AddEventHandlers<ComponentInteractionCreatedEventHandler>();
        });

        services.AddHostedService<BotService>();
    }

    private static void AddComponentInteractionHandlers(this IServiceCollection services)
    {
        services.AddKeyedScoped<IComponentInteractionHandler, SelectCharacterHandler>(ComponentIds.Select.Character);
        services.AddKeyedScoped<IComponentInteractionHandler, SelectCharacterForGearHandler>(ComponentIds.Select
            .CharacterForGear);
        services.AddKeyedScoped<IComponentInteractionHandler, SelectCharacterForAttributesHandler>(ComponentIds.Select
            .CharacterForAttributes);
        services.AddKeyedScoped<IComponentInteractionHandler, SelectCharacterForClassJobsHandler>(ComponentIds.Select
            .CharacterForClassJobs);

        services.AddKeyedScoped<IComponentInteractionHandler, SelectFreeCompanyHandler>(ComponentIds.Select
            .FreeCompany);

        services.AddKeyedScoped<IComponentInteractionHandler, ButtonConfirmClaimHandler>(ComponentIds.Button
            .ConfirmClaim);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonConfirmUnclaimHandler>(ComponentIds.Button
            .ConfirmUnclaim);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterSheetMoreHandler>(ComponentIds.Button
            .CharacterSheetMore);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterSheetGearHandler>(ComponentIds.Button
            .CharacterSheetGear);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterSheetAttributesHandler>(ComponentIds.Button
            .CharacterSheetAttributes);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterSheetClassJobsHandler>(ComponentIds.Button
            .CharacterSheetClassJobs);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterSheetFreeCompanyHandler>(ComponentIds
            .Button.CharacterSheetFreeCompany);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterGearHandler>(ComponentIds.Button
            .CharacterGear);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterAttributesHandler>(ComponentIds.Button
            .CharacterAttributes);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonCharacterClassJobsHandler>(ComponentIds.Button
            .CharacterClassJobs);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonFreeCompanyHandler>(ComponentIds.Button
            .CharacterFreeCompany);
        services.AddKeyedScoped<IComponentInteractionHandler, ButtonSheetMetadataHandler>(ComponentIds.Button
            .CharacterSheetMetadata);
    }
}