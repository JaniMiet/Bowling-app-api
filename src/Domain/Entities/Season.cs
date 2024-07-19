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

    public static List<int> GetSeasonWeeks(SeasonType seasonType)
    {
        var minWeekNumber = seasonType == SeasonType.Spring ? 1 : 33;
        var maxWeekNumber = seasonType == SeasonType.Spring ? 21 : 52;

        var weeks = new List<int>();

        for (int i = minWeekNumber; i <= maxWeekNumber; i++)
        {
            weeks.Add(i);
        }

        return weeks;
    }

    public void UpdateSeasonAndSeasonBowlerStatistics(Season currentSeason, Season? previousSeason)
    {
        UpdateSeasonStatistics(
            currentSeason.SeasonBowlers.SelectMany(sb => sb.Results),
            previousSeason == null ? [] : previousSeason.SeasonBowlers.SelectMany(sb => sb.Results)
        );

        foreach (var currentSeasonBowler in currentSeason.SeasonBowlers)
        {
            currentSeasonBowler.UpdateSeasonBowlerStatistics(
                currentSeasonBowler.Results,
                previousSeason == null
                    ? []
                    : previousSeason.SeasonBowlers.Single(sb => sb.BowlerId == currentSeasonBowler.BowlerId).Results
            );
        }
    }

    private void UpdateSeasonStatistics(
        IEnumerable<Result> currentSeasonResults,
        IEnumerable<Result> previousSeasonResults
    )
    {
        var previousSeasonScores = previousSeasonResults.Select(r => r.Score);
        var currentSeasonScores = currentSeasonResults.Select(r => r.Score);
        var previousSeasonAverage = previousSeasonScores.Any() ? previousSeasonScores.Average() : 0;
        var currentSeasonAverage = currentSeasonScores.Any() ? currentSeasonScores.Average() : 0;

        AverageScore = currentSeasonAverage;
        SetsThrownCount = currentSeasonResults.Count() * 6;

        var previousSeasonHasResults = previousSeasonResults.Any();
        var currentSeasonHasResults = currentSeasonResults.Any();

        AverageScoreChangeToPreviousSeason =
            (previousSeasonHasResults && currentSeasonHasResults) ? currentSeasonAverage - previousSeasonAverage : 0;
    }

    public static void UpdateSeasonNumbers(List<Season> seasons)
    {
        seasons = [.. seasons.OrderBy(r => r.Year).ThenBy(r => r.SeasonType)];

        var seasonNumber = 1;

        foreach (var season in seasons)
        {
            season.Number = seasonNumber;
            seasonNumber++;
        }
    }
}
