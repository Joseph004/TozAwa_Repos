using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tozawa.Auth.Svc.Migrations
{
    public partial class addindPropertyToIdentity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastAttemptLogin",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastAttemptLogin",
                schema: "Authorization",
                table: "AspNetUsers");
        }
    }
}
