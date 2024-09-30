using System.ComponentModel;
using Alyx.Discord.Bot.Requests.Ffxiv.Copypasta;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.Metadata;
using DSharpPlus.Entities;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

[Command("ffxiv")]
[InteractionInstallType(DiscordApplicationIntegrationType.GuildInstall, DiscordApplicationIntegrationType.UserInstall)]
public class FfxivCommands(ISender sender)
{
    [Command("copypasta")]
    [Description(Messages.Commands.Ffxiv.Copypasta.Description)]
    public Task CopypastaAsync(SlashCommandContext ctx,
        [Parameter("private")] [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)
    {
        return sender.Send(new FfxivCopypastaRequest(ctx, isPrivate));
    }
}