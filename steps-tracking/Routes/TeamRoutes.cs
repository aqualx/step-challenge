using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

public static class TeamRoutes
{
    public static void Init(WebApplication app)
    {
        var teamGroup = app.MapGroup("/teams").WithTags("Team endpoints");

        teamGroup.MapPost("/", CreateTeam)
            .WithName("CreateTeam")
            .WithDescription("Create a new team");

        teamGroup.MapGet("/", GetTeams)
            .WithName("GetAllTeams")
            .WithDescription("Get all registred teams");

        teamGroup.MapDelete("/{teamId}", DeleteTeam)
            .WithName("DeleteTeam")
            .WithDescription("Delete team by it's id.\nNote: All results for the team will be lost. Only users with admin right can use this endpoint");
    }

    static async Task<IResult> CreateTeam(TeamRequestDTO teamRequest, ChallengeContext db, CancellationToken cancellationToken)
    {
        if (await db.Teams.FirstOrDefaultAsync(x => x.Name == teamRequest.Name, cancellationToken) is not null)
        {
            return TypedResults.BadRequest(string.Format("Team with Name='{0}' already exists", teamRequest.Name));
        }

        var team = new Team { Name = teamRequest.Name };
        db.Teams.Add(team);
        await db.SaveChangesAsync(cancellationToken);

        return TypedResults.Created($"/teams/{team.Id}", team.ToTeamDTO());
    }

    static async Task<IResult> GetTeams(ChallengeContext db, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await db.Teams
            .Include(t => t.Users)
            .Select(team => new TeamDTO
            {
                Id = team.Id,
                Name = team.Name,
                Users = team.Users.Select(user => user.ToUserDTO()).ToList()
            })
            .ToListAsync(cancellationToken));
    }

    [Authorize]
    private static async Task<IResult> DeleteTeam(string teamId, ChallengeContext db, CancellationToken cancellationToken, ClaimsPrincipal userPrincipal)
    {
        if ((await db.Users.FindAsync(userPrincipal.Identity?.Name, cancellationToken))?.UserRole != Role.Admin)
        {
            return TypedResults.Forbid();
        }

        if (await db.Teams.FindAsync(teamId, cancellationToken) is Team team)
        {
            db.Teams.Remove(team);
            await db.SaveChangesAsync();
            return TypedResults.NoContent();
        }

        return TypedResults.NotFound(string.Format("Team with Id='{0}' not found", teamId));
    }
}