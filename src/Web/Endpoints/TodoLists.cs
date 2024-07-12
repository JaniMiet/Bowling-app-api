using BowlingApp.Application.TodoLists.Commands.CreateTodoList;
using BowlingApp.Application.TodoLists.Commands.DeleteTodoList;
using BowlingApp.Application.TodoLists.Commands.UpdateTodoList;
using BowlingApp.Application.TodoLists.Queries.GetTodos;

namespace BowlingApp.Web.Endpoints;

public class TodoLists : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoLists)
            .MapPost(CreateTodoList)
            .MapPut(UpdateTodoList, "{id}")
            .MapDelete(DeleteTodoList, "{id}");
    }

    public Task<TodosVm> GetTodoLists(ISender sender)
    {
        return sender.Send(new GetTodosQuery());
    }

    public Task<string> CreateTodoList(ISender sender, CreateTodoListCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateTodoList(ISender sender, string id, UpdateTodoListCommand command)
    {
        if (id != command.Id)
            return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoList(ISender sender, string id)
    {
        await sender.Send(new DeleteTodoListCommand(id));
        return Results.NoContent();
    }
}
