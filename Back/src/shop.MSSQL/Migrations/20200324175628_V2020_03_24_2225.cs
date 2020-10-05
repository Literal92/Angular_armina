using Microsoft.EntityFrameworkCore.Migrations;

namespace shop.DataLayer.MSSQL.Migrations
{
    public partial class V2020_03_24_2225 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ResellerPrice",
                table: "OptionColors",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ResellerPrice",
                table: "OptionColors");
        }
    }
}
