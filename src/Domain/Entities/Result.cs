namespace BowlingApp.Domain.Entities;

public class Result : BaseEntity
{
    // This is weekly result score which consists of six sets
    public int Score { get; set; }
    public string SeasonBowlerId { get; set; }
    public SeasonBowler SeasonBowler { get; set; } = null!;
    public string SeasonId { get; set; }
    public Season Season { get; set; } = null!;
    public int Week { get; set; }

    public Result(int score, int week, string seasonBowlerId, string seasonId)
    {
        Score = score;
        Week = week;
        SeasonBowlerId = seasonBowlerId;
        SeasonId = seasonId;
    }
}
