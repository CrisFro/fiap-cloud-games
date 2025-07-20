using Fcg.Domain.Entities;

namespace Fcg.Domain.Repositories
{
    public interface IPromotionRepository
    {
        Task<Promotion> CreatePromotionAsync(Promotion promotion);
    }
}
