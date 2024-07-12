using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Application.WeeklyResults.Queries.GetWeeklyResultsByBowlerId;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.Bowlers.Queries.GetBowlerReport;

public record GetBowlerReportQuery(string BowlerId, string? SeasonId) : IRequest<BowlerReport>;

public class GetBowlerReportQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetBowlerReportQuery, BowlerReport>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<BowlerReport> Handle(GetBowlerReportQuery request, CancellationToken cancellationToken)
    {
        var bowler =
            await _context.Bowlers.FirstOrDefaultAsync(
                b => b.Id == request.BowlerId,
                cancellationToken: cancellationToken
            ) ?? throw new NotFoundException(request.BowlerId.ToString(), nameof(Bowler));

        IQueryable<Result> resultsQuery = _context.Results;

        if (request.SeasonId != null)
        {
            resultsQuery = resultsQuery.Where(r => r.SeasonBowler.SeasonId == request.SeasonId);
        }

        var results = await resultsQuery
            .Where(r => r.SeasonBowler.BowlerId == request.BowlerId)
            .OrderBy(r => r.SeasonBowler.Season.Year)
            .ThenBy(r => r.Week)
            .Select(r => new ResultDto
            {
                Id = r.Id,
                Score = r.Score,
                Week = r.Week,
                Year = r.Year
            })
            .ToListAsync(cancellationToken);

        return new BowlerReport(results, bowler);
    }
}
