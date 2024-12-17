using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class addlistoftenants : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LandLords",
                schema: "Authorization",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Tenants",
                schema: "Authorization",
                table: "AspNetUsers");
        }
    }
}
