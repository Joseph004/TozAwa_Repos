using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class addcityandcountrycode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "Authorization",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "Authorization",
                table: "UserAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "Authorization",
                table: "StationAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "Authorization",
                table: "StationAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "Authorization",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "Authorization",
                table: "Organizations",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "Authorization",
                table: "OrganizationAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "Authorization",
                table: "OrganizationAddresses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CityCode",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CountryCode",
                schema: "Authorization",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "Authorization",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "Authorization",
                table: "UserAddresses");

            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "Authorization",
                table: "StationAddresses");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "Authorization",
                table: "StationAddresses");

            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "Authorization",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "Authorization",
                table: "Organizations");

            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "Authorization",
                table: "OrganizationAddresses");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "Authorization",
                table: "OrganizationAddresses");

            migrationBuilder.DropColumn(
                name: "CityCode",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "CountryCode",
                schema: "Authorization",
                table: "AspNetUsers");

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
        }
    }
}
