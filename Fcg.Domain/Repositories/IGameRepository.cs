using Fcg.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace Fcg.Domain.Repositories
{
    public interface IGameRepository
    {
        Task CreateAsync(Game game);
        Task<Game?> GetByIdAsync(Guid id);

        // Add this method to support fetching multiple games by their IDs
        Task<IEnumerable<Game>> GetGamesByIdsAsync(IEnumerable<Guid> ids);
    }
}