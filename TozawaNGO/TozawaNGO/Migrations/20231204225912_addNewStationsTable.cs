using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TozawaNGO.Migrations
{
    public partial class AddNewStationsTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Translation",
                schema: "Authorization",
                table: "Translation");

            migrationBuilder.RenameTable(
                name: "Translation",
                schema: "Authorization",
                newName: "Translations",
                newSchema: "Authorization");

            migrationBuilder.AddColumn<string>(
                name: "StationIds",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translations",
                schema: "Authorization",
                table: "Translations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ConvertedOwners",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConvertedOwners", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BlobId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    MiniatureId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Extension = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MimeType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Size = table.Column<double>(type: "float", nullable: false),
                    AttachmentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileAttachmentType = table.Column<int>(type: "int", nullable: false),
                    MetaData = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Stations",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    Adresse = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OwnerFileAttachments",
                schema: "Authorization",
                columns: table => new
                {
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OwnerFileAttachments", x => new { x.OwnerId, x.FileAttachmentId });
                    table.ForeignKey(
                        name: "FK_OwnerFileAttachments_FileAttachments_FileAttachmentId",
                        column: x => x.FileAttachmentId,
                        principalSchema: "Authorization",
                        principalTable: "FileAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Establishments",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    COO = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Establishments_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "Authorization",
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Reports",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Reports_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "Authorization",
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Establishments_StationId",
                schema: "Authorization",
                table: "Establishments",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_BlobId",
                schema: "Authorization",
                table: "FileAttachments",
                column: "BlobId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerFileAttachments_FileAttachmentId",
                schema: "Authorization",
                table: "OwnerFileAttachments",
                column: "FileAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Reports_StationId",
                schema: "Authorization",
                table: "Reports",
                column: "StationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertedOwners",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Establishments",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "OwnerFileAttachments",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Reports",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "FileAttachments",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Stations",
                schema: "Authorization");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Translations",
                schema: "Authorization",
                table: "Translations");

            migrationBuilder.DropColumn(
                name: "StationIds",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.RenameTable(
                name: "Translations",
                schema: "Authorization",
                newName: "Translation",
                newSchema: "Authorization");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Translation",
                schema: "Authorization",
                table: "Translation",
                column: "Id");
        }
    }
}
