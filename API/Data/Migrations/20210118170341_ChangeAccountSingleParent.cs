using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class ChangeAccountSingleParent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_AccountId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "AccountId",
                table: "Accounts",
                newName: "ParentId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_AccountId",
                table: "Accounts",
                newName: "IX_Accounts_ParentId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_ParentId",
                table: "Accounts",
                column: "ParentId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Accounts_Accounts_ParentId",
                table: "Accounts");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Accounts",
                newName: "AccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Accounts_ParentId",
                table: "Accounts",
                newName: "IX_Accounts_AccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Accounts_Accounts_AccountId",
                table: "Accounts",
                column: "AccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
