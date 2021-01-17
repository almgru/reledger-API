using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Data.Migrations
{
    public partial class SeededAccounts : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountId", "Name" },
                values: new object[] { 1, null, "Asset" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountId", "Name" },
                values: new object[] { 2, null, "Liability" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountId", "Name" },
                values: new object[] { 3, null, "Income" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountId", "Name" },
                values: new object[] { 4, null, "Expense" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "AccountId", "Name" },
                values: new object[] { 5, null, "Capital" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5);
        }
    }
}
