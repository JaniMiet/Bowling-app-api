using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Seasons_SeasonId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_SeasonId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "SeasonId",
                table: "Results");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SeasonId",
                table: "Results",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Results_SeasonId",
                table: "Results",
                column: "SeasonId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Seasons_SeasonId",
                table: "Results",
                column: "SeasonId",
                principalTable: "Seasons",
                principalColumn: "Id");
        }
    }
}
