using MediatR;

namespace Alyx.Discord.Core.Requests.Character.SheetMetadata;

public record CharacterSheetMetadataRequest(string LodestoneId, DateTime SheetTimestamp)
    : IRequest<ICollection<Structs.SheetMetadata>>;