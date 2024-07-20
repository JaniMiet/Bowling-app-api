using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BowlingApp.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bowlers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    FirstName = table.Column<string>(type: "text", nullable: false),
                    LastName = table.Column<string>(type: "text", nullable: false),
                    Gender = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bowlers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Seasons",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    SeasonType = table.Column<int>(type: "integer", nullable: false),
                    Number = table.Column<int>(type: "integer", nullable: false),
                    AverageScore = table.Column<double>(type: "double precision", nullable: false),
                    SetsThrownCount = table.Column<double>(type: "double precision", nullable: false),
                    AverageScoreChangeToPreviousSeason = table.Column<double>(type: "double precision", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Seasons", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SeasonBowlers",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SeasonId = table.Column<string>(type: "text", nullable: false),
                    BowlerId = table.Column<string>(type: "text", nullable: false),
                    ChangeToPreviousSeason = table.Column<double>(type: "double precision", nullable: true),
                    SeasonAverage = table.Column<double>(type: "double precision", nullable: true),
                    SeasonSetsThrowCount = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SeasonBowlers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SeasonBowlers_Bowlers_BowlerId",
                        column: x => x.BowlerId,
                        principalTable: "Bowlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SeasonBowlers_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Results",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    SeasonBowlerId = table.Column<string>(type: "text", nullable: false),
                    SeasonId = table.Column<string>(type: "text", nullable: false),
                    Week = table.Column<int>(type: "integer", nullable: false),
                    Created = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    LastModified = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Results", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Results_SeasonBowlers_SeasonBowlerId",
                        column: x => x.SeasonBowlerId,
                        principalTable: "SeasonBowlers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Results_Seasons_SeasonId",
                        column: x => x.SeasonId,
                        principalTable: "Seasons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Results_SeasonBowlerId",
                table: "Results",
                column: "SeasonBowlerId");

            migrationBuilder.CreateIndex(
                name: "IX_Results_SeasonId",
                table: "Results",
                column: "SeasonId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonBowlers_BowlerId",
                table: "SeasonBowlers",
                column: "BowlerId");

            migrationBuilder.CreateIndex(
                name: "IX_SeasonBowlers_SeasonId",
                table: "SeasonBowlers",
                column: "SeasonId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Results");

            migrationBuilder.DropTable(
                name: "SeasonBowlers");

            migrationBuilder.DropTable(
                name: "Bowlers");

            migrationBuilder.DropTable(
                name: "Seasons");
        }
    }
}
