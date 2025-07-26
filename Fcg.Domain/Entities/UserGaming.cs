namespace Fcg.Domain.Entities
{
    public class UserGaming
    {
        public Guid Id { get; set; }
        public User User { get; set; } = null!;
        public Game Game { get; set; } = null!;
        public DateTime PurchasedDate { get; set; }

        public UserGaming(User user, Game game)
        {
            User = user;
            Game = game;
            PurchasedDate = DateTime.Now;
        }

        public UserGaming(Game game, DateTime purchasedDate)
        {
            Id = Guid.NewGuid();
            Game = game;
            PurchasedDate = purchasedDate;
        }
    }
}
