using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

public static class CounterRoutes
{
    public static void Init(WebApplication app)
    {
        var counterGroup = app.MapGroup("/counters").WithTags("Counter endpoints");

        counterGroup.MapPost("/increment", IncrementUserCounter)
            .WithName("IncrementUserCounter")
            .WithDescription("Add new step counter for the user");

        counterGroup.MapGet("/team/{teamId}", GetTeamCounters)
            .WithName("GetTeamCounters")
            .WithDescription("Get team counters");

        counterGroup.MapGet("/user/{userId}", GetCountersByUserId)
            .WithName("GetCountersByUserId")
            .WithDescription("Get user by it's Id");

        counterGroup.MapGet("/", GetAllCounters)
            .WithName("GetAllCounters")
            .WithDescription("Get All Counters");

        counterGroup.MapDelete("/{counterId}", DeleteCounter)
            .WithName("DeleteCounter")
            .WithDescription("Delete counter by it's id.\nNote: Only counter owner can delete it.");
    }

    [Authorize]
    private static async Task<IResult> IncrementUserCounter(CounterRequestDTO counterRequest, ChallengeContext db, CancellationToken cancellationToken, ClaimsPrincipal userPrincipal)
    {
        if (counterRequest.UpdatedAt > DateTime.UtcNow)
        {
            return TypedResults.BadRequest("Date can't be from the future");
        }

        var userId = userPrincipal.Identity?.Name;
        if (userId is not null && await db.Users.FindAsync(userId, cancellationToken) is User user)
        {
            var counter = new Counter { Steps = counterRequest.Steps, UpdatedAt = counterRequest.UpdatedAt, UserId = userId };
            db.Counters.Add(counter);
            await db.SaveChangesAsync();
            return TypedResults.Created("counter/incrment", counter.ToCounterDTO());
        }
        return TypedResults.BadRequest("User is not valid anymore.");
    }

    private static async Task<IResult> GetTeamCounters(string teamId, ChallengeContext db, CancellationToken cancellationToken)
    {
        if (await db.Teams.FindAsync(teamId, cancellationToken) is Team team)
        {
            var teamUserIds = db.Users.Where(u => u.TeamId == teamId).Select(u => u.Id).ToList();
            var counters = db.Counters
                .AsEnumerable() // To process request on client (not on DB, as SQLite do not support some features needed)
                .Where(x => teamUserIds.Contains(x.UserId))
                .GroupBy(e => e.UserId)
                .Select(g => new
                {
                    userId = g.Key,
                    steps = g.Sum(e => e.Steps),
                    lastUpdate = g.Max(e => e.UpdatedAt),
                }
            ).ToList();
            var total = counters.Sum(e => e.steps);

            return TypedResults.Ok(new { Total = total, Counters = counters });
        }
        return TypedResults.NotFound(string.Format("Team with Id='{0}' not found", teamId));
    }

    private static async Task<IResult> GetCountersByUserId(string userId, ChallengeContext db, CancellationToken cancellationToken)
    {
        var counters = db.Counters.Where(x => x.UserId == userId).Select(x => x.ToCounterDTO());
        if (counters.Any())
        {
            var total = counters
                .AsEnumerable() // To process request on client (not on DB, as SQLite do not support some features needed)
                .Sum(x => x.Steps);
            return TypedResults.Ok(new { Total = total, Counters = counters });
        }
        return TypedResults.NotFound(string.Format("Counters for UserId='{0}' not found", userId));
    }

    private static async Task<IResult> GetAllCounters(ChallengeContext db, CancellationToken cancellationToken)
    {
        var counters = db.Counters.Select(x => x.ToCounterDTO());
        if (counters.Any())
        {
            var total = counters
                .AsEnumerable() // To process request on client (not on DB, as SQLite do not support some features needed)
                .Sum(x => x.Steps);
            return TypedResults.Ok(new { Total = total, Counters = counters });
        }
        return TypedResults.NotFound("Counters exist yet");
    }

    [Authorize]
    private static async Task<IResult> DeleteCounter(string counterId, ChallengeContext db, CancellationToken cancellationToken, ClaimsPrincipal userPrincipal)
    {
        if (await db.Counters.FindAsync(counterId, cancellationToken) is Counter counter)
        {
            if (userPrincipal.Identity?.Name != counter.UserId // Only owner
               ||
               (await db.Users.FindAsync(counter.UserId, cancellationToken))?.UserRole != Role.Admin // Or user with Admin rights
            )
            {
                return TypedResults.Forbid();
            }
            db.Counters.Remove(counter);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(string.Format("Counter with Id='{0}' not found", counterId));
    }
}