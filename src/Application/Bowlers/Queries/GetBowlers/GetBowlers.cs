using BowlingApp.Application.Common.Interfaces;

namespace BowlingApp.Application.Bowlers.Queries.GetBowlers;

public record GetBowlersQuery : IRequest<IEnumerable<BowlerDto>>;

public class GetBowlersQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetBowlersQuery, IEnumerable<BowlerDto>>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<IEnumerable<BowlerDto>> Handle(GetBowlersQuery request, CancellationToken cancellationToken)
    {
        return await _context
            .Bowlers.Select(b => new BowlerDto
            {
                Id = b.Id,
                FirstName = b.FirstName,
                LastName = b.LastName,
                Gender = b.Gender,
            })
            .OrderBy(b => b.FirstName)
            .ThenBy(b => b.LastName)
            .ToListAsync(cancellationToken);
    }
}
