using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tozawa.Auth.Svc.Migrations
{
    public partial class newtables04 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Authorization");

            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Audits",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TableName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    KeyValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OldValues = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NewValues = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Audits", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Organizations",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FallbackLanguageId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LanguageIds = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "''"),
                    ExportLanguageIds = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValueSql: "''"),
                    IsFederated = table.Column<bool>(type: "bit", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organizations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TozAwaFeatures",
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
                    table.PrimaryKey("PK_TozAwaFeatures", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserLogs",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Event = table.Column<int>(type: "int", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetRoleClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetRoleClaims_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkingOrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RootUser = table.Column<bool>(type: "bit", nullable: false),
                    Oid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    Email = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUsers", x => x.Id);
                    table.UniqueConstraint("AK_AspNetUsers_UserId", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_AspNetUsers_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationFeatures",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Feature = table.Column<int>(type: "int", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                name: "PartnerOrganization",
                schema: "Authorization",
                columns: table => new
                {
                    FromId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ToId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PartnerOrganization", x => new { x.FromId, x.ToId });
                    table.ForeignKey(
                        name: "FK_PartnerOrganization_Organizations_FromId",
                        column: x => x.FromId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_PartnerOrganization_Organizations_ToId",
                        column: x => x.ToId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TextId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Organizations_OrganizationId",
                        column: x => x.OrganizationId,
                        principalSchema: "Authorization",
                        principalTable: "Organizations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                schema: "Authorization",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClaimType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ClaimValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserClaims", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AspNetUserClaims_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                schema: "Authorization",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ProviderDisplayName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserLogins", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_AspNetUserLogins_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                schema: "Authorization",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserRoles", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetRoles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AspNetUserRoles_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AspNetUserTokens",
                schema: "Authorization",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LoginProvider = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Value = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AspNetUserTokens", x => new { x.UserId, x.LoginProvider, x.Name });
                    table.ForeignKey(
                        name: "FK_AspNetUserTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
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
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Functions", x => new { x.Functiontype, x.RoleId });
                    table.ForeignKey(
                        name: "FK_Functions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "Authorization",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRoles",
                schema: "Authorization",
                columns: table => new
                {
                    User_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Role_Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrganizationId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRoles", x => new { x.User_Id, x.Role_Id });
                    table.ForeignKey(
                        name: "FK_UserRoles_AspNetUsers_User_Id",
                        column: x => x.User_Id,
                        principalSchema: "Authorization",
                        principalTable: "AspNetUsers",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserRoles_Roles_Role_Id",
                        column: x => x.Role_Id,
                        principalSchema: "Authorization",
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "TozAwaFeatures",
                columns: new[] { "Id", "Deleted", "DescriptionTextId", "TextId" },
                values: new object[,]
                {
                    { 1, false, new Guid("97617538-8931-43ee-bd4c-769726bdb6a4"), new Guid("acd1ef02-0da3-474b-95d3-8861fcc8e368") },
                    { 2, false, new Guid("9319cd54-bc74-48cb-bcfc-7c7266dce102"), new Guid("4368eb54-8521-4892-b8fb-b4993ca115e2") },
                    { 3, false, new Guid("56afa5ba-1dbb-4220-bff7-667bf0669986"), new Guid("f2f40352-5e9f-4fc9-baab-201b0b9285fd") },
                    { 4, false, new Guid("2307af30-b6a1-41cc-979c-762f599aea1c"), new Guid("d88e4649-a599-499b-a711-99e747ef621f") },
                    { 5, false, new Guid("27596311-463e-4994-81c5-a5ae83cb3768"), new Guid("051daa8c-4e74-4a23-a1de-c91bbd11075a") },
                    { 6, false, new Guid("d7c29ad0-26d6-4c28-b269-de7373814f40"), new Guid("fe7530e2-d275-44b1-9565-d31964c7688b") },
                    { 7, false, new Guid("ba218979-4758-4a15-be35-7874f5e84d52"), new Guid("e185f011-cd98-47e2-a918-df370965481c") },
                    { 8, false, new Guid("443d5204-02fe-4569-969f-fb452816c332"), new Guid("892837fb-4949-48f1-9407-954bba0894f7") },
                    { 9, false, new Guid("7859194f-b86c-4bcd-891a-c99f5d33db63"), new Guid("a95fea80-24f5-45af-9012-1555c0d2d241") },
                    { 10, false, new Guid("cc91eeaa-26ff-4249-86e2-dfb7f9580b3c"), new Guid("b89fbc25-a81e-4f8d-ba37-0b96817d878d") },
                    { 11, false, new Guid("ff737bcd-c475-4620-9a47-131021f23d2b"), new Guid("67f38c1d-f365-4d63-9939-aa91d4a15cb4") },
                    { 12, false, new Guid("50c553e7-665e-492e-abe9-1a9e50133996"), new Guid("4653d990-6f5e-4e00-a52a-7f57aa94d2fc") },
                    { 13, false, new Guid("00054fe9-290e-467c-a19e-ed259cb9c1e0"), new Guid("cc391b4d-0c0e-44fe-a343-23129c7166ee") },
                    { 14, false, new Guid("4058300a-5b7b-42a7-b0bb-ccbf68d32d52"), new Guid("2685e7a0-8ff0-49d7-997b-12e50c333ffe") },
                    { 15, false, new Guid("8209d9e8-2deb-4120-9f91-31619ae422b1"), new Guid("27dd2f68-9656-477a-8cb0-068e230291c0") },
                    { 16, false, new Guid("fac2671f-688d-48bb-af06-0f3f0c8bd407"), new Guid("e56902fa-77b5-41a7-b74c-17eb111342e3") },
                    { 17, false, new Guid("3fcda399-dd92-43e2-87a9-61d7a671c778"), new Guid("80a0d18f-2d7b-4579-b741-9485c5265e13") },
                    { 18, false, new Guid("7258b643-40e4-416b-b3cc-5db3f71a4abe"), new Guid("d10c571e-fb30-4a1c-8115-9d5343045dd4") },
                    { 19, false, new Guid("c3dde4a8-1287-4321-8ebf-b918da35d013"), new Guid("d1338999-d588-4448-aa73-9b62414cb7b6") },
                    { 21, false, new Guid("c0ed5988-b532-4884-abeb-1aaf9b1118cf"), new Guid("36cd3946-ab37-4476-b824-9442fbcc9b55") },
                    { 22, false, new Guid("be98612e-569f-4558-9d16-db2d3e2675ca"), new Guid("93698a14-eaa7-4649-a3aa-4eb3080a6961") },
                    { 23, false, new Guid("2a0dd5af-c377-4d15-a8ae-642c55252d61"), new Guid("c75ddc1a-eb3c-42bb-9a7d-597eb146a3be") },
                    { 24, false, new Guid("71dc6d6c-193d-4334-b56d-f91f77faaf9a"), new Guid("32e1ad3b-c319-4e6e-864d-01c02e8138b5") },
                    { 25, false, new Guid("96a2ce3e-74a5-45dc-8377-5faeb78d296d"), new Guid("6a506b08-86be-4305-bc3f-a16fdb12556d") },
                    { 26, false, new Guid("7a7692df-6da4-47d8-a147-e63998328461"), new Guid("a7a366a6-8a3d-4621-b053-57cbedff427d") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetRoleClaims_RoleId",
                schema: "Authorization",
                table: "AspNetRoleClaims",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "Authorization",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserClaims_UserId",
                schema: "Authorization",
                table: "AspNetUserClaims",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserLogins_UserId",
                schema: "Authorization",
                table: "AspNetUserLogins",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUserRoles_RoleId",
                schema: "Authorization",
                table: "AspNetUserRoles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "Authorization",
                table: "AspNetUsers",
                column: "NormalizedEmail");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_OrganizationId",
                schema: "Authorization",
                table: "AspNetUsers",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "Authorization",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Functions_RoleId",
                schema: "Authorization",
                table: "Functions",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationFeatures_OrganizationId",
                schema: "Authorization",
                table: "OrganizationFeatures",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_PartnerOrganization_ToId",
                schema: "Authorization",
                table: "PartnerOrganization",
                column: "ToId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_OrganizationId",
                schema: "Authorization",
                table: "Roles",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_UserOrganization_UserId",
                schema: "Authorization",
                table: "UserOrganization",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRoles_Role_Id",
                schema: "Authorization",
                table: "UserRoles",
                column: "Role_Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AspNetRoleClaims",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "AspNetUserClaims",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "AspNetUserLogins",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "AspNetUserRoles",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "AspNetUserTokens",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Audits",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Functions",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "OrganizationFeatures",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "PartnerOrganization",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "TozAwaFeatures",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "UserLogs",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "UserOrganization",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "UserRoles",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "AspNetRoles",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "AspNetUsers",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Roles",
                schema: "Authorization");

            migrationBuilder.DropTable(
                name: "Organizations",
                schema: "Authorization");
        }
    }
}
