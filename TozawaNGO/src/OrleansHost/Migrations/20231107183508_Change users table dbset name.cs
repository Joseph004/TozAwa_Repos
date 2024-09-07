using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrleansHost.Migrations
{
    public partial class Changeuserstabledbsetname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userHashPwds_AspNetUsers_UserId",
                schema: "Authorization",
                table: "userHashPwds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userHashPwds",
                schema: "Authorization",
                table: "userHashPwds");

            migrationBuilder.RenameTable(
                name: "userHashPwds",
                schema: "Authorization",
                newName: "UserHashPwds",
                newSchema: "Authorization");

            migrationBuilder.RenameIndex(
                name: "IX_userHashPwds_UserId",
                schema: "Authorization",
                table: "UserHashPwds",
                newName: "IX_UserHashPwds_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserHashPwds",
                schema: "Authorization",
                table: "UserHashPwds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserHashPwds_AspNetUsers_UserId",
                schema: "Authorization",
                table: "UserHashPwds",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "AspNetUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserHashPwds_AspNetUsers_UserId",
                schema: "Authorization",
                table: "UserHashPwds");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserHashPwds",
                schema: "Authorization",
                table: "UserHashPwds");

            migrationBuilder.RenameTable(
                name: "UserHashPwds",
                schema: "Authorization",
                newName: "userHashPwds",
                newSchema: "Authorization");

            migrationBuilder.RenameIndex(
                name: "IX_UserHashPwds_UserId",
                schema: "Authorization",
                table: "userHashPwds",
                newName: "IX_userHashPwds_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userHashPwds",
                schema: "Authorization",
                table: "userHashPwds",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_userHashPwds_AspNetUsers_UserId",
                schema: "Authorization",
                table: "userHashPwds",
                column: "UserId",
                principalSchema: "Authorization",
                principalTable: "AspNetUsers",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
