using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using Alyx.Discord.Core.Requests.OptionHistory.Add;
using Alyx.Discord.Db.Models;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Gear.Get;

internal class CharacterGearGetRequestHandler(
    ISender sender,
    [FromKeyedServices(CharacterGearService.Key)]
    IDiscordContainerService<CharacterDto> gearService) : IRequestHandler<CharacterGearGetRequest>
{
    public async Task Handle(CharacterGearGetRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        ICollection<CharacterSearchPageResultDto> searchDtos;
        DiscordInteractionResponseBuilder builder;
        try
        {
            searchDtos = await sender.Send(new CharacterSearchRequest(request.Name, request.World), cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.Character.Get.CharacterNotFound(request.Name, request.World);
            builder = new DiscordInteractionResponseBuilder().AddError(description);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        if (request.IsPrivate && searchDtos.Count > 1)
        {
            var select = searchDtos.AsSelectComponent(ComponentIds.Select.CharacterForGear);
            builder = new DiscordInteractionResponseBuilder().AddTieBreakerSelect(select, searchDtos.Count);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        var first = searchDtos.FirstOrDefault(x =>
            x.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase)) ?? searchDtos.First();

        var container = await gearService.CreateContainerAsync(first.Id, cancellationToken: cancellationToken);
        await request.Ctx.FollowupAsync(new DiscordFollowupMessageBuilder().EnableV2Components()
            .AddContainerComponent(container));

        // cache recent search for discord user
        await sender.Send(new OptionHistoryAddRequest(request.Ctx.User.Id, HistoryType.Character, first.Name),
            cancellationToken);
    }
}