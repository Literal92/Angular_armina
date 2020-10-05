using Microsoft.EntityFrameworkCore.Migrations;

namespace shop.DataLayer.MSSQL.Migrations
{
    public partial class V2020_05_20_1853 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "AppUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Instagram",
                table: "AppUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRequest",
                table: "AppUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Picture",
                table: "AppUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Telegram",
                table: "AppUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WebSite",
                table: "AppUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "WhatsApp",
                table: "AppUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "Instagram",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "IsRequest",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "Picture",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "Telegram",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "WebSite",
                table: "AppUsers");

            migrationBuilder.DropColumn(
                name: "WhatsApp",
                table: "AppUsers");
        }
    }
}
