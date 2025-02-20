using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public static class AuthRoutes
{
    public static void Init(WebApplication app)
    {
        app.MapPost("/login", async (UserLoginRequestDTO login, ChallengeContext db,
                                     CancellationToken cancellationToken, IOptions<JwtSettings> jwtSettings) =>
        {
            if (await db.Users.FirstOrDefaultAsync(u => u.Name == login.Username, cancellationToken) is User user &&
                user.Password == login.Password)
            {
                var claims = new[] { new Claim(ClaimTypes.Name, user.Id), };

                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Value.Key));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: DateTime.UtcNow.AddHours(1),
                    signingCredentials: creds);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new { Token = tokenString });
            }
            return Results.Unauthorized();
        });
    }
}