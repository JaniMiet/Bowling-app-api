using BowlingApp.Application.Bowlers.Queries.GetBowlers;
using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;
using BowlingApp.Domain.Enums;

namespace BowlingApp.Application.Bowlers.Commands.CreateBowler;

public record CreateBowlerCommand : IRequest<BowlerDto>
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public Gender Gender { get; init; }
}

public class CreateBowlerCommandValidator : AbstractValidator<CreateBowlerCommand>
{
    public CreateBowlerCommandValidator() { }
}

public class CreateBowlerCommandHandler(IApplicationDbContext context) : IRequestHandler<CreateBowlerCommand, BowlerDto>
{
    private readonly IApplicationDbContext _context = context;

    public async Task<BowlerDto> Handle(CreateBowlerCommand request, CancellationToken cancellationToken)
    {
        var bowler = new Bowler
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Gender = request.Gender,
        };

        await _context.Bowlers.AddAsync(bowler);
        await _context.SaveChangesAsync(cancellationToken);

        return new BowlerDto
        {
            FirstName = bowler.FirstName,
            Id = bowler.Id,
            LastName = bowler.LastName,
            Gender = bowler.Gender,
        };
    }
}
