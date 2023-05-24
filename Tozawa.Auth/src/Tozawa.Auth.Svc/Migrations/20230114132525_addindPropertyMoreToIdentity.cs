using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tozawa.Auth.Svc.Migrations
{
    public partial class addindPropertyMoreToIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LastLoginCity",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastLoginState",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLoginCity",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "LastLoginState",
                schema: "Authorization",
                table: "AspNetUsers");
        }
    }
}
