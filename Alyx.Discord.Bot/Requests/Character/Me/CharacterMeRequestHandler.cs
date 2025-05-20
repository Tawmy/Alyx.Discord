using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Configuration;
using Alyx.Discord.Core.Requests.Character.GetLastForceRefresh;
using Alyx.Discord.Core.Requests.Character.GetMainCharacterId;
using Alyx.Discord.Core.Requests.Character.SetLastForceRefresh;
using DSharpPlus;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Me;

internal class CharacterMeRequestHandler(
    ISender sender,
    IInteractionDataService interactionDataService,
    AlyxConfiguration alyxConfiguration)
    : IRequestHandler<CharacterMeRequest>
{
    public async Task Handle(CharacterMeRequest request, CancellationToken cancellationToken)
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

        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        var builder = new DiscordInteractionResponseBuilder();

        await builder.CreateSheetAndSendFollowupAsync(sender, interactionDataService, lodestoneId, request.ForceRefresh,
            async b => await request.Ctx.RespondAsync(b), cancellationToken);

        if (request.ForceRefresh)
        {
            await sender.Send(new SetLastForceRefreshRequest(lodestoneId, DateTime.UtcNow), cancellationToken);
        }
    }
}