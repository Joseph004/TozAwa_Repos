using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class initOrganizationsdata : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("c235beb3-fc79-414d-8faf-3db71e78da6e"));

            migrationBuilder.RenameColumn(
                name: "Functiontype",
                schema: "Authorization",
                table: "Functions",
                newName: "FunctionType");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.RenameColumn(
                name: "FunctionType",
                schema: "Authorization",
                table: "Functions",
                newName: "Functiontype");

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Organizations",
                columns: new[] { "Id", "City", "Comment", "CommentTextId", "Country", "CreateDate", "CreatedBy", "Deleted", "Description", "DescriptionTextId", "Email", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "State" },
                values: new object[] { new Guid("c235beb3-fc79-414d-8faf-3db71e78da6e"), "World", null, new Guid("00000000-0000-0000-0000-000000000000"), "World", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "NoOrganization", "", null, "NoOrganization", null, null });
        }
    }
}
