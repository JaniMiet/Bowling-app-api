using BowlingApp.Application.Common.Interfaces;

namespace BowlingApp.Application.Seasons.Queries.GetSeasons;

public record GetSeasonsQuery : IRequest<List<SeasonDto>> { }

public class GetTodoItemsWithPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetSeasonsQuery, List<SeasonDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<List<SeasonDto>> Handle(GetSeasonsQuery request, CancellationToken cancellationToken)
    {
        return await _context
            .Seasons.OrderByDescending(s => s.Number)
            .Select(s => new SeasonDto
            {
                Id = s.Id,
                Year = s.Year,
                SeasonType = s.SeasonType
            })
            .ToListAsync(cancellationToken);
    }
}
