namespace Fcg.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = default!;
        public string Description { get; set; } = default!;
        public string Genre { get; set; } = default!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public decimal Price { get; set; } = default!;
    }
}
