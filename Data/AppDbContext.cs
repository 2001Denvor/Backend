using Microsoft.EntityFrameworkCore;
using MyBackend.Models; // ✅ Important: so it knows what User is

namespace MyBackend.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
    }
}
