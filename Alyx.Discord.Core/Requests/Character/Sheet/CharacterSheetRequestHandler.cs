using MediatR;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

public class CharacterSheetRequestHandler : IRequestHandler<CharacterSheetRequest>
{
    public Task Handle(CharacterSheetRequest request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}