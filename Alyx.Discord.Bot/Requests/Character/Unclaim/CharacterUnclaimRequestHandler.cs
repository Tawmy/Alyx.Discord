using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetCharacter;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Unclaim;

internal class CharacterUnclaimRequestHandler(ISender sender, DiscordEmbedService embedService)
    : IRequestHandler<CharacterUnclaimRequest>
{
    public async Task Handle(CharacterUnclaimRequest request, CancellationToken cancellationToken)
    {
        string lodestoneId;
        try
        {
            lodestoneId = await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var notFoundEmbed = embedService.CreateError(Messages.Commands.Character.Unclaim.NoMainCharacterDescription,
                Messages.Commands.Character.Unclaim.NoMainCharacterTitle);
            await request.Ctx.RespondAsync(notFoundEmbed, true);
            return;
        }

        await request.Ctx.DeferResponseAsync(true);

        var character = await sender.Send(new CharacterGetCharacterRequest(lodestoneId), cancellationToken);

        var embed = CreateConfirmationEmbed(character.Name, character.Server);
        var button = CreateConfirmationButton();

        await request.Ctx.FollowupAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed).AddComponents(button));
    }

    private DiscordEmbed CreateConfirmationEmbed(string name, string world)
    {
        var title = Messages.Commands.Character.Unclaim.ConfirmTitle(name, world);
        return embedService.Create(Messages.Commands.Character.Unclaim.ConfirmDescription, title);
    }

    private static DiscordButtonComponent CreateConfirmationButton()
    {
        return new DiscordButtonComponent(DiscordButtonStyle.Danger, ComponentIds.Button.ConfirmUnclaim,
            Messages.Buttons.ConfirmUnclaim);
    }
}