namespace Alyx.Discord.Core.Records;

public record SheetMetadata(
    string Title,
    TimeSpan Duration,
    DateTime LastUpdated,
    bool FallbackUsed,
    string? FallbackReason);