using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrleansHost.Migrations
{
    public partial class RemoveKeyOnUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Id_UserId",
                schema: "Authorization",
                table: "AspNetUsers",
                columns: new[] { "Id", "UserId" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Id_UserId",
                schema: "Authorization",
                table: "AspNetUsers");
        }
    }
}
