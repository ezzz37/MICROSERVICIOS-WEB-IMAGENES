using AuthService.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        { }

        public DbSet<Usuario> Usuarios { get; set; } = null!;

        // DbSet para llamar al SP
        public DbSet<LoginResult> LoginResults { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder mb)
        {
            // Usuario sigue igual
            mb.Entity<Usuario>()
              .HasIndex(u => u.Username)
              .IsUnique();

            // Configuramos LoginResult como entidad sin clave y sin tabla propia
            mb.Entity<LoginResult>()
              .HasNoKey()
              .ToView(null);     // <-- evita mapear a una vista real

            base.OnModelCreating(mb);
        }
    }
}
