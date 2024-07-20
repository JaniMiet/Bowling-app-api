using System.ComponentModel.DataAnnotations;
using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.WeeklyResults.Commands.UpdateWeeklyResult;

public record UpdateWeeklyResultCommand : IRequest
{
    [Required]
    public required string BowlerId { get; set; }

    [Required]
    public required string SeasonId { get; set; }

    [Required]
    public required int Week { get; set; }

    [Required]
    public required int? Score { get; set; }
}

public class UpdateWeeklyResultCommandHandler(IApplicationDbContext context)
    : IRequestHandler<UpdateWeeklyResultCommand>
{
    private readonly IApplicationDbContext _context = context;

    public async Task Handle(UpdateWeeklyResultCommand request, CancellationToken cancellationToken)
    {
        var existingWeeklyResult = await _context
            .Results.Where(r =>
                r.SeasonBowler.SeasonId == request.SeasonId
                && r.SeasonBowler.BowlerId == request.BowlerId
                && r.Week == request.Week
            )
            .FirstOrDefaultAsync(cancellationToken);

        if (existingWeeklyResult == null)
        {
            // if the result doesnt exists beforehand and we get null score we don't have to update anything
            if (request.Score == null)
                return;

            var seasonBowlerId = await _context
                .SeasonBowlers.Where(sb => sb.SeasonId == request.SeasonId && sb.BowlerId == request.BowlerId)
                .Select(sb => sb.Id)
                .SingleAsync(cancellationToken);

            var newWeeklyResult = new Result((int)request.Score, request.Week, seasonBowlerId, request.SeasonId);

            _context.Results.Add(newWeeklyResult);
        }
        else
        {
            // Result exists already
            if (request.Score == null || request.Score == 0)
            {
                _context.Results.Remove(existingWeeklyResult);
            }
            else
            {
                existingWeeklyResult.Score = (int)request.Score;
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        var currentSeason = await _context
            .Seasons.Include(s => s.SeasonBowlers)
            .ThenInclude(sb => sb.Results)
            .SingleAsync(s => request.SeasonId == s.Id, cancellationToken);

        var previousSeason = await _context
            .Seasons.Include(s => s.SeasonBowlers)
            .ThenInclude(sb => sb.Results)
            .FirstOrDefaultAsync(s => s.Number == currentSeason.Number - 1, cancellationToken);

        currentSeason.UpdateSeasonAndSeasonBowlerStatistics(currentSeason, previousSeason);

        await _context.SaveChangesAsync(cancellationToken);
    }
}
