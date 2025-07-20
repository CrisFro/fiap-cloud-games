namespace Fcg.Domain.Entities
{
    public class Game
    {
        public Guid Id { get; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }

        public Game(string title, string description, string genre, decimal price)
        {
            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            CreatedAt = DateTime.UtcNow;
        }
        public Game(Guid id, string title, string description, string genre, decimal price, DateTime createAt)
        {
            Id = id;
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            CreatedAt = createAt;
        }
    }
}
