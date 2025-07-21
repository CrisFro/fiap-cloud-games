using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task UpdateUserRoleAsync(Guid userId, string newRole);
    }
}
