using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    /// <inheritdoc />
    public partial class HistoryTypeFreeCompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:history_type", "character,free_company")
                .OldAnnotation("Npgsql:Enum:history_type", "character");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:Enum:history_type", "character")
                .OldAnnotation("Npgsql:Enum:history_type", "character,free_company");
        }
    }
}
