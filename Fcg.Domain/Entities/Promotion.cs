namespace Fcg.Domain.Entities
{
    public class Promotion
    {
        public Guid PromotionId { get; set; } = default!;
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public decimal DiscountPercent { get; set; } = default!;
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public DateTime EndDate { get; set; } = DateTime.UtcNow;
    }
}
