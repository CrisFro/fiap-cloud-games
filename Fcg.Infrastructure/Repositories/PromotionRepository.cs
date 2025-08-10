using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using System.Threading.Tasks;

namespace Fcg.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly FcgDbContext _context;

        public PromotionRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task UpdateAsync(Promotion promotion) => await _context.SaveChangesAsync();
    }
}