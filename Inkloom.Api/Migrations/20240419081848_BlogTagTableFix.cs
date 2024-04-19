using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inkloom.Api.Migrations
{
    /// <inheritdoc />
    public partial class BlogTagTableFix : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTag_Blogs_BlogsId",
                table: "BlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTag_Tags_TagsId",
                table: "BlogTag");

            migrationBuilder.DropTable(
                name: "BlogTags");

            migrationBuilder.RenameColumn(
                name: "TagsId",
                table: "BlogTag",
                newName: "TagId");

            migrationBuilder.RenameColumn(
                name: "BlogsId",
                table: "BlogTag",
                newName: "BlogId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTag_TagsId",
                table: "BlogTag",
                newName: "IX_BlogTag_TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTag_Blogs_BlogId",
                table: "BlogTag",
                column: "BlogId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTag_Tags_TagId",
                table: "BlogTag",
                column: "TagId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogTag_Blogs_BlogId",
                table: "BlogTag");

            migrationBuilder.DropForeignKey(
                name: "FK_BlogTag_Tags_TagId",
                table: "BlogTag");

            migrationBuilder.RenameColumn(
                name: "TagId",
                table: "BlogTag",
                newName: "TagsId");

            migrationBuilder.RenameColumn(
                name: "BlogId",
                table: "BlogTag",
                newName: "BlogsId");

            migrationBuilder.RenameIndex(
                name: "IX_BlogTag_TagId",
                table: "BlogTag",
                newName: "IX_BlogTag_TagsId");

            migrationBuilder.CreateTable(
                name: "BlogTags",
                columns: table => new
                {
                    BlogId = table.Column<int>(type: "integer", nullable: false),
                    TagId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogTags", x => new { x.BlogId, x.TagId });
                    table.ForeignKey(
                        name: "FK_BlogTags_Blogs_BlogId",
                        column: x => x.BlogId,
                        principalTable: "Blogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BlogTags_Tags_TagId",
                        column: x => x.TagId,
                        principalTable: "Tags",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogTags_TagId",
                table: "BlogTags",
                column: "TagId");

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTag_Blogs_BlogsId",
                table: "BlogTag",
                column: "BlogsId",
                principalTable: "Blogs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogTag_Tags_TagsId",
                table: "BlogTag",
                column: "TagsId",
                principalTable: "Tags",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
