using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using MediatR;
using NetStone.Common.Exceptions;
using CoreRequest = Alyx.Discord.Core.Requests.Character.Unclaim.CharacterUnclaimRequest;

namespace Alyx.Discord.Bot.Requests.Character.Unclaim;

internal class CharacterUnclaimRequestHandler(ISender sender, DiscordEmbedService embedService)
    : IRequestHandler<CharacterUnclaimRequest>
{
    public async Task Handle(CharacterUnclaimRequest request, CancellationToken cancellationToken)
    {
        try
        {
            // throws exception when not found, we do not need return value
            await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var notFoundEmbed = embedService.CreateError(Messages.Commands.Character.Unclaim.NoMainCharacterDescription,
                Messages.Commands.Character.Unclaim.NoMainCharacterTitle);
            await request.Ctx.RespondAsync(notFoundEmbed, true);
            return;
        }

        await sender.Send(new CoreRequest(request.Ctx.User.Id), cancellationToken);

        var embed = embedService.Create(Messages.Commands.Character.Unclaim.SuccessDescription,
            Messages.Commands.Character.Unclaim.SuccessTitle);
        await request.Ctx.RespondAsync(embed, true);
    }
}