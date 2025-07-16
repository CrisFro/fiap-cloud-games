using Fcg.Application.DTOs;
using Fcg.Application.Interfaces;
using Fcg.Domain.Entities;

namespace Fcg.Infrastructure.Services;

public class GameService : IGameService
{
    private static readonly List<Game> _Games = new();

    public async Task<RegisterGameResponse> RegisterGameAsync(RegisterGameRequest request)
    {
        var exists = _Games.Any(u => u.Title == request.Title);

        if (exists)
        {
            return new RegisterGameResponse
            {
                Success = false,
                Message = "Cadastro de jogo já existente"
            };
        }

        var Game = new Game
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.Parse(request.CreatedAt),
            Genre = request.Genre,
            Price = request.Price,
        };

        _Games.Add(Game);

        return new RegisterGameResponse
        {
            Success = true,
            Message = "Jogo registrado com sucesso.",
            GameId = Game.Id
        };
    }

    public async Task<List<GameResponse>> GetAllGamesAsync()
    {
        return [.. _Games.Select(g => new GameResponse
        {
            Id = g.Id,
            Title = g.Title,
            Description = g.Description,
            Genre = g.Genre,
            CreatedAt = g.CreatedAt,
            Price = g.Price
        })];
    }
}
