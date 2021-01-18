using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class RenameTransactionAccountProps : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_CreditAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_DebitAccountId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "DebitAccountId",
                table: "Transactions",
                newName: "ToAccountId");

            migrationBuilder.RenameColumn(
                name: "CreditAccountId",
                table: "Transactions",
                newName: "FromAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_DebitAccountId",
                table: "Transactions",
                newName: "IX_Transactions_ToAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_CreditAccountId",
                table: "Transactions",
                newName: "IX_Transactions_FromAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_FromAccountId",
                table: "Transactions",
                column: "FromAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_ToAccountId",
                table: "Transactions",
                column: "ToAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_FromAccountId",
                table: "Transactions");

            migrationBuilder.DropForeignKey(
                name: "FK_Transactions_Accounts_ToAccountId",
                table: "Transactions");

            migrationBuilder.RenameColumn(
                name: "ToAccountId",
                table: "Transactions",
                newName: "DebitAccountId");

            migrationBuilder.RenameColumn(
                name: "FromAccountId",
                table: "Transactions",
                newName: "CreditAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_ToAccountId",
                table: "Transactions",
                newName: "IX_Transactions_DebitAccountId");

            migrationBuilder.RenameIndex(
                name: "IX_Transactions_FromAccountId",
                table: "Transactions",
                newName: "IX_Transactions_CreditAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_CreditAccountId",
                table: "Transactions",
                column: "CreditAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Transactions_Accounts_DebitAccountId",
                table: "Transactions",
                column: "DebitAccountId",
                principalTable: "Accounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
