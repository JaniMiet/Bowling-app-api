namespace BowlingApp.Domain.Entities;

public class Result : BaseAuditableEntity
{
    // This is weekly result score which consists of six sets
    public int Score { get; set; }
    public string SeasonBowlerId { get; set; }
    public SeasonBowler SeasonBowler { get; set; } = null!;
    public int Week { get; set; }
    public int Year { get; set; }

    public Result(int score, string seasonBowlerId, int week, int year)
    {
        Score = score;
        SeasonBowlerId = seasonBowlerId;
        Week = week;
        Year = year;
    }
}
