using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inkloom.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class TokenExpiry : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Expiry",
                table: "Tokens",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Expiry",
                table: "Tokens");
        }
    }
}
