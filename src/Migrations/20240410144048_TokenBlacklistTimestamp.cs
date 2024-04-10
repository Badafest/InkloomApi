using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InkloomApi.Migrations
{
    /// <inheritdoc />
    public partial class TokenBlacklistTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "TokenBlacklistTimestamp",
                table: "Users",
                type: "timestamp with time zone",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TokenBlacklistTimestamp",
                table: "Users");
        }
    }
}
