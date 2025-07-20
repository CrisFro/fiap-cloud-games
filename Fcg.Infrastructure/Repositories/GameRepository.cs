using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Repositories
{
    public class GameRepository : IGameRepository
    {
        private readonly FcgDbContext _context;

        public GameRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateGameAsync(Game game)
        {
            var entity = new Tables.Game
            {
                Id = game.Id,
                Title = game.Title,
                Description = game.Description,
                CreatedAt = game.CreatedAt,
                Genre = game.Genre,
                Price = game.Price
            };

            _context.Games.Add(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Game?> GetGameByTitleAsync(string title)
        {
            var game = await (from g in _context.Games
                              where g.Title == title
                              select new Game(
                                  g.Id, 
                                  g.Title, 
                                  g.Description, 
                                  g.Genre,
                                  g.Price,
                                  g.CreatedAt))
                              .FirstOrDefaultAsync();

            return game;
        }
    }
}
