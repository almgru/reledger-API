using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class AddAppUserEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Transactions",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Tags",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Attachments",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "AppUserId",
                table: "Accounts",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AppUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: true),
                    PasswordHash = table.Column<byte[]>(type: "BLOB", nullable: true),
                    PasswordSalt = table.Column<byte[]>(type: "BLOB", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppUsers", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AppUserId",
                table: "Transactions",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tags_AppUserId",
                table: "Tags",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Attachments_AppUserId",
                table: "Attachments",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Accounts_AppUserId",
                table: "Accounts",
                column: "AppUserId");

            migrationBuilder.CreateIndex(
                name: "IX_AppUsers_UserName",
                table: "AppUsers",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_AppUsers_AppUserId",
                table: "Accounts",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Attachments_AppUsers_AppUserId",
                table: "Attachments",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tags_AppUsers_AppUserId",
                table: "Tags",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_AppUsers_AppUserId",
                table: "Transactions",
                column: "AppUserId",
                principalTable: "AppUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_AppUsers_AppUserId",
                table: "Accounts");

            migrationBuilder.DropForeignKey(
                name: "FK_Attachments_AppUsers_AppUserId",
                table: "Attachments");

            migrationBuilder.DropForeignKey(
                name: "FK_Tags_AppUsers_AppUserId",
                table: "Tags");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_AppUsers_AppUserId",
                table: "Transactions");

            migrationBuilder.DropTable(
                name: "AppUsers");

            migrationBuilder.DropIndex(
                name: "IX_Transactions_AppUserId",
                table: "Transactions");

            migrationBuilder.DropIndex(
                name: "IX_Tags_AppUserId",
                table: "Tags");

            migrationBuilder.DropIndex(
                name: "IX_Attachments_AppUserId",
                table: "Attachments");

            migrationBuilder.DropIndex(
                name: "IX_Accounts_AppUserId",
                table: "Accounts");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Tags");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Attachments");

            migrationBuilder.DropColumn(
                name: "AppUserId",
                table: "Accounts");
        }
    }
}
