using Fcg.Domain.Entities;
using Fcg.Domain.Repositories;
using Fcg.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Repositories
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly FcgDbContext _context;

        public PromotionRepository(FcgDbContext context)
        {
            _context = context;
        }

        public async Task<Guid> CreatePromotionAsync(Promotion promotion)
        {
            var entity = new Tables.Promotion
            {
                Id = promotion.Id,
                Title = promotion.Title,
                Description = promotion.Description,
                DiscountPercent = promotion.DiscountPercent,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };

            _context.Promotions.Add(entity);

            await _context.SaveChangesAsync();

            return entity.Id;
        }

        public async Task<Promotion?> GetPromotionByTitleAsync(string title)
        {
            var promotion = await(from g in _context.Promotions
                                  where g.Title == title
                                  select new Promotion(
                                      g.Id,
                                      g.Title,
                                      g.Description,
                                      g.DiscountPercent,
                                      g.StartDate,
                                      g.EndDate))
                                 .FirstOrDefaultAsync();

            return promotion;
        }
    }
}
