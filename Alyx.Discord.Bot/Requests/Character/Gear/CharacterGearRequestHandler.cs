using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.Search;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Gear;

internal class CharacterGearRequestHandler(ISender sender, CharacterGearService gearService)
    : IRequestHandler<CharacterGearRequest>
{
    public async Task Handle(CharacterGearRequest request, CancellationToken cancellationToken)
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
            var content = Messages.Commands.Character.Get.SelectMenu(searchDtos.Count);
            builder = new DiscordInteractionResponseBuilder().AddActionRowComponent(select).WithContent(content);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        var first = searchDtos.FirstOrDefault(x =>
            x.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase)) ?? searchDtos.First();

        var container = await gearService.CreateGearContainerAsync(first.Id, cancellationToken);
        await request.Ctx.FollowupAsync(new DiscordFollowupMessageBuilder().EnableV2Components()
            .AddContainerComponent(container));
    }
}