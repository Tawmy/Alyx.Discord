namespace Alyx.Discord.Core.Structs;

public readonly record struct SheetMetadata(
    string Title,
    TimeSpan Duration,
    DateTime LastUpdated,
    bool FallbackUsed,
    string? FallbackReason);