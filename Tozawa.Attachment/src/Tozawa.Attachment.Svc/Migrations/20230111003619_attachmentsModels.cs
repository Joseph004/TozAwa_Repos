using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tozawa.Attachment.Svc.Migrations
{
    public partial class attachmentsModels : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Attachment");

            migrationBuilder.CreateTable(
                name: "ConvertedOwners",
                schema: "Attachment",
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
                name: "FileAttachmentTypes",
                schema: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileAttachmentTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                schema: "Attachment",
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
                    FileAttachmentTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
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
                    table.ForeignKey(
                        name: "FK_FileAttachments_FileAttachmentTypes_FileAttachmentTypeId",
                        column: x => x.FileAttachmentTypeId,
                        principalSchema: "Attachment",
                        principalTable: "FileAttachmentTypes",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ImagesInformation",
                schema: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileAttachmentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Width = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    Height = table.Column<decimal>(type: "decimal(18,4)", nullable: false),
                    EditorBlobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesInformation", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagesInformation_FileAttachments_FileAttachmentId",
                        column: x => x.FileAttachmentId,
                        principalSchema: "Attachment",
                        principalTable: "FileAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OwnerFileAttachments",
                schema: "Attachment",
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
                        principalSchema: "Attachment",
                        principalTable: "FileAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ImageArea",
                schema: "Attachment",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageInformationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DataType = table.Column<int>(type: "int", nullable: false),
                    PathData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageArea", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageArea_ImagesInformation_ImageInformationId",
                        column: x => x.ImageInformationId,
                        principalSchema: "Attachment",
                        principalTable: "ImagesInformation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_BlobId",
                schema: "Attachment",
                table: "FileAttachments",
                column: "BlobId");

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

            migrationBuilder.CreateIndex(
                name: "IX_ImageArea_ImageInformationId",
                schema: "Attachment",
                table: "ImageArea",
                column: "ImageInformationId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagesInformation_FileAttachmentId",
                schema: "Attachment",
                table: "ImagesInformation",
                column: "FileAttachmentId");

            migrationBuilder.CreateIndex(
                name: "IX_OwnerFileAttachments_FileAttachmentId",
                schema: "Attachment",
                table: "OwnerFileAttachments",
                column: "FileAttachmentId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ConvertedOwners",
                schema: "Attachment");

            migrationBuilder.DropTable(
                name: "ImageArea",
                schema: "Attachment");

            migrationBuilder.DropTable(
                name: "OwnerFileAttachments",
                schema: "Attachment");

            migrationBuilder.DropTable(
                name: "ImagesInformation",
                schema: "Attachment");

            migrationBuilder.DropTable(
                name: "FileAttachments",
                schema: "Attachment");

            migrationBuilder.DropTable(
                name: "FileAttachmentTypes",
                schema: "Attachment");
        }
    }
}
