using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IGameRepository
    {
        Task<Guid> CreateGameAsync(Game game);
        Task<Game?> GetGameByTitleAsync(string title);
    }
}
