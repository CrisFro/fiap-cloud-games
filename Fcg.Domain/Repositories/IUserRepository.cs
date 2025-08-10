using Fcg.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Fcg.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<Guid> CreateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task<User?> GetUserByIdAsync(Guid id);
        Task UpdateAsync(User user);
        Task<bool> DeleteUserAsync(Guid userId);
    }
}