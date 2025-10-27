using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.Services.CharacterJobs;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.Character.GetLastForceRefresh;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using Alyx.Discord.Core.Requests.Character.SetLastForceRefreshCharacter;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Jobs.Me;

internal class CharacterJobsMeRequestHandler(
    ISender sender,
    AlyxConfiguration config,
    [FromKeyedServices(CharacterClassJobsService.Key)]
    IDiscordContainerServiceCustom<CharacterClassJobOuterDto, Role> jobsService)
    : IRequestHandler<CharacterJobsMeRequest>
{
    public async Task Handle(CharacterJobsMeRequest request, CancellationToken cancellationToken)
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
            var allowedBefore = DateTime.Now.AddMinutes(-1 * config.NetStone.ForceRefreshCooldown);

            if (lastForceRefresh.LastForceRefreshCharacter > allowedBefore)
            {
                var formattedLastRefresh = Formatter.Timestamp(lastForceRefresh.LastForceRefreshCharacter.Value);
                var allowedAfter =
                    lastForceRefresh.LastForceRefreshCharacter.Value.AddMinutes(config.NetStone.ForceRefreshCooldown);
                var formattedAllowedAfterRel = Formatter.Timestamp(allowedAfter);
                var formattedAllowedAfterAbs = Formatter.Timestamp(allowedAfter, TimestampFormat.ShortDateTime);
                var errorBuilder = new DiscordInteractionResponseBuilder()
                    .AddError(Messages.Commands.Character.Me.ForceRefreshErrorDescription(formattedLastRefresh,
                        formattedAllowedAfterRel, formattedAllowedAfterAbs));
                await request.Ctx.RespondAsync(errorBuilder.AsEphemeral());
                return;
            }
        }

        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        var container = await jobsService.CreateContainerAsync(request.Role, lodestoneId, request.ForceRefresh,
            cancellationToken);

        await request.Ctx.FollowupAsync(new DiscordFollowupMessageBuilder().EnableV2Components()
            .AddContainerComponent(container));

        if (request.ForceRefresh)
        {
            await sender.Send(new SetLastForceRefreshCharacterRequest(lodestoneId, DateTime.UtcNow),
                cancellationToken);
        }
    }
}