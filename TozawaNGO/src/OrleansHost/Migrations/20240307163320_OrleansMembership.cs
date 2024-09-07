using Microsoft.EntityFrameworkCore.Migrations;
using Orleans.Extensions;

#nullable disable

namespace OrleansHost.Migrations
{
    /// <inheritdoc />
    public partial class OrleansMembership : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(FileExtension.ReadFileCurrentUsingDirectory("MigrationSql\\SQLServer-Clustering.sql"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(FileExtension.ReadFileCurrentUsingDirectory("MigrationSql\\SQLServer-Clustering_Drop.sql"));
        }
    }
}
