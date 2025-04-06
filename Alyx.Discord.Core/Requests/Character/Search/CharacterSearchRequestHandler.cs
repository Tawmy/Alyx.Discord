using MediatR;
using NetStone.Api.Sdk.Abstractions;
using NetStone.Common.DTOs.Character;
using NetStone.Common.Queries;

namespace Alyx.Discord.Core.Requests.Character.Search;

public class CharacterSearchRequestHandler(INetStoneApiCharacter apiCharacter)
    : IRequestHandler<CharacterSearchRequest, ICollection<CharacterSearchPageResultDto>>
{
    public async Task<ICollection<CharacterSearchPageResultDto>> Handle(CharacterSearchRequest request,
        CancellationToken cancellationToken)
    {
        var query = new CharacterSearchQuery(request.Name, request.World);
        var searchPage = await apiCharacter.SearchAsync(query, cancellationToken: cancellationToken);
        return searchPage.Results.ToArray();
    }
}