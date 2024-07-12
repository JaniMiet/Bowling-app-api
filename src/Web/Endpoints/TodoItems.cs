using BowlingApp.Application.Common.Models;
using BowlingApp.Application.TodoItems.Commands.CreateTodoItem;
using BowlingApp.Application.TodoItems.Commands.DeleteTodoItem;
using BowlingApp.Application.TodoItems.Commands.UpdateTodoItem;
using BowlingApp.Application.TodoItems.Commands.UpdateTodoItemDetail;
using BowlingApp.Application.TodoItems.Queries.GetTodoItemsWithPagination;

namespace BowlingApp.Web.Endpoints;

public class TodoItems : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .RequireAuthorization()
            .MapGet(GetTodoItemsWithPagination)
            .MapPost(CreateTodoItem)
            .MapPut(UpdateTodoItem, "{id}")
            .MapPut(UpdateTodoItemDetail, "UpdateDetail/{id}")
            .MapDelete(DeleteTodoItem, "{id}");
    }

    public Task<PaginatedList<TodoItemBriefDto>> GetTodoItemsWithPagination(
        ISender sender,
        [AsParameters] GetTodoItemsWithPaginationQuery query
    )
    {
        return sender.Send(query);
    }

    public Task<string> CreateTodoItem(ISender sender, CreateTodoItemCommand command)
    {
        return sender.Send(command);
    }

    public async Task<IResult> UpdateTodoItem(ISender sender, string id, UpdateTodoItemCommand command)
    {
        if (id != command.Id)
            return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> UpdateTodoItemDetail(ISender sender, string id, UpdateTodoItemDetailCommand command)
    {
        if (id != command.Id)
            return Results.BadRequest();
        await sender.Send(command);
        return Results.NoContent();
    }

    public async Task<IResult> DeleteTodoItem(ISender sender, string id)
    {
        await sender.Send(new DeleteTodoItemCommand(id));
        return Results.NoContent();
    }
}
