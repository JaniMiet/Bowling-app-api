using BowlingApp.Application.WeeklyResults.Commands.UpdateWeeklyResult;
using BowlingApp.Application.WeeklyResults.Queries.GetMostRecentWeekResults;
using BowlingApp.Application.WeeklyResults.Queries.GetWeeklyResultsByBowlerId;

namespace BowlingApp.Web.Endpoints;

public class WeeklyResults : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this)
            .MapGet(GetMostRecentWeekResults)
            .MapGet(GetWeeklyResultsByBowlerId, "ByBowlerId/${bowlerId}");
        app.MapGroup(this).RequireAuthorization().MapPut(UpdateWeeklyResult, "UpdateWeeklyResult");
    }

    public async Task<WeeklyResultsDto> GetMostRecentWeekResults(ISender sender)
    {
        return await sender.Send(new GetMostRecentWeekResultsQuery());
    }

    public async Task<IEnumerable<ResultDto>> GetWeeklyResultsByBowlerId(
        ISender sender,
        string bowlerId,
        string? seasonId
    )
    {
        return await sender.Send(new GetWeeklyResultsByBowlerIdQuery(bowlerId, seasonId));
    }

    public async Task<IResult> UpdateWeeklyResult(ISender sender, UpdateWeeklyResultCommand updateWeeklyResultCommand)
    {
        await sender.Send(updateWeeklyResultCommand);
        return Results.Ok();
    }
}
