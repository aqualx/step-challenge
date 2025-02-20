using Microsoft.EntityFrameworkCore;

public static class UserRoutes
{
    public static void Init(WebApplication app)
    {
        // TODO: wrap with a protective route. Should be available only for admin
        var userGroup = app.MapGroup("/users").WithTags("User endpoints");
        userGroup.MapPost("/", CreateUser)
            .WithName("CreateUser")
            .WithDescription("Create a new user and assign it to existing team");

        userGroup.MapGet("/", GetUsers)
            .WithName("GetAllUsers")
            .WithDescription("Get all user entities");

        userGroup.MapGet("/{userId}", GetUserById)
            .WithName("GetUserById")
            .WithDescription("Get user by it's Id");
    }

    static async Task<IResult> CreateUser(UserRequestDTO userRequest, ChallengeContext db, CancellationToken cancellationToken)
    {
        if (await db.Users.FirstOrDefaultAsync(x => x.Name == userRequest.Name, cancellationToken) is not null)
        {
            return TypedResults.BadRequest(string.Format("User with Name='{0}' already exists", userRequest.Name));
        }

        if (await db.Teams.FindAsync(userRequest.TeamId, cancellationToken) is Team team)
        {
            var user = new User { Name = userRequest.Name, TeamId = userRequest.TeamId, Password = userRequest.Password };
            db.Users.Add(user);
            await db.SaveChangesAsync(cancellationToken);

            return TypedResults.Created($"/users/{user.Id}", user.ToUserDTO());
        }

        return TypedResults.BadRequest(string.Format("Group with Id='{0}' doesn't exist", userRequest.TeamId));
    }

    static async Task<IResult> GetUsers(ChallengeContext db, CancellationToken cancellationToken)
    {
        return TypedResults.Ok(await db.Users.Select(x => x.ToUserDTO()).ToListAsync(cancellationToken));
    }

    private static async Task<IResult> GetUserById(string userId, ChallengeContext db, CancellationToken cancellationToken)
    {
        if (await db.Users.FindAsync(userId, cancellationToken) is User user)
        {
            return TypedResults.Ok(user.ToUserDTO());
        }
        return TypedResults.NotFound(string.Format("User with Id='{0}' not found", userId));
    }

}