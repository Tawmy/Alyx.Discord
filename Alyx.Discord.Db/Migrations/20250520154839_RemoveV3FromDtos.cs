using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    /// <inheritdoc />
    public partial class RemoveV3FromDtos : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE interaction_datas
                SET type = 'NetStone.Common.DTOs.Character.CharacterDto'
                WHERE type = 'NetStone.Common.DTOs.Character.CharacterDtoV3'
                """);
            
            migrationBuilder.Sql(
                """
                UPDATE interaction_datas
                SET type = 'NetStone.Common.DTOs.FreeCompany.FreeCompanyDto'
                WHERE type = 'NetStone.Common.DTOs.FreeCompany.FreeCompanyDtoV3'
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                UPDATE interaction_datas
                SET type = 'NetStone.Common.DTOs.Character.CharacterDtoV3'
                WHERE type = 'NetStone.Common.DTOs.Character.CharacterDto'
                """);
            
            migrationBuilder.Sql(
                """
                UPDATE interaction_datas
                SET type = 'NetStone.Common.DTOs.Character.FreeCompanyDtoV3'
                WHERE type = 'NetStone.Common.DTOs.Character.FreeCompanyDto'
                """);
        }
    }
}
