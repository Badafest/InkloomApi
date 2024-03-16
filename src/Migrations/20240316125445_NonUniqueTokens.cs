using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InkloomApi.Migrations
{
    /// <inheritdoc />
    public partial class NonUniqueTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_UserId_Type",
                table: "Tokens");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId_Type",
                table: "Tokens",
                columns: new[] { "UserId", "Type" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tokens_UserId_Type",
                table: "Tokens");

            migrationBuilder.CreateIndex(
                name: "IX_Tokens_UserId_Type",
                table: "Tokens",
                columns: new[] { "UserId", "Type" },
                unique: true);
        }
    }
}
