using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Interfaces;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Requests.Character.GetCharacterByName;
using Alyx.Discord.Core.Requests.Character.Search;
using Alyx.Discord.Core.Requests.OptionHistory.Add;
using Alyx.Discord.Db.Models;
using DSharpPlus.Entities;
using MediatR;
using NetStone.Api.Sdk;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.Character.Get;

internal class CharacterGetRequestHandler(
    ISender sender,
    IInteractionDataService interactionDataService)
    : IRequestHandler<CharacterGetRequest>
{
    public async Task Handle(CharacterGetRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        ICollection<CharacterSearchPageResultDto> searchDtos;
        try
        {
            searchDtos = await sender.Send(new CharacterSearchRequest(request.Name, request.World), cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.Character.Get.CharacterNotFound(request.Name, request.World);
            var builder = new DiscordInteractionResponseBuilder().AddError(description);
            await request.Ctx.FollowupAsync(builder);
            return;
        }
        catch (NetStoneException)
        {
            // Search unavailable, Lodestone might be down. Try retrieving character from cache. 
            await RequestFromCacheByNameAsync(request, cancellationToken);
            return;
        }

        await RequestFromSearchResultAsync(request, searchDtos, cancellationToken);
    }

    private async Task RequestFromSearchResultAsync(CharacterGetRequest request,
        ICollection<CharacterSearchPageResultDto> searchDtos, CancellationToken cancellationToken)
    {
        DiscordInteractionResponseBuilder builder;
        if (request.IsPrivate && searchDtos.Count > 1)
        {
            var select = searchDtos.AsSelectComponent(ComponentIds.Select.Character);
            builder = new DiscordInteractionResponseBuilder().AddTieBreakerSelect(select, searchDtos.Count);
            await request.Ctx.FollowupAsync(builder);
        }
        else
        {
            var first = searchDtos.FirstOrDefault(x =>
                x.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase)) ?? searchDtos.First();

            builder = new DiscordInteractionResponseBuilder();
            await builder.CreateSheetAndSendFollowupAsync(sender, interactionDataService, first.Id, false,
                async b => await request.Ctx.RespondAsync(b), cancellationToken);

            // cache recent search for discord user
            await sender.Send(new OptionHistoryAddRequest(request.Ctx.User.Id, HistoryType.Character, first.Name),
                cancellationToken);
        }
    }

    private async Task RequestFromCacheByNameAsync(CharacterGetRequest request, CancellationToken cancellationToken)
    {
        CharacterDto character;
        var builder = new DiscordInteractionResponseBuilder();
        try
        {
            character = await sender.Send(new CharacterGetCharacterByNameRequest(request.Name, request.World),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.Character.Get.CharacterNotFoundInCache(request.Name, request.World);
            builder = new DiscordInteractionResponseBuilder().AddError(description);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        await builder.CreateSheetAndSendFollowupAsync(sender, interactionDataService, character.Id, false,
            async b => await request.Ctx.RespondAsync(b), cancellationToken);

        // cache recent search for discord user
        await sender.Send(new OptionHistoryAddRequest(request.Ctx.User.Id, HistoryType.Character, character.Name),
            cancellationToken);
    }
}