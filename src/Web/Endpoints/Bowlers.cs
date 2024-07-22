using BowlingApp.Application.Bowlers.Commands.CreateBowler;
using BowlingApp.Application.Bowlers.Queries.GetBowlerReport;
using BowlingApp.Application.Bowlers.Queries.GetBowlers;
using BowlingApp.Application.WeeklyResults.Queries.GetMostRecentWeekResults;

namespace BowlingApp.Web.Endpoints;

public class Bowlers : EndpointGroupBase
{
    public override void Map(WebApplication app)
    {
        app.MapGroup(this).MapGet(GetBowlers).MapGet(GetBowlerReport, "Report");
        app.MapGroup(this).RequireAuthorization().MapPost(CreateBowler).MapDelete(DeleteBowler, "id");
    }

    public async Task<WeeklyResultsDto> GetMostRecentWeekResults(ISender sender)
    {
        return await sender.Send(new GetMostRecentWeekResultsQuery());
    }

    public async Task<IEnumerable<BowlerDto>> GetBowlers(ISender sender)
    {
        return await sender.Send(new GetBowlersQuery());
    }

    public async Task<BowlerDto> CreateBowler(ISender sender, CreateBowlerCommand command)
    {
        return await sender.Send(command);
    }

    public async Task<IResult> DeleteBowler(ISender sender, string id)
    {
        await sender.Send(new DeleteBowlerCommand(id));
        return Results.NoContent();
    }

    public async Task<BowlerReport> GetBowlerReport(ISender sender, string bowlerId, string? seasonId)
    {
        seasonId = string.IsNullOrWhiteSpace(seasonId) ? null : seasonId;
        return await sender.Send(new GetBowlerReportQuery(bowlerId, seasonId));
    }
}
