using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ChallengeContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"))
);
builder.Services.AddOpenApi();

// response error formating
builder.Services.AddProblemDetails(options =>
{
    options.CustomizeProblemDetails = context =>
    {
        context.ProblemDetails.Instance = $"{context.HttpContext.Request.Method} {context.HttpContext.Request.Path}";
        context.ProblemDetails.Extensions.TryAdd("requestId", context.HttpContext.TraceIdentifier);
        var activity = context.HttpContext.Features.Get<IHttpActivityFeature>()?.Activity;
        context.ProblemDetails.Extensions.TryAdd("traceId", activity?.Id);
        context.ProblemDetails.Extensions.Add("nodeId", Environment.MachineName);
    };
});

#region Configure JWT
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
if (string.IsNullOrEmpty(jwtSettings?.Key))
{
    throw new Exception("JWT key is not configured.");
}
builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key))
        };
    }
);
builder.Services.AddAuthorization();
#endregion

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();
app.UseHsts();

// Handle routing initialization
AuthRoutes.Init(app);
UserRoutes.Init(app);
TeamRoutes.Init(app);
CounterRoutes.Init(app);
MainRoutes.Init(app);

// Map OpenAPI and Scalar APIs
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("Steps challange")
           .WithDefaultHttpClient(ScalarTarget.CSharp, ScalarClient.HttpClient); ;
    // TODO: Enable in production
    // options
    //        .WithPreferredScheme("ApiKey") // Security scheme name from the OpenAPI document
    //        .WithApiKeyAuthentication(apiKey =>
    //        {
    //            apiKey.Token = "SCALAR-API-TOKEN";
    //        })
}); // get ui under "/scalar/v1"

app.Run();