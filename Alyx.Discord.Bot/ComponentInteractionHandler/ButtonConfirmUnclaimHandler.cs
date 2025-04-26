using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using Alyx.Discord.Core.Requests.Character.Unclaim;
using DSharpPlus;
using DSharpPlus.Commands.Trees;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using NotFoundException = NetStone.Common.Exceptions.NotFoundException;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonConfirmUnclaimHandler(ISender sender) : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId, IReadOnlyDictionary<ulong, Command> commands)
    {
        try
        {
            // we do not need the return value here, we only need to catch the exception
            await sender.Send(new GetMainCharacterIdRequest(args.Interaction.User.Id));
        }
        catch (NotFoundException)
        {
            var errorBuilder = new DiscordInteractionResponseBuilder().AsEphemeral().AddError(
                Messages.Commands.Character.Unclaim.NoMainCharacterDescription(commands, "character claim"),
                Messages.Commands.Character.Unclaim.NoMainCharacterTitle);
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                errorBuilder);
            return;
        }

        await sender.Send(new CharacterUnclaimRequest(args.Interaction.User.Id));

        var builder = new DiscordInteractionResponseBuilder().EnableV2Components().AsEphemeral();

        var desc = Messages.Commands.Character.Unclaim.SuccessDescription(commands, "character claim");
        builder.AddContainerComponent(new DiscordContainerComponent([
            new DiscordTextDisplayComponent($"# {Messages.Commands.Character.Unclaim.SuccessTitle}"),
            new DiscordSeparatorComponent(true, DiscordSeparatorSpacing.Large),
            new DiscordTextDisplayComponent(desc)
        ]));

        await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource, builder);
    }
}