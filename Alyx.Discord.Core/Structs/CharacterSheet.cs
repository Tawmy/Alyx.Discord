using SixLabors.ImageSharp;

namespace Alyx.Discord.Core.Structs;

public readonly record struct CharacterSheet(
    Image Image,
    IEnumerable<SheetMetadata> SheetMetadata,
    bool MinionsPublic,
    bool MountsPublic);