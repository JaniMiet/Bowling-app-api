﻿using System.ComponentModel.DataAnnotations;
using BowlingApp.Application.Seasons.Queries.GetSeasonReport;
using BowlingApp.Domain.Entities;
using BowlingApp.Domain.Enums;

namespace BowlingApp.Application.Seasons.Queries.GetSeasons;

public record SeasonReportDto
{
    [Required]
    public string SeasonId { get; set; }

    [Required]
    public int SetsCount { get; set; }

    [Required]
    public double Average { get; set; }

    [Required]
    public int Year { get; set; }

    [Required]
    public SeasonType SeasonType { get; set; }

    [Required]
    public List<PlayerRow> PlayerRows { get; set; } = [];

    [Required]
    public List<TopImprover> TopImprovers { get; set; } = [];

    [Required]
    public List<int> Weeks { get; set; }

    public record PlayerRow
    {
        [Required]
        public required string Id { get; set; }

        [Required]
        public required string FirstName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required double Average { get; set; }

        [Required]
        public List<int?> Scores { get; set; } = [];

        [Required]
        public required int SetsThrownCount { get; set; }
    }

    public SeasonReportDto(SeasonReportSeasonDto season)
    {
        var results = season.Bowlers.SelectMany(sb => sb.Results);

        SeasonId = season.Id;
        SeasonType = season.SeasonType;
        SetsCount = results.Count() * 6;
        Average = results.Any() ? Math.Round(results.Select(r => r.Score).Average(), 1) : 0;
        Year = season.Year;
        Weeks = Season.GetSeasonWeeks(SeasonType);

        foreach (var seasonBowler in season.Bowlers)
        {
            var bowlerResults = seasonBowler.Results.OrderBy(r => r.Week);

            var playerRow = new PlayerRow
            {
                FirstName = seasonBowler.FirstName,
                LastName = seasonBowler.LastName,
                Id = seasonBowler.Id,
                Average = bowlerResults.Any() ? bowlerResults.Select(r => r.Score).Average() : 0,
                SetsThrownCount = bowlerResults.Count() * 6
            };

            foreach (var week in Weeks)
            {
                playerRow.Scores.Add(bowlerResults.FirstOrDefault(r => r.Week == week)?.Score);
            }

            PlayerRows.Add(playerRow);
            PlayerRows = [.. PlayerRows.OrderByDescending(r => r.Average)];
        }

        var top5ImprovedBowlers = season
            .Bowlers.Where(b => b.ChangeToPreviousSeason != null && b.ChangeToPreviousSeason > 0)
            .OrderByDescending(r => r.ChangeToPreviousSeason)
            .Take(5);

        var position = 1;
        foreach (var bowler in top5ImprovedBowlers)
        {
            TopImprovers.Add(
                new TopImprover
                {
                    ChangeToPreviousSeason = bowler.ChangeToPreviousSeason,
                    FirstName = bowler.FirstName,
                    LastName = bowler.LastName,
                    Position = position
                }
            );
            position++;
        }
    }

    public record TopImprover
    {
        public required int Position { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required double? ChangeToPreviousSeason { get; set; }
    }
}
