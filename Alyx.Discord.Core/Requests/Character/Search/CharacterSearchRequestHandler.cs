using MediatR;
using NetStone.Api.Client;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Exceptions;
using NetStone.Common.Queries;

namespace Alyx.Discord.Core.Requests.Character.Search;

public class CharacterSearchRequestHandler(NetStoneApiClient client)
    : IRequestHandler<CharacterSearchRequest, ICollection<CharacterSearchPageResultDto>>
{
    public async Task<ICollection<CharacterSearchPageResultDto>> Handle(CharacterSearchRequest request,
        CancellationToken cancellationToken)
    {
        var query = new CharacterSearchQuery(request.Name, request.World);
        var searchPage = await client.Character.SearchAsync(query);
        if (!searchPage.HasResults)
        {
            throw new NotFoundException();
        }

        return searchPage.Results.ToArray();
    }
}