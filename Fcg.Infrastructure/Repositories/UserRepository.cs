using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Fcg.Infrastructure.Tables;
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

        public async Task<Guid> CreateUserAsync(Domain.Entities.User user)
        {
            var entity = new User
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

        public async Task<Domain.Entities.User?> GetUserByEmailAsync(string email)
        {
            var user = await (from u in _context.Users
                              where u.Email == email
                              select new Domain.Entities.User(u.Id, u.Name, u.Email, u.PasswordHash, u.Role))
                              .FirstOrDefaultAsync();

            return user;
        }

        public async Task<Domain.Entities.User?> GetUserByIdAsync(Guid id)
        {
            var user = await (from u in _context.Users
                              where u.Id == id
                              select new Domain.Entities.User(u.Id, u.Name, u.Email, u.PasswordHash, u.Role))
                              .FirstOrDefaultAsync();

            return user;
        }

        public async Task UpdateUserRoleAsync(Guid userId, string newRole)
        {
            var entity = new User
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
