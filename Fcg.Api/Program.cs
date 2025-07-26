using Fcg.Api.Middlewares;
using Fcg.Application.Requests;
using Fcg.Application.Services;
using Fcg.Domain.Queries;
using Fcg.Domain.Repositories;
using Fcg.Domain.Services;
using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Queries;
using Fcg.Infrastructure.Repositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

#region Services Configuration

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

#region Swagger Configuration
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Insira o token JWT com prefixo 'Bearer '"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
#endregion

#region Database
builder.Services.AddDbContext<FcgDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
#endregion

#region MediatR
builder.Services.AddMediatR(fcg =>
    fcg.RegisterServicesFromAssemblyContaining<CreateUserRequest>());
#endregion

#region Authentication & Authorization
builder.Services.AddAuthentication("Bearer")
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
});
#endregion

#region Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IPromotionRepository, PromotionRepository>();
#endregion

#region Queries
builder.Services.AddScoped<IUserQuery, UserQuery>();
builder.Services.AddScoped<IGameQuery, GameQuery>();
builder.Services.AddScoped<IPromotionQuery, PromotionQuery>();
#endregion

#region Domain Services
builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
builder.Services.AddScoped<IJwtTokenService, JwtTokenService>();
#endregion

#endregion

var app = builder.Build();


#region Minimal APIs

#region User Endpoints
app.MapPost("/api/users", async (CreateUserRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/users/{response.UserId}", response)
        : Results.BadRequest(response!.Message);
}).AllowAnonymous();

app.MapPost("/api/login", async (LoginRequest request, IMediator mediator) =>
{
    var response = await mediator.Send(request);

    if (!response.Success)
        return Results.Json(new { response.Message }, statusCode: StatusCodes.Status401Unauthorized);

    return Results.Ok(new
    {
        response.Token,
        response.UserId,
        response.Email,
        response.Message
    });
}).AllowAnonymous();

app.MapGet("/api/users/{id}", async (Guid id, IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetByIdUserAsync(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/api/users/{id}/games", async (Guid id, IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetLibraryByUserAsync(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
}).RequireAuthorization();

app.MapPut("/api/users/{id}/role", async (Guid id, UpdateRoleRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/users/{response.UserId}", response)
        : Results.BadRequest(response!.Message);
}).RequireAuthorization("AdminPolicy");

app.MapGet("/api/users", async (IUserQuery _userQuery) =>
{
    var users = await _userQuery.GetAllUsersAsync();

    return users is not null ? Results.Ok(users) : Results.NotFound();
}).RequireAuthorization("AdminPolicy");
#endregion

#region Game Endpoints
app.MapGet("/api/games/{id}", async (Guid id, IGameQuery _gameQuery) =>
{
    var game = await _gameQuery.GetByIdGameAsync(id);

    return game is not null ? Results.Ok(game) : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/api/games", async (IGameQuery _gameQuery) =>
{
    var games = await _gameQuery.GetAllGamesAsync();

    return games is not null ? Results.Ok(games) : Results.NotFound();
}).RequireAuthorization();

app.MapPost("/api/games", async (CreateGameRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/games/{response.GameId}", response)
        : Results.BadRequest(response!.Message);
}).RequireAuthorization("AdminPolicy");

app.MapPost("/api/games/buy", async (BuyGameRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/users/{response.UserId}/games", response)
        : Results.BadRequest(response!.Message);
}).RequireAuthorization();
#endregion

#region Promotion Endpoints
app.MapGet("/api/promotions/{id}", async (Guid id, IPromotionQuery _promotionQuery) =>
{
    var promotion = await _promotionQuery.GetByIdPromotionAsync(id);

    return promotion is not null ? Results.Ok(promotion) : Results.NotFound();
}).RequireAuthorization();

app.MapGet("/api/promotions", async (IPromotionQuery _promotionQuery) =>
{
    var games = await _promotionQuery.GetAllPromotionsAsync();

    return games is not null ? Results.Ok(games) : Results.NotFound();
}).RequireAuthorization("AdminPolicy");

app.MapPost("/api/promotions", async (CreatePromotionRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/promotions/{response.PromotionId}", response)
        : Results.BadRequest(response!.Message);
}).RequireAuthorization("AdminPolicy");

#endregion

#endregion

#region Middleware Pipeline

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseLogMiddleware();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

#endregion

app.Run();
