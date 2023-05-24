using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tozawa.Language.Svc.Migrations
{
    public partial class newlanguagedatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Language");

            migrationBuilder.CreateTable(
                name: "Languages",
                schema: "Language",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LongName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Languages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SystemTypes",
                schema: "Language",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDefault = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemTypes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "XliffDistributionFiles",
                schema: "Language",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FileState = table.Column<int>(type: "int", nullable: false),
                    SourceLanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TargetLanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    NumberOfTranslations = table.Column<int>(type: "int", nullable: false),
                    NumberOfWordsSentInSourceLanguage = table.Column<int>(type: "int", nullable: true),
                    RequestedDeliveryDate = table.Column<DateTime>(type: "datetime", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_XliffDistributionFiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Language.XliffDistributionFiles_Language.Languages_SourceLanguageId",
                        column: x => x.SourceLanguageId,
                        principalSchema: "Language",
                        principalTable: "Languages",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Language.XliffDistributionFiles_Language.Languages_TargetLanguageId",
                        column: x => x.TargetLanguageId,
                        principalSchema: "Language",
                        principalTable: "Languages",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Translations",
                schema: "Language",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SystemTypeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    XliffState = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    IsOriginalText = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Translations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Translations_Languages_LanguageId",
                        column: x => x.LanguageId,
                        principalSchema: "Language",
                        principalTable: "Languages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Translations_SystemTypes_SystemTypeId",
                        column: x => x.SystemTypeId,
                        principalSchema: "Language",
                        principalTable: "SystemTypes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Translations_LanguageId",
                schema: "Language",
                table: "Translations",
                column: "LanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_Translations_SystemTypeId",
                schema: "Language",
                table: "Translations",
                column: "SystemTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SourceLanguageId",
                schema: "Language",
                table: "XliffDistributionFiles",
                column: "SourceLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_TargetLanguageId",
                schema: "Language",
                table: "XliffDistributionFiles",
                column: "TargetLanguageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Translations",
                schema: "Language");

            migrationBuilder.DropTable(
                name: "XliffDistributionFiles",
                schema: "Language");

            migrationBuilder.DropTable(
                name: "SystemTypes",
                schema: "Language");

            migrationBuilder.DropTable(
                name: "Languages",
                schema: "Language");
        }
    }
}
