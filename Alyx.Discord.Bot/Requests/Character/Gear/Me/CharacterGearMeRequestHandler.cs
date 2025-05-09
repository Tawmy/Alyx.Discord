using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.Character.GetLastForceRefresh;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using Alyx.Discord.Core.Requests.Character.SetLastForceRefresh;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Gear.Me;

internal class CharacterGearMeRequestHandler(
    ISender sender,
    AlyxConfiguration alyxConfiguration,
    [FromKeyedServices(CharacterGearService.Key)]
    IDiscordContainerService<CharacterDtoV3> gearService) : IRequestHandler<CharacterGearMeRequest>
{
    public async Task Handle(CharacterGearMeRequest request, CancellationToken cancellationToken)
    {
        string lodestoneId;
        try
        {
            lodestoneId = await sender.Send(new GetMainCharacterIdRequest(request.Ctx.User.Id), cancellationToken);
        }
        catch (NotFoundException)
        {
            var errorBuilder = new DiscordInteractionResponseBuilder().AddError(
                Messages.Commands.Character.Me.NotFoundDescription(request.GetSlashCommandMapping(), "character claim"),
                Messages.Commands.Character.Me.NotFoundTitle);
            await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
            return;
        }

        if (request.ForceRefresh)
        {
            var lastForceRefresh = await sender.Send(new GetLastForceRefreshRequest(lodestoneId), cancellationToken);
            var allowedBefore = DateTime.Now.AddMinutes(-1 * alyxConfiguration.NetStone.ForceRefreshCooldown);

            if (lastForceRefresh > allowedBefore)
            {
                var formattedLastRefresh = Formatter.Timestamp(lastForceRefresh.Value);
                var allowedAfter = lastForceRefresh.Value.AddMinutes(alyxConfiguration.NetStone.ForceRefreshCooldown);
                var formattedAllowedAfterRel = Formatter.Timestamp(allowedAfter);
                var formattedAllowedAfterAbs = Formatter.Timestamp(allowedAfter, TimestampFormat.ShortDateTime);
                var errorBuilder = new DiscordInteractionResponseBuilder()
                    .AddError(Messages.Commands.Character.Me.ForceRefreshErrorDescription(formattedLastRefresh,
                        formattedAllowedAfterRel, formattedAllowedAfterAbs));
                await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
                return;
            }
        }

        // TODO move the above from this and CharacterMeRequestHandler to a common method

        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        var container = await gearService.CreateContainerAsync(lodestoneId, request.ForceRefresh, cancellationToken);
        await request.Ctx.FollowupAsync(new DiscordFollowupMessageBuilder().EnableV2Components()
            .AddContainerComponent(container));

        if (request.ForceRefresh)
        {
            await sender.Send(new SetLastForceRefreshRequest(lodestoneId, DateTime.UtcNow), cancellationToken);
        }
    }
}