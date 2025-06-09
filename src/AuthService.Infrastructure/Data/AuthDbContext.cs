using Microsoft.EntityFrameworkCore;
using AuthService.Infrastructure.Entities;

namespace AuthService.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> opts)
            : base(opts)
        { }

        public DbSet<UsuarioEntity> Usuarios { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UsuarioEntity>()
                        .ToTable("Usuarios")
                        .HasKey(u => u.IdUsuario);

            base.OnModelCreating(modelBuilder);
        }
    }
}
