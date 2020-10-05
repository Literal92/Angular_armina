using Microsoft.EntityFrameworkCore.Migrations;

namespace shop.DataLayer.MSSQL.Migrations
{
    public partial class V2020_03_26_2126 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TotalForReseller",
                table: "OrderProducts",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalForReseller",
                table: "OrderProducts");
        }
    }
}
