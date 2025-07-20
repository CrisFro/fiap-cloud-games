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
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<FcgDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(fcg =>
    fcg.RegisterServicesFromAssemblyContaining<CreateUserRequest>());

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();

// Queries
builder.Services.AddScoped<IUserQuery, UserQuery>();
builder.Services.AddScoped<IGameQuery, GameQuery>();

builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();

var app = builder.Build();

#region MinimalApi

#region User
app.MapGet("/api/users/{id}", async (Guid id, IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetByIdUserAsync(id);

    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapGet("/api/users", async (IUserQuery _userQuery) =>
{
    var user = await _userQuery.GetAllUsersAsync();

    return user is not null ? Results.Ok(user) : Results.NotFound();
});

app.MapPost("/api/users", async (CreateUserRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/users/{response.UserId}", response)
        : Results.BadRequest(response!.Message);
});
#endregion

#region Game
app.MapGet("/api/games/{id}", async (Guid id, IGameQuery _gameQuery) =>
{
    var game = await _gameQuery.GetByIdGameAsync(id);

    return game is not null ? Results.Ok(game) : Results.NotFound();
});

app.MapGet("/api/games", async (IGameQuery _gameQuery) =>
{
    var games = await _gameQuery.GetAllGamesAsync();

    return games is not null ? Results.Ok(games) : Results.NotFound();
});

app.MapPost("/api/games", async (CreateGameRequest request, IMediator _mediator) =>
{
    var response = await _mediator.Send(request);

    return response is not null
        ? Results.Created($"/api/games/{response.GameId}", response)
        : Results.BadRequest(response!.Message);
});
#endregion

#region Promotion

#endregion

#endregion


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseLogMiddleware();

app.UseAuthorization();

app.MapControllers();

app.Run();
