using BowlingApp.Application.WeeklyResults.Queries.GetWeeklyResultsByBowlerId;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.Bowlers.Queries.GetBowlerReport;

public record class BowlerReport
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Id { get; set; }
    public int SetsCount { get; set; }
    public int Average { get; set; }
    public SetDetails? BestSet { get; set; }
    public List<ResultDto> Results { get; set; } = [];

    public record SetDetails
    {
        public int Score { get; set; }
        public int Year { get; set; }
        public int Week { get; set; }
    }

    public BowlerReport(List<ResultDto> results, Bowler bowler)
    {
        var bestResult = results.OrderByDescending(x => x.Score).FirstOrDefault();

        FirstName = bowler.FirstName;
        LastName = bowler.LastName;
        Id = bowler.Id;
        SetsCount = results.Count * 6;
        Average = results.Any() ? (int)Math.Round(results.Select(r => r.Score).Average()) : 0;
        Results = results;

        if (bestResult != null)
        {
            BestSet = new SetDetails
            {
                Score = bestResult.Score,
                Year = bestResult.Year,
                Week = bestResult.Week
            };
        }
    }
}
