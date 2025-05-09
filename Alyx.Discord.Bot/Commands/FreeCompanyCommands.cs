using System.ComponentModel;
using Alyx.Discord.Bot.AutoCompleteProviders;
using Alyx.Discord.Bot.Requests.FreeCompany.Me;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Commands.Processors.SlashCommands.Metadata;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

[Command("fc")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
[InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM,
    DiscordInteractionContextType.PrivateChannel)]
public class FreeCompanyCommands(ISender sender)
{
    [Command("me")]
    [Description(Messages.Commands.FreeCompany.Me.Description)]
    public Task MeAsync(SlashCommandContext ctx,
        [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)
    {
        return sender.Send(new FreeCompanyMeRequest(ctx, isPrivate));
    }
}