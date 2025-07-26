namespace Fcg.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; }
        public string Role { get; set; }
        public IEnumerable<UserGaming>? Library { get; set; } = new List<UserGaming>();
        public IEnumerable<UserGaming>? GamesAdded { get; set; } = new List<UserGaming>();
        public IEnumerable<UserGaming>? GamesRemoved { get; set; } = new List<UserGaming>();

        public User(string name, string email, string role = "User")
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Role = role;
            Library = new List<UserGaming>();
        }

        public User(Guid id,string name, string email, string passwordHash, IEnumerable<UserGaming> gameLibrary, string role = "User")
        {
            Id = id;
            Name = name;
            Email = email;
            PasswordHash = passwordHash;
            Library = gameLibrary;
            GamesAdded = [];
            GamesRemoved = [];
            Role = role;
        }

        public void UpdateGameLibrary(IEnumerable<UserGaming> gameLibrary)
        {
            var currentLibrary = Library?.ToList() ?? new List<UserGaming>();
            var updatedLibrary = gameLibrary?.ToList() ?? new List<UserGaming>();

            var currentGameIds = currentLibrary.Select(x => x.Game.Id).ToHashSet();
            var updatedGameIds = updatedLibrary.Select(x => x.Game.Id).ToHashSet();

            GamesAdded = updatedLibrary
                .Where(x => !currentGameIds.Contains(x.Game.Id))
                .ToList();

            GamesRemoved = currentLibrary
                .Where(x => !updatedGameIds.Contains(x.Game.Id))
                .ToList();

            Library = updatedLibrary;
        }

        public void SetPassword(string passwordHash)
        {
            PasswordHash = passwordHash;
        }

        public void SetRole(string role)
        {
            Role = role;
        }
    }
}
