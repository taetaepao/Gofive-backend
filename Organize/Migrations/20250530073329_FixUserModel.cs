using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Organize.Migrations
{
    /// <inheritdoc />
    public partial class FixUserModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_Permissions_PermissionId",
                table: "Documents");

            migrationBuilder.DropIndex(
                name: "IX_Documents_PermissionId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "PermissionId",
                table: "Documents");

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UserName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "Users");

            migrationBuilder.AddColumn<Guid>(
                name: "PermissionId",
                table: "Documents",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PermissionId",
                table: "Documents",
                column: "PermissionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_Permissions_PermissionId",
                table: "Documents",
                column: "PermissionId",
                principalTable: "Permissions",
                principalColumn: "Id");
        }
    }
}
