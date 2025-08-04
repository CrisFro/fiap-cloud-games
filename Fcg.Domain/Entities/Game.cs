namespace Fcg.Domain.Entities
{
    public class Game
    {
        // Id is truly immutable, set only during construction.
        public Guid Id { get; }
        public string Title { get; private set; } 
        public string Description { get; private set; }
        public string Genre { get; private set; }
        public DateTime CreatedAt { get; } 
        public decimal Price { get; private set; }

        public Game(string title, string description, string genre, decimal price)
        {            
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Title não pode ser vazio ou nulo", nameof(title));
            if (string.IsNullOrWhiteSpace(description))
                throw new ArgumentException("Descrição não pode ser vazio ou nulo.", nameof(description));
            if (string.IsNullOrWhiteSpace(genre))
                throw new ArgumentException("Gênero não pode ser vazio ou nulo", nameof(genre));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Price não pode ser vazio ou nulo");

            Id = Guid.NewGuid();
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            CreatedAt = DateTime.UtcNow; 
        }

        public Game(Guid id, string title, string description, string genre, decimal price, DateTime createdAt)
        {
            if (id == Guid.Empty)
                throw new ArgumentException("Id  não pode ser vazio", nameof(id));
            if (string.IsNullOrWhiteSpace(title)) 
                throw new ArgumentException("Title não pode ser vazio ou nulo.", nameof(title));
            if (price < 0)
                throw new ArgumentOutOfRangeException(nameof(price), "Preço  não pode ser vazio ou nulo..");
            

            Id = id;
            Title = title;
            Description = description;
            Genre = genre;
            Price = price;
            CreatedAt = createdAt;
        }
        public void UpdateDetails(string newTitle, string newDescription, string newGenre)
        {
            if (string.IsNullOrWhiteSpace(newTitle))
                throw new ArgumentException("Title não pode ser vazio ou nulo.", nameof(newTitle));
            if (string.IsNullOrWhiteSpace(newDescription))
                throw new ArgumentException("Descrição  não pode ser vazio ou nulo..", nameof(newDescription));
            if (string.IsNullOrWhiteSpace(newGenre))
                throw new ArgumentException("Gênero não pode ser vazio ou nulo.", nameof(newGenre));

            Title = newTitle;
            Description = newDescription;
            Genre = newGenre;
        }

        public void ChangePrice(decimal newPrice)
        {
            if (newPrice < 0)
                throw new ArgumentOutOfRangeException(nameof(newPrice), "Preço não pode ser negativo.");

            Price = newPrice;
        }
   }
}