using System.ComponentModel;
using Alyx.Discord.Bot.AutoCompleteProviders;
using Alyx.Discord.Bot.Requests.Character.Claim;
using Alyx.Discord.Bot.Requests.Character.Get;
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
}