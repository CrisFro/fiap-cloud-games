using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly FcgDbContext _context;

        public UserRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreateUserAsync(User user)
        {
            var entity = new Tables.User
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                PasswordHash = user.PasswordHash,
                Role = user.Role
            };

            _context.Users.Add(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            var user = await (from u in _context.Users
                              join ug in _context.UserGamings on u.Id equals ug.UserId into games
                              from g in games.DefaultIfEmpty()
                              where u.Email == email
                              select new User(
                                  u.Id,
                                  u.Name,
                                  u.Email,
                                  u.PasswordHash,
                                  u.Library == null ?
                                  new List<UserGaming>() :
                                  u.Library.Select(x => new UserGaming(
                                      new Game(
                                          x.Game.Id,
                                          x.Game.Title,
                                          x.Game.Description,
                                          x.Game.Genre,
                                          x.Game.Price,
                                          x.Game.CreatedAt),
                                      x.PurchasedDate)).ToList(),
                                  u.Role))
                              .FirstOrDefaultAsync();

            return user;
        }

        public async Task<User?> GetUserByIdAsync(Guid id)
        {
            var user = await (from u in _context.Users
                              join ug in _context.UserGamings on u.Id equals ug.UserId into games
                              from g in games.DefaultIfEmpty()
                              where u.Id == id
                              select new User(
                                  u.Id,
                                  u.Name,
                                  u.Email,
                                  u.PasswordHash,
                                  u.Library == null ?
                                  new List<UserGaming>() :
                                  u.Library.Select(x => new UserGaming(
                                      new Game(
                                          x.Game.Id,
                                          x.Game.Title,
                                          x.Game.Description,
                                          x.Game.Genre,
                                          x.Game.Price,
                                          x.Game.CreatedAt),
                                      x.PurchasedDate)).ToList(),
                                  u.Role))
                              .FirstOrDefaultAsync();

            return user;
        }

        public async Task UpdateUserLibraryAsync(User user)
        {
            if (user.GamesAdded != null && user.GamesAdded.Any())
            {
                var userGamingEntities = user.GamesAdded.Select(ug => new Tables.UserGaming
                {
                    Id = ug.Id,
                    UserId = user.Id,
                    GameId = ug.Game.Id,
                    PurchasedDate = ug.PurchasedDate
                });

                _context.UserGamings.AddRange(userGamingEntities);
            }

            if (user.GamesRemoved != null && user.GamesRemoved.Any())
            {
                var userGamingEntities = user.GamesRemoved.Select(ug => new Tables.UserGaming
                {
                    Id = ug.Id,
                    UserId = user.Id,
                    GameId = ug.Game.Id,
                    PurchasedDate = ug.PurchasedDate
                });

                _context.UserGamings.RemoveRange(userGamingEntities);
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserRoleAsync(Guid userId, string newRole)
        {
            var entity = new Tables.User
            {
                Id = userId,
                Role = newRole
            };

            var entry = _context.Users.Entry(entity);

            entry.Property(e => e.Role).IsModified = true;

            await _context.SaveChangesAsync();
        }
    }
}
