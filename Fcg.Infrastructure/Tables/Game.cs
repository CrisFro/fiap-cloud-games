namespace Fcg.Infrastructure.Tables
{
    public class Game
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Genre { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public decimal Price { get; set; }
    }
}
