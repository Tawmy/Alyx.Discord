namespace Alyx.Discord.Core.Structs;

public readonly record struct SheetMetadata(
    string Title,
    DateTime LastUpdated,
    bool FallbackUsed,
    string? FallbackReason);