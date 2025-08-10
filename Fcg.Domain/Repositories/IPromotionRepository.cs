using Fcg.Domain.Entities;
using System.Threading.Tasks;

namespace Fcg.Domain.Repositories
{
    public interface IPromotionRepository
    {
        Task UpdateAsync(Promotion promotion);
    }
}