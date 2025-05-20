using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    /// <inheritdoc />
    public partial class v180 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:history_type", "character,free_company")
                .OldAnnotation("Npgsql:Enum:history_type", "character");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "interaction_datas",
                type: "character varying(36)",
                maxLength: 36,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character(32)",
                oldFixedLength: true,
                oldMaxLength: 32);

            #region SQL

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

            #endregion
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:history_type", "character")
                .OldAnnotation("Npgsql:Enum:history_type", "character,free_company");

            migrationBuilder.AlterColumn<string>(
                name: "key",
                table: "interaction_datas",
                type: "character(32)",
                fixedLength: true,
                maxLength: 32,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(36)",
                oldMaxLength: 36);

            #region SQL

            migrationBuilder.Sql(
                """
                UPDATE interaction_datas
                SET type = 'NetStone.Common.DTOs.Character.CharacterDtoV3'
                WHERE type = 'NetStone.Common.DTOs.Character.CharacterDto'
                """);
            
            migrationBuilder.Sql(
                """
                UPDATE interaction_datas
                SET type = 'NetStone.Common.DTOs.FreeCompany.FreeCompanyDtoV3'
                WHERE type = 'NetStone.Common.DTOs.FreeCompany.FreeCompanyDto'
                """);

            #endregion
        }
    }
}
