using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReledgerApi.Data.Migrations
{
    public partial class RemoveIncreaseBalanceOnFromAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DropColumn(
                name: "IncreaseBalanceOn",
                table: "Accounts");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Checking");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "Expenses");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Savings");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IncreaseBalanceOn",
                table: "Accounts",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Asset");

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "IncreaseBalanceOn", "Name" },
                values: new object[] { 1, "Liability" });

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "IncreaseBalanceOn", "Name" },
                values: new object[] { 1, "Income" });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Balance", "IncreaseBalanceOn", "Name", "ParentId" },
                values: new object[] { 4, 0m, 0, "Expense", null });

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "Balance", "IncreaseBalanceOn", "Name", "ParentId" },
                values: new object[] { 5, 0m, 1, "Capital", null });
        }
    }
}
