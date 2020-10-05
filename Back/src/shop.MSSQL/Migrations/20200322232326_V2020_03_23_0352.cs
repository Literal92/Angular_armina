using Microsoft.EntityFrameworkCore.Migrations;

namespace shop.DataLayer.MSSQL.Migrations
{
    public partial class V2020_03_23_0352 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "RefId",
                table: "Orders",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RefId",
                table: "Orders");
        }
    }
}
