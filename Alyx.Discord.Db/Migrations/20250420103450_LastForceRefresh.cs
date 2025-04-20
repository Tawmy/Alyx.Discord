using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Alyx.Discord.Db.Migrations
{
    /// <inheritdoc />
    public partial class LastForceRefresh : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_force_refresh",
                table: "characters",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "last_force_refresh",
                table: "characters");
        }
    }
}
