using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Inkloom.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveChangeTrackUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "DeletedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "UpdatedDate",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "CreatedById",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "DeletedById",
                table: "Blogs");

            migrationBuilder.DropColumn(
                name: "UpdatedById",
                table: "Blogs");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Tags",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_Name",
                table: "Tags",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tags_Name",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Tags");

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedDate",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Tags",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedDate",
                table: "Tags",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "CreatedById",
                table: "Blogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DeletedById",
                table: "Blogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UpdatedById",
                table: "Blogs",
                type: "integer",
                nullable: true);

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
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_DeletedById",
                table: "Blogs",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blogs_Users_UpdatedById",
                table: "Blogs",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_CreatedById",
                table: "Tags",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_DeletedById",
                table: "Tags",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_Users_UpdatedById",
                table: "Tags",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_CreatedById",
                table: "Users",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_DeletedById",
                table: "Users",
                column: "DeletedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Users_UpdatedById",
                table: "Users",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
