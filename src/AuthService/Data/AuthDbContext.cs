using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> opts)
          : base(opts) { }

        public DbSet<Usuario> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder model)
        {
            model.Entity<Usuario>()
                 .HasIndex(u => u.Username)
                 .IsUnique();
        }
    }
}
