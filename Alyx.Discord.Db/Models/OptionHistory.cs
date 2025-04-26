namespace Alyx.Discord.Db.Models;

public class OptionHistory
{
    public int Id { get; set; } // PK

    public required ulong DiscordId { get; set; }

    public required HistoryType HistoryType { get; set; }

    public required string Value { get; set; }

    public required DateTime LastUsed { get; set; }
}

public enum HistoryType
{
    Character
}