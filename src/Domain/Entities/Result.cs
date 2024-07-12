namespace BowlingApp.Domain.Entities;

public class Result : BaseAuditableEntity
{
    // This is weekly result score which consists of six sets
    public required int Score { get; set; }
    public string SeasonBowlerId { get; set; } = null!;
    public SeasonBowler SeasonBowler { get; set; } = null!;
    public int Week { get; set; }
    public int Year { get; set; }
}
