using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inkloom.Api.Migrations
{
    /// <inheritdoc />
    public partial class UserRefInChangesMadeBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Users",
                newName: "UpdatedById");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "Users",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Users",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Tags",
                newName: "UpdatedById");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "Tags",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Tags",
                newName: "CreatedById");

            migrationBuilder.RenameColumn(
                name: "UpdatedBy",
                table: "Blogs",
                newName: "UpdatedById");

            migrationBuilder.RenameColumn(
                name: "DeletedBy",
                table: "Blogs",
                newName: "DeletedById");

            migrationBuilder.RenameColumn(
                name: "CreatedBy",
                table: "Blogs",
                newName: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CreatedById",
                table: "Users",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DeletedById",
                table: "Users",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UpdatedById",
                table: "Users",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_DeletedById",
                table: "Tags",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_UpdatedById",
                table: "Tags",
                column: "UpdatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_CreatedById",
                table: "Blogs",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_DeletedById",
                table: "Blogs",
                column: "DeletedById");

            migrationBuilder.CreateIndex(
                name: "IX_Blogs_UpdatedById",
                table: "Blogs",
                column: "UpdatedById");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_CreatedById",
                table: "Blogs",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_DeletedById",
                table: "Blogs",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_UpdatedById",
                table: "Blogs",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_CreatedById",
                table: "Tags",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_DeletedById",
                table: "Tags",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_UpdatedById",
                table: "Tags",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DeletedById",
                table: "Users",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UpdatedById",
                table: "Users",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_CreatedById",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_DeletedById",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Blogs_Users_UpdatedById",
                table: "Blogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_CreatedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_DeletedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_Users_UpdatedById",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_DeletedById",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Users_UpdatedById",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_CreatedById",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DeletedById",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UpdatedById",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Tags_CreatedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_DeletedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Tags_UpdatedById",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_CreatedById",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_DeletedById",
                table: "Blogs");

            migrationBuilder.DropIndex(
                name: "IX_Blogs_UpdatedById",
                table: "Blogs");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "Users",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "Users",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Users",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "Tags",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "Tags",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Tags",
                newName: "CreatedBy");

            migrationBuilder.RenameColumn(
                name: "UpdatedById",
                table: "Blogs",
                newName: "UpdatedBy");

            migrationBuilder.RenameColumn(
                name: "DeletedById",
                table: "Blogs",
                newName: "DeletedBy");

            migrationBuilder.RenameColumn(
                name: "CreatedById",
                table: "Blogs",
                newName: "CreatedBy");
        }
    }
}
