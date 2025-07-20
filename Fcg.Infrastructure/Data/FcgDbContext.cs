using Fcg.Infrastructure.Tables;
using Microsoft.EntityFrameworkCore;

namespace Fcg.Infrastructure.Data
{
    public class FcgDbContext : DbContext
    {
        public FcgDbContext(DbContextOptions<FcgDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Game> Games { get; set; }
        //public DbSet<Promotion> Promotions { get; set; }
    }
}