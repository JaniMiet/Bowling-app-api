using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedUniqueIndexesToSeasonAndSeasonBowler : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Seasons_Year_SeasonType",
                table: "Seasons",
                columns: new[] { "Year", "SeasonType" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_Week_SeasonBowlerId",
                table: "Results",
                columns: new[] { "Week", "SeasonBowlerId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Seasons_Year_SeasonType",
                table: "Seasons");

            migrationBuilder.DropIndex(
                name: "IX_Results_Week_SeasonBowlerId",
                table: "Results");
        }
    }
}
