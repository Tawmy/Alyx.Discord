using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    /// <inheritdoc />
    public partial class IsMainCharacterProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "character_claims");

            migrationBuilder.CreateTable(
                name: "characters",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    discord_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false),
                    character_id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    code = table.Column<string>(type: "character(32)", fixedLength: true, maxLength: 32, nullable: false),
                    confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    is_main_character = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_characters", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_characters_code",
                table: "characters",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_characters_discord_id",
                table: "characters",
                column: "discord_id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "characters");

            migrationBuilder.CreateTable(
                name: "character_claims",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityAlwaysColumn),
                    character_id = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    code = table.Column<string>(type: "character(32)", fixedLength: true, maxLength: 32, nullable: false),
                    confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    discord_id = table.Column<decimal>(type: "numeric(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_character_claims", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_character_claims_code",
                table: "character_claims",
                column: "code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_character_claims_discord_id",
                table: "character_claims",
                column: "discord_id",
                unique: true);
        }
    }
}
