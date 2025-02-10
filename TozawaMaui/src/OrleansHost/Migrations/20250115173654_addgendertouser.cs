using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class addgendertouser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 3, new Guid("5f864303-9118-426e-94f2-bad76d1de779") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 4, new Guid("5f864303-9118-426e-94f2-bad76d1de779") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 7, new Guid("07c82dec-162b-4d09-a5e1-6f26ad700e7e") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 8, new Guid("07c82dec-162b-4d09-a5e1-6f26ad700e7e") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 9, new Guid("79de78cd-2a1d-4a1a-ae0b-c836cb2ebdde") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 10, new Guid("79de78cd-2a1d-4a1a-ae0b-c836cb2ebdde") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("7171619f-6ecc-43d2-bdd2-ec14cfd92220"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("07c82dec-162b-4d09-a5e1-6f26ad700e7e"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("5f864303-9118-426e-94f2-bad76d1de779"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("79de78cd-2a1d-4a1a-ae0b-c836cb2ebdde"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("f7d42b7a-d7ab-4d2e-8844-8facd13c8970"));

            migrationBuilder.AddColumn<int>(
                name: "Gender",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Organizations",
                columns: new[] { "Id", "City", "CityCode", "Comment", "CommentTextId", "Country", "CountryCode", "CreateDate", "CreatedBy", "Deleted", "Description", "DescriptionTextId", "Email", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "State" },
                values: new object[,]
                {
                    { new Guid("05962aa0-90bf-4090-964b-8edf26ba250b"), "World", null, null, new Guid("00000000-0000-0000-0000-000000000000"), "World", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "NoOrganization", "", null, "NoOrganization", null, null },
                    { new Guid("5b13a92e-58ff-462c-b173-1076cbc9ecf2"), "Kinshasa", null, null, new Guid("00000000-0000-0000-0000-000000000000"), "COD", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "tozawa_kinlabelle@gmail.com", "", null, "KinLaBelle", null, null }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "TzRoles",
                columns: new[] { "Id", "OrganizationId", "RoleEnum" },
                values: new object[,]
                {
                    { new Guid("8f65da8e-7cde-473d-9d3a-c05efd756af9"), new Guid("5b13a92e-58ff-462c-b173-1076cbc9ecf2"), 1 },
                    { new Guid("bbe43765-633f-4d11-a8f8-3076b8e6a5ad"), new Guid("5b13a92e-58ff-462c-b173-1076cbc9ecf2"), 5 },
                    { new Guid("d8afcfeb-31b9-4134-80fa-94819274ff98"), new Guid("5b13a92e-58ff-462c-b173-1076cbc9ecf2"), 2 }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Functions",
                columns: new[] { "FunctionType", "RoleId", "CreateDate", "CreatedBy", "ModifiedBy", "ModifiedDate" },
                values: new object[,]
                {
                    { 3, new Guid("bbe43765-633f-4d11-a8f8-3076b8e6a5ad"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 4, new Guid("bbe43765-633f-4d11-a8f8-3076b8e6a5ad"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 7, new Guid("8f65da8e-7cde-473d-9d3a-c05efd756af9"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 8, new Guid("8f65da8e-7cde-473d-9d3a-c05efd756af9"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 9, new Guid("d8afcfeb-31b9-4134-80fa-94819274ff98"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 10, new Guid("d8afcfeb-31b9-4134-80fa-94819274ff98"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 3, new Guid("bbe43765-633f-4d11-a8f8-3076b8e6a5ad") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 4, new Guid("bbe43765-633f-4d11-a8f8-3076b8e6a5ad") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 7, new Guid("8f65da8e-7cde-473d-9d3a-c05efd756af9") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 8, new Guid("8f65da8e-7cde-473d-9d3a-c05efd756af9") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 9, new Guid("d8afcfeb-31b9-4134-80fa-94819274ff98") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Functions",
                keyColumns: new[] { "FunctionType", "RoleId" },
                keyValues: new object[] { 10, new Guid("d8afcfeb-31b9-4134-80fa-94819274ff98") });

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("05962aa0-90bf-4090-964b-8edf26ba250b"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("8f65da8e-7cde-473d-9d3a-c05efd756af9"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("bbe43765-633f-4d11-a8f8-3076b8e6a5ad"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "TzRoles",
                keyColumn: "Id",
                keyValue: new Guid("d8afcfeb-31b9-4134-80fa-94819274ff98"));

            migrationBuilder.DeleteData(
                schema: "Authorization",
                table: "Organizations",
                keyColumn: "Id",
                keyValue: new Guid("5b13a92e-58ff-462c-b173-1076cbc9ecf2"));

            migrationBuilder.DropColumn(
                name: "Gender",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Organizations",
                columns: new[] { "Id", "City", "CityCode", "Comment", "CommentTextId", "Country", "CountryCode", "CreateDate", "CreatedBy", "Deleted", "Description", "DescriptionTextId", "Email", "ModifiedBy", "ModifiedDate", "Name", "PhoneNumber", "State" },
                values: new object[,]
                {
                    { new Guid("7171619f-6ecc-43d2-bdd2-ec14cfd92220"), "World", null, null, new Guid("00000000-0000-0000-0000-000000000000"), "World", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "NoOrganization", "", null, "NoOrganization", null, null },
                    { new Guid("f7d42b7a-d7ab-4d2e-8844-8facd13c8970"), "Kinshasa", null, null, new Guid("00000000-0000-0000-0000-000000000000"), "COD", null, new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", false, null, new Guid("00000000-0000-0000-0000-000000000000"), "tozawa_kinlabelle@gmail.com", "", null, "KinLaBelle", null, null }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "TzRoles",
                columns: new[] { "Id", "OrganizationId", "RoleEnum" },
                values: new object[,]
                {
                    { new Guid("07c82dec-162b-4d09-a5e1-6f26ad700e7e"), new Guid("f7d42b7a-d7ab-4d2e-8844-8facd13c8970"), 1 },
                    { new Guid("5f864303-9118-426e-94f2-bad76d1de779"), new Guid("f7d42b7a-d7ab-4d2e-8844-8facd13c8970"), 5 },
                    { new Guid("79de78cd-2a1d-4a1a-ae0b-c836cb2ebdde"), new Guid("f7d42b7a-d7ab-4d2e-8844-8facd13c8970"), 2 }
                });

            migrationBuilder.InsertData(
                schema: "Authorization",
                table: "Functions",
                columns: new[] { "FunctionType", "RoleId", "CreateDate", "CreatedBy", "ModifiedBy", "ModifiedDate" },
                values: new object[,]
                {
                    { 3, new Guid("5f864303-9118-426e-94f2-bad76d1de779"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 4, new Guid("5f864303-9118-426e-94f2-bad76d1de779"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 7, new Guid("07c82dec-162b-4d09-a5e1-6f26ad700e7e"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 8, new Guid("07c82dec-162b-4d09-a5e1-6f26ad700e7e"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 9, new Guid("79de78cd-2a1d-4a1a-ae0b-c836cb2ebdde"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null },
                    { 10, new Guid("79de78cd-2a1d-4a1a-ae0b-c836cb2ebdde"), new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "", "", null }
                });
        }
    }
}
