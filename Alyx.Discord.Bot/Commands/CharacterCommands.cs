using System.ComponentModel;
using Alyx.Discord.Bot.AutoCompleteProviders;
using Alyx.Discord.Bot.Requests.Character.Claim;
using Alyx.Discord.Bot.Requests.Character.Get;
using Alyx.Discord.Bot.Requests.Character.Me;
using Alyx.Discord.Bot.Requests.Character.Unclaim;
using Alyx.Discord.Bot.StaticValues;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using MediatR;

namespace Alyx.Discord.Bot.Commands;

[Command("character")]
internal class CharacterCommands(ISender sender)
{
    [Command("get")]
    [Description(Messages.Commands.Character.Get.Description)]
    public Task GetAsync(SlashCommandContext ctx,
        [Description(Messages.Commands.Parameters.CharacterName)]
        string name,
        [SlashAutoCompleteProvider<ServerAutoCompleteProvider>]
        [Description(Messages.Commands.Parameters.CharacterWorld)]
        string world,
        [Description(Messages.Commands.Parameters.Private)]
        bool isPrivate = false)
    {
        return sender.Send(new CharacterGetRequest(ctx, name, world, isPrivate));
    }

    [Command("me")]
    [Description(Messages.Commands.Character.Me.Description)]
    public Task MeAsync(SlashCommandContext ctx)
    {
        return sender.Send(new CharacterMeRequest(ctx));
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
}