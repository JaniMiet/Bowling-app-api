namespace BowlingApp.Application.WeeklyResults.Queries.GetMostRecentWeekResults;

public record WeeklyResultsDto
{
    public int Year { get; init; }
    public int Week { get; init; }

    public List<BowlerResult> BowlerResults { get; init; } = [];
}
