using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using Fcg.Domain; // Add this if GameGenre is in Fcg.Domain namespace

namespace Fcg.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly FcgDbContext _context;

        public GameRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Game game)
        {
            var gameTable = new Tables.Game {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                Genre = (int)game.Genre,
                Price = game.Price,
                CreatedAt = game.CreatedAt
            };
            _context.Games.Add(gameTable);
            await _context.SaveChangesAsync();
        }

        public async Task<Game?> GetByIdAsync(System.Guid id)
        {
            var gameTable = await _context.Games.AsNoTracking().FirstOrDefaultAsync(g => g.Id == id);
            if (gameTable == null)
            {
                return null;
            }

            return new Game(
                gameTable.Id,
                gameTable.Title,
                gameTable.Description,
                (GenreEnum)gameTable.Genre, // Changed from GameGenre to GenreEnum
                gameTable.Price,
                gameTable.CreatedAt
            );
        }

        public async Task<IEnumerable<Game>> GetGamesByIdsAsync(IEnumerable<Guid> ids)
        {
            var gameTables = await _context.Games
                .AsNoTracking()
                .Where(g => ids.Contains(g.Id))
                .ToListAsync();

            return gameTables.Select(gameTable => new Game(
                gameTable.Id,
                gameTable.Title,
                gameTable.Description,
                (GenreEnum)gameTable.Genre,
                gameTable.Price,
                gameTable.CreatedAt
            ));
        }
    }
}