using Alyx.Discord.Core.Structs;
using NetStone.Common.DTOs.Character;
using NetStone.Common.DTOs.FreeCompany;
using SixLabors.ImageSharp;

namespace Alyx.Discord.Core.Requests.Character.Sheet;

public record CharacterSheetResponse(
    Image Image,
    IEnumerable<SheetMetadata> SheetMetadata,
    bool MinionsPublic,
    bool MountsPublic,
    CharacterDtoV3 Character,
    FreeCompanyDtoV3? FreeCompany);