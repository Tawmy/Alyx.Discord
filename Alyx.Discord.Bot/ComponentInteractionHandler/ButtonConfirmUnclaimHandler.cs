using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using Alyx.Discord.Core.Requests.Character.Unclaim;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;
using MediatR;
using NotFoundException = NetStone.Common.Exceptions.NotFoundException;

namespace Alyx.Discord.Bot.ComponentInteractionHandler;

internal class ButtonConfirmUnclaimHandler(ISender sender, DiscordEmbedService embedService)
    : IComponentInteractionHandler
{
    public async Task HandleAsync(DiscordClient discordClient, ComponentInteractionCreatedEventArgs args,
        string? dataId)
    {
        try
        {
            // we do not need the return value here, we only need to catch the exception
            await sender.Send(new GetMainCharacterIdRequest(args.Interaction.User.Id));
        }
        catch (NotFoundException)
        {
            var embed = embedService.CreateError(Messages.Commands.Character.Unclaim.NoMainCharacterDescription,
                Messages.Commands.Character.Unclaim.NoMainCharacterTitle);
            var builder = new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral();
            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                builder);
            return;
        }

        await sender.Send(new CharacterUnclaimRequest(args.Interaction.User.Id));

        {
            var embed = embedService.Create(Messages.Commands.Character.Unclaim.SuccessDescription,
                Messages.Commands.Character.Unclaim.SuccessTitle);
            var builder = new DiscordInteractionResponseBuilder().AddEmbed(embed).AsEphemeral();

            await args.Interaction.CreateResponseAsync(DiscordInteractionResponseType.ChannelMessageWithSource,
                builder);
        }
    }
}