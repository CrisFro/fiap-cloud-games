namespace Fcg.Domain.Entities
{
    public class Promotion
    {
        public Guid Id { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }
        public decimal DiscountPercent { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        // Constructor for creating a new Promotion.
        public Promotion(string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate)
        {
            // Enforce invariants at creation
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title Não pode ser vazio ou nulo.", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição Não pode ser vazio ou nulo.", nameof(description));
            if (discountPercent <= 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), "O percemtual de Desconto deve estar entre 0 e 100.");
            if (startDate == default)
                throw new ArgumentException("A data de início deve ser informada.", nameof(startDate));
            if (endDate == default)
                throw new ArgumentException("A data de fim deve ser informada.", nameof(endDate));
            if (startDate >= endDate)
                throw new ArgumentException("A data de início deve ser menor ou igual a data fim.", nameof(startDate));

            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            DiscountPercent = discountPercent;
            StartDate = startDate.ToUniversalTime(); // Store as UTC
            EndDate = endDate.ToUniversalTime();     // Store as UTC
        }

        // Constructor for rehydrating an existing Promotion.
        public Promotion(Guid id, string title, string description, decimal discountPercent, DateTime startDate, DateTime endDate)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id não pode ser vazio.", nameof(id));
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title não pode ser vazio.", nameof(title));
            if (discountPercent <= 0 || discountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(discountPercent), "O percentual de desconto deve estar entre 0 e 100.");
            if (startDate == default)
                throw new ArgumentException("A data início deve ser informada.", nameof(startDate));
            if (endDate == default)
                throw new ArgumentException("A data de fim deve ser informada.", nameof(endDate));
            if (startDate >= endDate)
                throw new ArgumentException("A data de início deve ser menor ou igual a data fim.", nameof(startDate));

            Id = id;
            Title = title;
            Description = description;
            DiscountPercent = discountPercent;
            StartDate = startDate;
            EndDate = endDate;
        }

        // --- Behavior (Commands) ---

        public void UpdatePromotionDetails(string newTitle, string newDescription)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title não pode ser vazio ou nuo.", nameof(newTitle));
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("Descrição não pode ser vazio.", nameof(newDescription));

            Title = newTitle;
            Description = newDescription;
        }

        public void UpdateDiscount(decimal newDiscountPercent)
        {
            if (newDiscountPercent <= 0 || newDiscountPercent > 100)
                throw new ArgumentOutOfRangeException(nameof(newDiscountPercent), "O percentual de desconto deve estar entre 0 e 100.");

            DiscountPercent = newDiscountPercent;
        }

        public void UpdateDates(DateTime newStartDate, DateTime newEndDate)
        {
            if (newStartDate == default)
                throw new ArgumentException("A data de início deve ser informada.", nameof(newStartDate));
            if (newEndDate == default)
                throw new ArgumentException("A data de fim deve ser informada.", nameof(newEndDate));
            if (newStartDate >= newEndDate)
                throw new ArgumentException("A data de início deve ser menor ou igual a data fim.", nameof(newStartDate));
           

            StartDate = newStartDate.ToUniversalTime();
            EndDate = newEndDate.ToUniversalTime();
        }

        // --- Queries ---

        public bool IsActive(DateTime checkDate)
        {
            var utcCheckDate = checkDate.ToUniversalTime();
            return utcCheckDate >= StartDate && utcCheckDate <= EndDate;
        }
    }
}