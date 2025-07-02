using Fcg.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Data
{
    public class FcgDbContext : DbContext
    {
        public FcgDbContext(DbContextOptions<FcgDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();
        public DbSet<Game> Games => Set<Game>();
    }
}