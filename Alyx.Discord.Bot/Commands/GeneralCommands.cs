using System.ComponentModel;
using Alyx.Discord.Bot.Requests.General.About;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Metadata;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

internal class GeneralCommands(ISender sender)
{
    [Command("about")]
    [Description(Messages.Commands.General.About.Description)]
    [InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall,
        DiscordApplicationIntegrationType.UserInstall)]
    [InteractionAllowedContexts(DiscordInteractionContextType.Guild, DiscordInteractionContextType.BotDM,
        DiscordInteractionContextType.PrivateChannel)]
    public Task AboutAsync(SlashCommandContext ctx,
        [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)

    {
        return sender.Send(new AboutRequest(ctx, isPrivate));
    }
}