using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial4 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeasonNumber",
                table: "Seasons",
                newName: "Year");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Seasons",
                newName: "SeasonNumber");
        }
    }
}
