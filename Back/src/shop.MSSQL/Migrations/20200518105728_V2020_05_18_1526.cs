using Microsoft.EntityFrameworkCore.Migrations;

namespace shop.DataLayer.MSSQL.Migrations
{
    public partial class V2020_05_18_1526 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonthOfYear",
                table: "Orders",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonthOfYear",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Orders");
        }
    }
}
