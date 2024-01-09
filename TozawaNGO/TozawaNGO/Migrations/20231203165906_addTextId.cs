using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TozawaNGO.Migrations
{
    public partial class AddTextId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comment",
                schema: "Authorization",
                table: "Partners",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "CommentTextId",
                schema: "Authorization",
                table: "Partners",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "DescriptionTextId",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Comment",
                schema: "Authorization",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "CommentTextId",
                schema: "Authorization",
                table: "Partners");

            migrationBuilder.DropColumn(
                name: "DescriptionTextId",
                schema: "Authorization",
                table: "AspNetUsers");
        }
    }
}
