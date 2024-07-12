using BowlingApp.Application.Common.Interfaces;

namespace BowlingApp.Application.WeeklyResults.Queries.GetMostRecentWeekResults;

public record GetMostRecentWeekResultsQuery : IRequest<WeeklyResultsDto>;

public class GetMostRecentWeekResultsQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetMostRecentWeekResultsQuery, WeeklyResultsDto>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<WeeklyResultsDto> Handle(
        GetMostRecentWeekResultsQuery request,
        CancellationToken cancellationToken
    )
    {
        var mostRecentYearWeek =
            await _context
                .Results.OrderByDescending(r => r.Year)
                .ThenByDescending(r => r.Week)
                .Select(r => new { r.Year, r.Week, })
                .FirstOrDefaultAsync(cancellationToken) ?? throw new Exception();

        var week = mostRecentYearWeek.Week;
        var year = mostRecentYearWeek.Year;

        var bowlerResults2 = await _context.Results.ToListAsync();

        var bowlerResults = await _context
            .Results.Where(r => r.Week == week && r.Year == year)
            .Select(r => new BowlerResult
            {
                Id = r.Id,
                Name = r.SeasonBowler.Bowler.LastName + " " + r.SeasonBowler.Bowler.FirstName,
                Score = r.Score,
                SetAverageScore = Math.Round((double)(r.Score / 6), 1)
            })
            .OrderByDescending(r => r.Score)
            .ToListAsync(cancellationToken);

        return new WeeklyResultsDto
        {
            Week = mostRecentYearWeek.Week,
            Year = mostRecentYearWeek.Year,
            BowlerResults = bowlerResults
        };
    }
}
