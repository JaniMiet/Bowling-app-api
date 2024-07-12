using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "SeasonAverage",
                table: "Seasons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SeasonSetsThrownCount",
                table: "Seasons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "SeasonSetsThrowCount",
                table: "SeasonBowlers",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SeasonAverage",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SeasonSetsThrownCount",
                table: "Seasons");

            migrationBuilder.DropColumn(
                name: "SeasonSetsThrowCount",
                table: "SeasonBowlers");
        }
    }
}
