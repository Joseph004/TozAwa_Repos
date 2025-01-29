using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class updateusertenantlandlord : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 3, new Guid("a78379a8-72df-4885-b3f0-62571088524b") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 4, new Guid("a78379a8-72df-4885-b3f0-62571088524b") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 7, new Guid("e91ea6b8-3e15-4ea9-bb17-dd60e09d9060") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 8, new Guid("e91ea6b8-3e15-4ea9-bb17-dd60e09d9060") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 9, new Guid("4d4e49eb-1099-457a-87ce-0846da6964c2") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 10, new Guid("4d4e49eb-1099-457a-87ce-0846da6964c2") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("d1497680-6be8-4e81-a781-2ab1e5c7b80d"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("4d4e49eb-1099-457a-87ce-0846da6964c2"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("a78379a8-72df-4885-b3f0-62571088524b"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("e91ea6b8-3e15-4ea9-bb17-dd60e09d9060"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("aa2dd4d7-001a-479a-9da5-495e8902f710"));

            migrationBuilder.DropColumn(
                name: "LandLords",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Tenants",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "Commun",
                schema: "Authorization",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Commun",
                schema: "Authorization",
                table: "Stations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Commun",
                schema: "Authorization",
                table: "StationAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Commun",
                schema: "Authorization",
                table: "OrganizationAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "UserLandLords",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserLandLord_LandLordUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    UserLandLord_TenantUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLandLords", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserLandLords_AspNetUsers_UserLandLord_LandLordUserId",
                        column: x => x.UserLandLord_LandLordUserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserLandLords_AspNetUsers_UserLandLord_TenantUserId",
                        column: x => x.UserLandLord_TenantUserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTenants",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserTenant_LandLordUserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserTenant_TenantUserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTenants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTenants_AspNetUsers_UserTenant_LandLordUserId",
                        column: x => x.UserTenant_LandLordUserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTenants_AspNetUsers_UserTenant_TenantUserId",
                        column: x => x.UserTenant_TenantUserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Organizations",
                columns: new[] { "Id", "City", "Comment", "CommentTextId", "Country", "CreateDate", "CreatedBy", "Deleted", "Description", "DescriptionTextId", "Email", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "State" },
                values: new object[,]
                {
                    { new Guid("1c909244-0e96-422d-9a76-28c4ac9db8b9"), "Kinshasa", null, new Guid("00000000-0000-0000-0000-000000000000"), "COD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "tozawa_kinlabelle@gmail.com", "", null, "KinLaBelle", null, null },
                    { new Guid("5f2f2989-215c-4908-9714-708071d166e6"), "World", null, new Guid("00000000-0000-0000-0000-000000000000"), "World", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "NoOrganization", "", null, "NoOrganization", null, null }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "TzRoles",
                columns: new[] { "Id", "OrganizationId", "RoleEnum" },
                values: new object[,]
                {
                    { new Guid("222d3077-4d13-4733-8688-57ff43091d69"), new Guid("1c909244-0e96-422d-9a76-28c4ac9db8b9"), 2 },
                    { new Guid("59651122-b13e-42bd-babb-1f62a2a238c8"), new Guid("1c909244-0e96-422d-9a76-28c4ac9db8b9"), 1 },
                    { new Guid("8c8812cb-ecb7-4a5a-a97d-309228011c74"), new Guid("1c909244-0e96-422d-9a76-28c4ac9db8b9"), 5 }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Functions",
                columns: new[] { "FunctionType", "RoleId", "CreateDate", "CreatedBy", "ModifiedBy", "ModifiedDate" },
                values: new object[,]
                {
                    { 3, new Guid("8c8812cb-ecb7-4a5a-a97d-309228011c74"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 4, new Guid("8c8812cb-ecb7-4a5a-a97d-309228011c74"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 7, new Guid("59651122-b13e-42bd-babb-1f62a2a238c8"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 8, new Guid("59651122-b13e-42bd-babb-1f62a2a238c8"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 9, new Guid("222d3077-4d13-4733-8688-57ff43091d69"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 10, new Guid("222d3077-4d13-4733-8688-57ff43091d69"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserLandLords_UserLandLord_LandLordUserId",
                schema: "Authorization",
                table: "UserLandLords",
                column: "UserLandLord_LandLordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserLandLords_UserLandLord_TenantUserId",
                schema: "Authorization",
                table: "UserLandLords",
                column: "UserLandLord_TenantUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenants_UserTenant_LandLordUserId",
                schema: "Authorization",
                table: "UserTenants",
                column: "UserTenant_LandLordUserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenants_UserTenant_TenantUserId",
                schema: "Authorization",
                table: "UserTenants",
                column: "UserTenant_TenantUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserLandLords",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "UserTenants",
                schema: "Authorization");

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 3, new Guid("8c8812cb-ecb7-4a5a-a97d-309228011c74") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 4, new Guid("8c8812cb-ecb7-4a5a-a97d-309228011c74") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 7, new Guid("59651122-b13e-42bd-babb-1f62a2a238c8") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 8, new Guid("59651122-b13e-42bd-babb-1f62a2a238c8") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 9, new Guid("222d3077-4d13-4733-8688-57ff43091d69") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 10, new Guid("222d3077-4d13-4733-8688-57ff43091d69") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("5f2f2989-215c-4908-9714-708071d166e6"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("222d3077-4d13-4733-8688-57ff43091d69"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("59651122-b13e-42bd-babb-1f62a2a238c8"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("8c8812cb-ecb7-4a5a-a97d-309228011c74"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("1c909244-0e96-422d-9a76-28c4ac9db8b9"));

            migrationBuilder.DropColumn(
                name: "Commun",
                schema: "Authorization",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "Commun",
                schema: "Authorization",
                table: "Stations");

            migrationBuilder.DropColumn(
                name: "Commun",
                schema: "Authorization",
                table: "StationAddresses");

            migrationBuilder.DropColumn(
                name: "Commun",
                schema: "Authorization",
                table: "OrganizationAddresses");

            migrationBuilder.AddColumn<string>(
                name: "LandLords",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Tenants",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Organizations",
                columns: new[] { "Id", "City", "Comment", "CommentTextId", "Country", "CreateDate", "CreatedBy", "Deleted", "Description", "DescriptionTextId", "Email", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "State" },
                values: new object[,]
                {
                    { new Guid("aa2dd4d7-001a-479a-9da5-495e8902f710"), "Kinshasa", null, new Guid("00000000-0000-0000-0000-000000000000"), "COD", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "tozawa_kinlabelle@gmail.com", "", null, "KinLaBelle", null, null },
                    { new Guid("d1497680-6be8-4e81-a781-2ab1e5c7b80d"), "World", null, new Guid("00000000-0000-0000-0000-000000000000"), "World", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "NoOrganization", "", null, "NoOrganization", null, null }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "TzRoles",
                columns: new[] { "Id", "OrganizationId", "RoleEnum" },
                values: new object[,]
                {
                    { new Guid("4d4e49eb-1099-457a-87ce-0846da6964c2"), new Guid("aa2dd4d7-001a-479a-9da5-495e8902f710"), 2 },
                    { new Guid("a78379a8-72df-4885-b3f0-62571088524b"), new Guid("aa2dd4d7-001a-479a-9da5-495e8902f710"), 5 },
                    { new Guid("e91ea6b8-3e15-4ea9-bb17-dd60e09d9060"), new Guid("aa2dd4d7-001a-479a-9da5-495e8902f710"), 1 }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Functions",
                columns: new[] { "FunctionType", "RoleId", "CreateDate", "CreatedBy", "ModifiedBy", "ModifiedDate" },
                values: new object[,]
                {
                    { 3, new Guid("a78379a8-72df-4885-b3f0-62571088524b"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 4, new Guid("a78379a8-72df-4885-b3f0-62571088524b"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 7, new Guid("e91ea6b8-3e15-4ea9-bb17-dd60e09d9060"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 8, new Guid("e91ea6b8-3e15-4ea9-bb17-dd60e09d9060"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 9, new Guid("4d4e49eb-1099-457a-87ce-0846da6964c2"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 10, new Guid("4d4e49eb-1099-457a-87ce-0846da6964c2"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null }
                });
        }
    }
}
