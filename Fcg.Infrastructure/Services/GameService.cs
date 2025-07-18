using Fcg.Application.DTOs;
using Fcg.Application.Interfaces;
using Fcg.Domain.Entities;
using System.Linq;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Fcg.Infrastructure.Services;

public class GameService : IGameService
{
    private readonly FcgDbContext _context;

    public GameService(FcgDbContext context)
    {
        _context = context;
    }

    public async Task<RegisterGameResponse> RegisterGameAsync(RegisterGameRequest request)
    {
        // var exists = _context.Any(u => u.Title == request.Title);

        // if (exists)
        // {
        //     return new RegisterGameResponse
        //     {
        //         Success = false,
        //         Message = "Cadastro de jogo já existente"
        //     };
        // }

        var newGame = new Game
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Description = request.Description,
            CreatedAt = DateTime.Parse(request.CreatedAt),
            Genre = request.Genre,
            Price = request.Price,
        };

        _context.Games.Add(newGame);
        await _context.SaveChangesAsync();

        return new RegisterGameResponse
        {
            Success = true,
            Message = "Jogo registrado com sucesso.",
            GameId = newGame.Id
        };
    }

    public async Task<List<GameResponse>> GetAllGamesAsync()
    {
        var games = await _context.Games.ToListAsync();

        return games.Select(game => new GameResponse
        {
            Id = game.Id,
            Title = game.Title,
            Description = game.Description,
            Genre = game.Genre,
            CreatedAt = game.CreatedAt,
            Price = game.Price

        })
        .ToList();
    }
}
