namespace Alyx.Discord.Db.Models;

public class MainCharacter
{
    public int Id { get; set; } // PK

    public required ulong DiscordId { get; set; }

    public required string CharacterId { get; set; }

    public required string Code { get; set; }

    public bool Confirmed { get; set; }
}