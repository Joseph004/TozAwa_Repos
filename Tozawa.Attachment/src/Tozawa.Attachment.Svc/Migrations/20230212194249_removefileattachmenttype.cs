using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tozawa.Attachment.Svc.Migrations
{
    public partial class removefileattachmenttype : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FileAttachments_FileAttachmentTypes_FileAttachmentTypeId",
                schema: "Attachment",
                table: "FileAttachments");

            migrationBuilder.DropTable(
                name: "FileAttachmentTypes",
                schema: "Attachment");

            migrationBuilder.DropIndex(
                name: "IX_FileAttachments_FileAttachmentTypeId",
                schema: "Attachment",
                table: "FileAttachments");

            migrationBuilder.DropColumn(
                name: "FileAttachmentTypeId",
                schema: "Attachment",
                table: "FileAttachments");

            migrationBuilder.AddColumn<int>(
                name: "FileAttachmentType",
                schema: "Attachment",
                table: "FileAttachments",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileAttachmentType",
                schema: "Attachment",
                table: "FileAttachments");

            migrationBuilder.AddColumn<Guid>(
                name: "FileAttachmentTypeId",
                schema: "Attachment",
                table: "FileAttachments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FileAttachmentTypes",
                schema: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_FileAttachmentTypeId",
                schema: "Attachment",
                table: "FileAttachments",
                column: "FileAttachmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachmentTypes_Name",
                schema: "Attachment",
                table: "FileAttachmentTypes",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_FileAttachments_FileAttachmentTypes_FileAttachmentTypeId",
                schema: "Attachment",
                table: "FileAttachments",
                column: "FileAttachmentTypeId",
                principalSchema: "Attachment",
                principalTable: "FileAttachmentTypes",
                principalColumn: "Id");
        }
    }
}
