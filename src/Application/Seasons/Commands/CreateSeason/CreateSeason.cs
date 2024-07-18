using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;
using BowlingApp.Domain.Enums;

namespace BowlingApp.Application.Seasons.Commands.CreateSeason;

public record CreateSeasonCommand : IRequest<string>
{
    public int Year { get; init; }
    public SeasonType SeasonType { get; init; }
}

public class CreateTodoListCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateSeasonCommand, string>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<string> Handle(CreateSeasonCommand request, CancellationToken cancellationToken)
    {
        var existingSeason = await _context.Seasons.FirstOrDefaultAsync(
            s => s.Year == request.Year && s.SeasonType == request.SeasonType,
            cancellationToken
        );

        if (existingSeason != null)
            throw new ValidationException("Year / season already exists");

        var previousSeasonNumber = await _context
            .Seasons.OrderByDescending(s => s.Number)
            .Select(s => s.Number)
            .FirstOrDefaultAsync(cancellationToken);

        var bowlers = await _context.Bowlers.ToListAsync(cancellationToken);
        var season = new Season(request.Year, request.SeasonType, previousSeasonNumber, bowlers);

        _context.Seasons.Add(season);
        await _context.SaveChangesAsync(cancellationToken);

        return season.Id;
    }
}
