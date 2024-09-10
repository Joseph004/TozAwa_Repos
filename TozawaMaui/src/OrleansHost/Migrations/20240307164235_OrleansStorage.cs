using Microsoft.EntityFrameworkCore.Migrations;
using Orleans.Extensions;

#nullable disable

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class OrleansStorage : Migration
    {
        /// <inheritdoc />
         protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(FileExtension.ReadFileCurrentUsingDirectory("MigrationSql\\SQLServer-Persistence.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(FileExtension.ReadFileCurrentUsingDirectory("MigrationSql\\SQLServer-Persistence_Drop.sql"));
        }
    }
}
