using MediatR;

namespace Alyx.Discord.Core.Requests.Character.Search;

public class CharacterSearchRequestHandler : IRequestHandler<CharacterSearchRequest, string>
{
    public Task<string> Handle(CharacterSearchRequest request, CancellationToken cancellationToken)
    {
        return Task.FromResult($"{request.Name} from {request.World}");
    }
}