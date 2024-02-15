using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TozawaNGO.Migrations
{
    /// <inheritdoc />
    public partial class AddNewPropertyToAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InloggedEmail",
                schema: "Authorization",
                table: "Audits",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "InloggedEmail",
                schema: "Authorization",
                table: "Audits");
        }
    }
}
