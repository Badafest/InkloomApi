using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inkloom.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RenameDescriptionToSubtitle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Blogs",
                newName: "Subtitle");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Subtitle",
                table: "Blogs",
                newName: "Description");
        }
    }
}
