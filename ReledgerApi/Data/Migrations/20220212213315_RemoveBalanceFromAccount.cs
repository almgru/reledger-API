using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReledgerApi.Data.Migrations
{
    public partial class RemoveBalanceFromAccount : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Balance",
                table: "Accounts");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Balance",
                table: "Accounts",
                type: "TEXT",
                nullable: false,
                defaultValue: 0m);
        }
    }
}
