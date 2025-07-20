using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(User user);
        Task<User?> GetByEmailUserAsync(string email);
    }
}
