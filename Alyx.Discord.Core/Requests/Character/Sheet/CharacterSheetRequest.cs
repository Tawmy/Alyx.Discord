using MediatR;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

public record CharacterSheetRequest(string LodestoneId) : IRequest;