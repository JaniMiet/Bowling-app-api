namespace BowlingApp.Domain.Entities;

public class Season : BaseAuditableEntity
{
    public int Year { get; set; }
    public SeasonType SeasonType { get; set; }
    public int Number { get; set; }
    public double AverageScore { get; set; } = 0;
    public double SetsThrownCount { get; set; } = 0;
    public double AverageScoreChangeToPreviousSeason { get; set; } = 0;
    public List<SeasonBowler> SeasonBowlers { get; set; } = [];

    public Season() { }

    public Season(int year, SeasonType seasonType, int previousSeasonNumber, List<Bowler> bowlers)
    {
        Year = year;
        SeasonType = seasonType;
        Number = previousSeasonNumber + 1;

        foreach (var bowler in bowlers)
        {
            SeasonBowlers.Add(new SeasonBowler(this, bowler));
        }
    }

    public static int SeasonMinWeekNumber(SeasonType seasonType)
    {
        return seasonType == SeasonType.Spring ? 1 : 33;
    }

    public static int SeasonMaxWeekNumber(SeasonType seasonType)
    {
        return seasonType == SeasonType.Spring ? 21 : 52;
    }

    public void UpdateSeasonStatistics(List<Result> currentSeasonResults, List<Result> previousSeasonResults)
    {
        var previousSeasonScores = previousSeasonResults.Select(r => r.Score);
        var currentSeasonScores = currentSeasonResults.Select(r => r.Score);
        var previousSeasonAverage = previousSeasonScores.Any() ? previousSeasonScores.Average() : 0;
        var currentSeasonAverage = currentSeasonScores.Any() ? currentSeasonScores.Average() : 0;

        AverageScore = currentSeasonAverage;
        SetsThrownCount = currentSeasonResults.Count * 6;

        var previousSeasonHasResults = previousSeasonResults.Count > 0;
        var currentSeasonHasResults = currentSeasonResults.Count > 0;
        AverageScoreChangeToPreviousSeason =
            (previousSeasonHasResults && currentSeasonHasResults) ? currentSeasonAverage - previousSeasonAverage : 0;
    }
}
