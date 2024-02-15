using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TozawaNGO.Migrations
{
    public partial class AddPwdHah : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PasswordSalt",
                schema: "Authorization",
                table: "UserHashPwds",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordSalt",
                schema: "Authorization",
                table: "UserHashPwds");
        }
    }
}
