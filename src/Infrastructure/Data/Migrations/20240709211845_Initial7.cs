using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SeasonSetsThrownCount",
                table: "Seasons",
                newName: "SetsThrownCount");

            migrationBuilder.RenameColumn(
                name: "SeasonNumber",
                table: "Seasons",
                newName: "Number");

            migrationBuilder.RenameColumn(
                name: "SeasonAverage",
                table: "Seasons",
                newName: "AverageScoreChangeToPreviousSeason");

            migrationBuilder.AddColumn<double>(
                name: "AverageScore",
                table: "Seasons",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageScore",
                table: "Seasons");

            migrationBuilder.RenameColumn(
                name: "SetsThrownCount",
                table: "Seasons",
                newName: "SeasonSetsThrownCount");

            migrationBuilder.RenameColumn(
                name: "Number",
                table: "Seasons",
                newName: "SeasonNumber");

            migrationBuilder.RenameColumn(
                name: "AverageScoreChangeToPreviousSeason",
                table: "Seasons",
                newName: "SeasonAverage");
        }
    }
}
