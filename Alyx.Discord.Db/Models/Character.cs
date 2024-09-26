namespace Alyx.Discord.Db.Models;

public class Character
{
    public int Id { get; set; } // PK

    public required ulong DiscordId { get; set; }

    public required string LodestoneId { get; set; }

    public required string Code { get; set; }

    public bool Confirmed { get; set; }

    public bool IsMainCharacter { get; set; }
}