using System.ComponentModel;
using Alyx.Discord.Bot.AutoCompleteProviders;
using Alyx.Discord.Bot.Requests.Character.Attributes.Get;
using Alyx.Discord.Bot.Requests.Character.Attributes.Me;
using Alyx.Discord.Bot.Requests.Character.Claim;
using Alyx.Discord.Bot.Requests.Character.Gear.Get;
using Alyx.Discord.Bot.Requests.Character.Gear.Me;
using Alyx.Discord.Bot.Requests.Character.Get;
using Alyx.Discord.Bot.Requests.Character.Jobs.Get;
using Alyx.Discord.Bot.Requests.Character.Jobs.Me;
using Alyx.Discord.Bot.Requests.Character.Me;
using Alyx.Discord.Bot.Requests.Character.Unclaim;
using Alyx.Discord.Bot.Services.CharacterJobs;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands.Metadata;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

[Command("character")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM,
    DiscordInteractionContextType.PrivateChannel)]
internal class CharacterCommands(ISender sender)
{
    [Command("get")]
    [Description(Messages.Commands.Character.Get.Description)]
    public Task GetAsync(SlashCommandContext ctx,
        [SlashAutoCompleteProvider<CharacterAutoCompleteProvider>]
        [Description(Messages.Commands.Parameters.CharacterNameWithCompletion)]
        string name,
        [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
        [Description(Messages.Commands.Parameters.CharacterWorld)]
        string world,
        [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)
    {
        return sender.Send(new CharacterGetRequest(ctx, name, world, isPrivate));
    }

    [Command("me")]
    [Description(Messages.Commands.Character.Me.Description)]
    public Task MeAsync(SlashCommandContext ctx,
        [Description(Messages.Commands.Parameters.ForceRefresh)]
        bool forceRefresh = false,
        [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)
    {
        return sender.Send(new CharacterMeRequest(ctx, forceRefresh, isPrivate));
    }

    [Command("claim")]
    [Description(Messages.Commands.Character.Claim.Description)]
    public Task ClaimAsync(SlashCommandContext ctx,
        [Description(Messages.Commands.Parameters.CharacterName)]
        string name,
        [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
        [Description(Messages.Commands.Parameters.CharacterWorld)]
        string world)
    {
        return sender.Send(new CharacterClaimRequest(ctx, name, world));
    }

    [Command("unclaim")]
    [Description(Messages.Commands.Character.Unclaim.Description)]
    public Task UnclaimAsync(SlashCommandContext ctx)
    {
        return sender.Send(new CharacterUnclaimRequest(ctx));
    }

    [Command("gear")]
    internal class Gear(ISender sender)
    {
        [Command("get")]
        [Description(Messages.Commands.Character.Gear.GearGet.Description)]
        public Task GetAsync(SlashCommandContext ctx,
            [SlashAutoCompleteProvider<CharacterAutoCompleteProvider>]
            [Description(Messages.Commands.Parameters.CharacterNameWithCompletion)]
            string name,
            [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
            [Description(Messages.Commands.Parameters.CharacterWorld)]
            string world,
            [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
            bool isPrivate = false)
        {
            return sender.Send(new CharacterGearGetRequest(ctx, name, world, isPrivate));
        }

        [Command("me")]
        [Description(Messages.Commands.Character.Gear.GearMe.Description)]
        public Task MeAsync(SlashCommandContext ctx,
            [Description(Messages.Commands.Parameters.ForceRefresh)]
            bool forceRefresh = false,
            [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
            bool isPrivate = false)
        {
            return sender.Send(new CharacterGearMeRequest(ctx, forceRefresh, isPrivate));
        }
    }

    [Command("attributes")]
    internal class Attributes(ISender sender)
    {
        [Command("get")]
        [Description(Messages.Commands.Character.Attributes.AttributesGet.Description)]
        public Task GetAsync(SlashCommandContext ctx,
            [SlashAutoCompleteProvider<CharacterAutoCompleteProvider>]
            [Description(Messages.Commands.Parameters.CharacterNameWithCompletion)]
            string name,
            [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
            [Description(Messages.Commands.Parameters.CharacterWorld)]
            string world,
            [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
            bool isPrivate = false)
        {
            return sender.Send(new CharacterAttributesGetRequest(ctx, name, world, isPrivate));
        }

        [Command("me")]
        [Description(Messages.Commands.Character.Attributes.AttributesMe.Description)]
        public Task MeAsync(SlashCommandContext ctx,
            [Description(Messages.Commands.Parameters.ForceRefresh)]
            bool forceRefresh = false,
            [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
            bool isPrivate = false)
        {
            return sender.Send(new CharacterAttributesMeRequest(ctx, forceRefresh, isPrivate));
        }
    }

    [Command("jobs")]
    internal class Jobs(ISender sender)
    {
        [Command("get")]
        [Description(Messages.Commands.Character.Jobs.JobsGet.Description)]
        public Task GetAsync(SlashCommandContext ctx,
            [Description(Messages.Commands.Parameters.CharacterNameWithCompletion)]
            Role role,
            [SlashAutoCompleteProvider<CharacterAutoCompleteProvider>]
            [Description(Messages.Commands.Parameters.CharacterNameWithCompletion)]
            string name,
            [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
            [Description(Messages.Commands.Parameters.CharacterWorld)]
            string world,
            [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
            bool isPrivate = false)
        {
            return sender.Send(new CharacterJobsGetRequest(ctx, role, name, world, isPrivate));
        }

        [Command("me")]
        [Description(Messages.Commands.Character.Jobs.JobsMe.Description)]
        public Task MeAsync(SlashCommandContext ctx,
            [Description(Messages.Commands.Parameters.CharacterNameWithCompletion)]
            Role role,
            [Description(Messages.Commands.Parameters.ForceRefresh)]
            bool forceRefresh = false,
            [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
            bool isPrivate = false)
        {
            return sender.Send(new CharacterJobsMeRequest(ctx, role, forceRefresh, isPrivate));
        }
    }
}