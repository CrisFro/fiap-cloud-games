using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IPromotionRepository
    {
        Task<Guid> CreatePromotionAsync(Promotion promotion);
        Task<Promotion?> GetPromotionByTitleAsync(string title);
    }
}
