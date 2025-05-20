using Alyx.Discord.Bot.Extensions;
using Alyx.Discord.Bot.Services;
using Alyx.Discord.Bot.StaticValues;
using Alyx.Discord.Core.Extensions;
using Alyx.Discord.Core.Requests.FreeCompany.GetFreeCompanyByName;
using Alyx.Discord.Core.Requests.FreeCompany.Search;
using Alyx.Discord.Core.Requests.OptionHistory.Add;
using Alyx.Discord.Db.Models;
using DSharpPlus.Entities;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using NetStone.Api.Sdk;
using NetStone.Common.DTOs.FreeCompany;
using NetStone.Common.Exceptions;

namespace Alyx.Discord.Bot.Requests.FreeCompany.Get;

internal class FreeCompanyGetRequestHandler(
    ISender sender,
    HttpClient httpClient,
    [FromKeyedServices("fc")] IDiscordContainerService<FreeCompanyDto> fcService)
    : IRequestHandler<FreeCompanyGetRequest>
{
    public async Task Handle(FreeCompanyGetRequest request, CancellationToken cancellationToken)
    {
        await request.Ctx.DeferResponseAsync(request.IsPrivate);

        ICollection<FreeCompanySearchPageResultDto> searchDtos;
        try
        {
            searchDtos = await sender.Send(new FreeCompanySearchRequest(request.Name, request.World),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.FreeCompany.Get.FreeCompanyNotFound(request.Name, request.World);
            var builder = new DiscordFollowupMessageBuilder().AddError(description);
            await request.Ctx.FollowupAsync(builder);
            return;
        }
        catch (NetStoneException)
        {
            // Search unavailable, Lodestone might be down. Try retrieving fc from cache.
            await RequestFromCacheByNameAsync(request, cancellationToken);
            return;
        }

        await RequestFromSearchResultAsync(request, searchDtos, cancellationToken);
    }

    private async Task RequestFromSearchResultAsync(FreeCompanyGetRequest request,
        ICollection<FreeCompanySearchPageResultDto> searchDtos, CancellationToken cancellationToken)
    {
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components();
        if (request.IsPrivate && searchDtos.Count > 1)
        {
            var select = searchDtos.AsSelectComponent(ComponentIds.Select.FreeCompany);
            builder = builder.AddTieBreakerSelect(select, searchDtos.Count);
            await request.Ctx.FollowupAsync(builder);
        }
        else
        {
            var first = searchDtos.FirstOrDefault(x =>
                x.Name.Equals(request.Name, StringComparison.InvariantCultureIgnoreCase)) ?? searchDtos.First();

            var container = await fcService.CreateContainerAsync(first.Id, cancellationToken: cancellationToken);
            builder.AddContainerComponent(container);

            var crest = await first.CrestLayers.DownloadCrestAsync(httpClient);
            await using var _ = await builder.AddImageAsync(crest, Messages.FileNames.Crest, cancellationToken);

            await request.Ctx.RespondAsync(builder);

            // cache recent search for discord user
            await sender.Send(new OptionHistoryAddRequest(request.Ctx.User.Id, HistoryType.FreeCompany, first.Name),
                cancellationToken);
        }
    }

    private async Task RequestFromCacheByNameAsync(FreeCompanyGetRequest request, CancellationToken cancellationToken)
    {
        FreeCompanyDto freeCompany;
        var builder = new DiscordFollowupMessageBuilder().EnableV2Components();
        try
        {
            freeCompany = await sender.Send(new FreeCompanyGetFreeCompanyByNameRequest(request.Name, request.World),
                cancellationToken);
        }
        catch (NotFoundException)
        {
            var description = Messages.Commands.FreeCompany.Get.FreeCompanyNotFoundInCache(request.Name, request.World);
            builder.AddError(description);
            await request.Ctx.FollowupAsync(builder);
            return;
        }

        var container = await fcService.CreateContainerAsync(freeCompany, cancellationToken);
        builder.AddContainerComponent(container);

        var crest = await freeCompany.CrestLayers.DownloadCrestAsync(httpClient);
        await using var _ = await builder.AddImageAsync(crest, Messages.FileNames.Crest, cancellationToken);

        await request.Ctx.RespondAsync(builder);

        // cache recent search for discord user
        await sender.Send(new OptionHistoryAddRequest(request.Ctx.User.Id, HistoryType.FreeCompany, freeCompany.Name),
            cancellationToken);
    }
}