namespace Alyx.Discord.Db.Models;

public class InteractionData
{
    public int Id { get; set; } // PK

    public required string Key { get; set; }

    public required string Value { get; set; }

    public required string Type { get; set; }
}