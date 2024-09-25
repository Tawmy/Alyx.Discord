using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    /// <inheritdoc />
    public partial class RenameCharacterIdToLodestoneId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "character_id",
                table: "characters",
                newName: "lodestone_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "lodestone_id",
                table: "characters",
                newName: "character_id");
        }
    }
}
