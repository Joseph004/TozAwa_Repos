using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class organizationandfeaturestables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                schema: "Authorization",
                table: "UserLogs",
                newName: "OrganizationName");

            migrationBuilder.RenameColumn(
                name: "Roles",
                schema: "Authorization",
                table: "AspNetUsers",
                newName: "Comment");

            migrationBuilder.AddColumn<Guid>(
                name: "DescriptionTextId",
                schema: "Authorization",
                table: "UserLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                schema: "Authorization",
                table: "UserLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FeatureName",
                schema: "Authorization",
                table: "UserLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifiedDate",
                schema: "Authorization",
                table: "UserLogs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "ModifieddBy",
                schema: "Authorization",
                table: "UserLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TextId",
                schema: "Authorization",
                table: "UserLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "Authorization",
                table: "Stations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "CommentTextId",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommentTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StationAddresses",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    StationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StationAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StationAddresses_Stations_StationId",
                        column: x => x.StationId,
                        principalSchema: "Authorization",
                        principalTable: "Stations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TozawaFeatures",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false),
                    TextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DescriptionTextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Deleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TozawaFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserAddresses",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserAddresses_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationAddresses",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    City = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    State = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ZipCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationAddresses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationAddresses_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationFeatures",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Feature = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationFeatures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationFeatures_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TzRoles",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleEnum = table.Column<int>(type: "int", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TzRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TzRoles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserOrganization",
                schema: "Authorization",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserOrganization", x => new { x.OrganizationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_UserOrganization_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserOrganization_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Functions",
                schema: "Authorization",
                columns: table => new
                {
                    Functiontype = table.Column<int>(type: "int", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functions", x => new { x.Functiontype, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Functions_TzRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Authorization",
                        principalTable: "TzRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TzUserRoles",
                schema: "Authorization",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TzUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_TzUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TzUserRoles_TzRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Authorization",
                        principalTable: "TzRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "TozawaFeatures",
                columns: new[] { "Id", "Deleted", "DescriptionTextId", "TextId" },
                values: new object[] { 1, false, new Guid("97617538-8931-43ee-bd4c-769726bdb6a4"), new Guid("acd1ef02-0da3-474b-95d3-8861fcc8e368") });

            migrationBuilder.CreateIndex(
                name: "IX_Stations_OrganizationId",
                schema: "Authorization",
                table: "Stations",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_RoleId",
                schema: "Authorization",
                table: "Functions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationAddresses_OrganizationId",
                schema: "Authorization",
                table: "OrganizationAddresses",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationFeatures_OrganizationId",
                schema: "Authorization",
                table: "OrganizationFeatures",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_StationAddresses_StationId",
                schema: "Authorization",
                table: "StationAddresses",
                column: "StationId");

            migrationBuilder.CreateIndex(
                name: "IX_TzRoles_OrganizationId",
                schema: "Authorization",
                table: "TzRoles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_TzUserRoles_RoleId",
                schema: "Authorization",
                table: "TzUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserAddresses_UserId",
                schema: "Authorization",
                table: "UserAddresses",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganization_UserId",
                schema: "Authorization",
                table: "UserOrganization",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers",
                column: "OrganizationId",
                principalSchema: "Authorization",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Stations_Organizations_OrganizationId",
                schema: "Authorization",
                table: "Stations",
                column: "OrganizationId",
                principalSchema: "Authorization",
                principalTable: "Organizations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Organizations_OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Stations_Organizations_OrganizationId",
                schema: "Authorization",
                table: "Stations");

            migrationBuilder.DropTable(
                name: "Functions",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "OrganizationAddresses",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "OrganizationFeatures",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "StationAddresses",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "TozawaFeatures",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "TzUserRoles",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "UserAddresses",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "UserOrganization",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "TzRoles",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "Authorization");

            migrationBuilder.DropIndex(
                name: "IX_Stations_OrganizationId",
                schema: "Authorization",
                table: "Stations");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "DescriptionTextId",
                schema: "Authorization",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "Email",
                schema: "Authorization",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "FeatureName",
                schema: "Authorization",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "ModifiedDate",
                schema: "Authorization",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "ModifieddBy",
                schema: "Authorization",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "TextId",
                schema: "Authorization",
                table: "UserLogs");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "Authorization",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "CommentTextId",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "OrganizationName",
                schema: "Authorization",
                table: "UserLogs",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "Comment",
                schema: "Authorization",
                table: "AspNetUsers",
                newName: "Roles");
        }
    }
}
