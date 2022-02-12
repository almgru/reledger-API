using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReledgerApi.Data.Migrations
{
    public partial class RenameTransactionDateToDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Transactions",
                newName: "DateTime");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DateTime",
                table: "Transactions",
                newName: "Date");
        }
    }
}
