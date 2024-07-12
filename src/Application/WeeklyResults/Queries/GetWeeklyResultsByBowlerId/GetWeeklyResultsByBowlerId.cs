using BowlingApp.Application.Common.Interfaces;

namespace BowlingApp.Application.WeeklyResults.Queries.GetWeeklyResultsByBowlerId;

public record GetWeeklyResultsByBowlerIdQuery(string BowlerId, string? SeasonId) : IRequest<IEnumerable<ResultDto>>;

public class GetWeeklyResultsByUserIdQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetWeeklyResultsByBowlerIdQuery, IEnumerable<ResultDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<IEnumerable<ResultDto>> Handle(
        GetWeeklyResultsByBowlerIdQuery request,
        CancellationToken cancellationToken
    )
    {
        var query = _context.Results;

        if (request.SeasonId != null)
        {
            query.Where(r => r.SeasonBowler.SeasonId == request.SeasonId);
        }

        return await query
            .Where(r => r.SeasonBowler.BowlerId == request.BowlerId)
            .OrderBy(r => r.SeasonBowler.Season.Year)
            .ThenBy(r => r.Week)
            .Select(r => new ResultDto
            {
                Id = r.Id,
                Score = (int)Math.Round((double)r.Score / 6),
                Year = r.Year,
                Week = r.Week,
            })
            .ToListAsync(cancellationToken);
    }
}
