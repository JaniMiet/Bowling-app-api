using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;

namespace BowlingApp.Application.TodoLists.Commands.CreateTodoList;

public record CreateTodoListCommand : IRequest<string>
{
    public string? Title { get; init; }
}

public class CreateTodoListCommandHandler : IRequestHandler<CreateTodoListCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoListCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateTodoListCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoList();

        entity.Title = request.Title;

        await _context.TodoLists.AddAsync(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
