namespace Fcg.Domain.Entities
{
    public class Promotion
    {
        public Guid Id { get;}
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public decimal DiscountPercent { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public Promotion(string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            DiscountPercent = discountPercent;
            StartDate = startDate;
            EndDate = endDate;
        }

        public Promotion(Guid id, string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate)
        {
            Id = id;
            Title = title;
            Description = description;
            DiscountPercent = discountPercent;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
