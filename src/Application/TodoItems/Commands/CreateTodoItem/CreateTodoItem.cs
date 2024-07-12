using BowlingApp.Application.Common.Interfaces;
using BowlingApp.Domain.Entities;
using BowlingApp.Domain.Events;

namespace BowlingApp.Application.TodoItems.Commands.CreateTodoItem;

public record CreateTodoItemCommand : IRequest<string>
{
    public required string ListId { get; init; }

    public string? Title { get; init; }
}

public class CreateTodoItemCommandHandler : IRequestHandler<CreateTodoItemCommand, string>
{
    private readonly IApplicationDbContext _context;

    public CreateTodoItemCommandHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<string> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
    {
        var entity = new TodoItem
        {
            ListId = request.ListId,
            Title = request.Title,
            Done = false
        };

        entity.AddDomainEvent(new TodoItemCreatedEvent(entity));

        await _context.TodoItems.AddAsync(entity);

        await _context.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
}
