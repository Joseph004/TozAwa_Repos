using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrleansHost.Migrations
{
    public partial class RemovePwdHah : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                schema: "Authorization",
                table: "UserHashPwds");

            migrationBuilder.AddColumn<string>(
                name: "UserPasswordHash",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserPasswordHash",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                schema: "Authorization",
                table: "UserHashPwds",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
