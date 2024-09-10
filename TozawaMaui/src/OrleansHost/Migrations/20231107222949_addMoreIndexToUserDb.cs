using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrleansHost.Migrations
{
    public partial class AddMoreIndexToUserDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Id_UserId",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Id_UserId_Email",
                schema: "Authorization",
                table: "AspNetUsers",
                columns: new[] { "Id", "UserId", "Email" },
                unique: true,
                filter: "[Email] IS NOT NULL");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_Id_UserId_Email",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_Id_UserId",
                schema: "Authorization",
                table: "AspNetUsers",
                columns: new[] { "Id", "UserId" },
                unique: true);
        }
    }
}
