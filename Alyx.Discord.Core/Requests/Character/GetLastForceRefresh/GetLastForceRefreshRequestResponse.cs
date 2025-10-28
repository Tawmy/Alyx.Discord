namespace Alyx.Discord.Core.Requests.Character.GetLastForceRefresh;

public record GetLastForceRefreshRequestResponse
{
    public DateTime? LastForceRefreshCharacter { get; init; }
    public DateTime? LastForceRefreshFreeCompany { get; init; }
}