using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;

namespace Alyx.Discord.Core.Records;

public class SheetCache
{
    public required bool MountsPublic { get; init; }
    public required bool MinionsPublic { get; init; }
    public required CharacterDto Character { get; init; }
    public required CharacterClassJobOuterDto ClassJobs { get; init; }
    public required FreeCompanyDto? FreeCompany { get; init; }
}