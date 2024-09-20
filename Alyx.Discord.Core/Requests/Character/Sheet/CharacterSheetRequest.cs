using MediatR;
using SixLabors.ImageSharp;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

public record CharacterSheetRequest(string LodestoneId) : IRequest<Image>;