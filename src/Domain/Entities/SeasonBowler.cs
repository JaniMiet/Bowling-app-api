namespace BowlingApp.Domain.Entities;

public class SeasonBowler : BaseAuditableEntity
{
    public string SeasonId { get; set; } = null!;
    public Season Season { get; set; } = null!;
    public string BowlerId { get; set; } = null!;
    public Bowler Bowler { get; set; } = null!;
    public double? ChangeToPreviousSeason { get; set; }
    public double? SeasonAverage { get; set; }
    public int SeasonSetsThrowCount { get; set; }
    public List<Result> Results { get; set; } = [];

    public SeasonBowler() { }

    public SeasonBowler(Season season, Bowler bowler)
    {
        SeasonId = season.Id;
        BowlerId = bowler.Id;
        Season = season;
        Bowler = bowler;
    }

    public void UpdateSeasonBowlerStatistics(List<Result> currentSeasonResults, List<Result> previousSeasonResults)
    {
        var previousSeasonScores = previousSeasonResults.Select(r => r.Score);
        var currentSeasonScores = currentSeasonResults.Select(r => r.Score);
        var previousSeasonAverage = previousSeasonScores.Any() ? previousSeasonScores.Average() : 0;
        var currentSeasonAverage = currentSeasonScores.Any() ? currentSeasonScores.Average() : 0;

        SeasonAverage = currentSeasonAverage;
        SeasonSetsThrowCount = currentSeasonResults.Count * 6;

        var previousSeasonHasResults = previousSeasonResults.Count > 0;
        var currentSeasonHasResults = currentSeasonResults.Count > 0;
        ChangeToPreviousSeason =
            (previousSeasonHasResults && currentSeasonHasResults) ? currentSeasonAverage - previousSeasonAverage : 0;
    }
}
