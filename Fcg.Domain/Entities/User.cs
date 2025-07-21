namespace Fcg.Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; }
        public string Role { get; set; }

        public User(string name, string email, string role = "User")
        {
            Id = Guid.NewGuid();
            Name = name;
            Email = email;
            Role = role;
        }

        public User(Guid id,string name, string email, string passwordHash, string role = "User")
        {
            Id = id;
            Name = name;
            Email = email;
            Role = role;
            PasswordHash = passwordHash;
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
