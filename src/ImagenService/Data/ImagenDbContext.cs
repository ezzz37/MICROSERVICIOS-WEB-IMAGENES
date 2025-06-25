using Microsoft.EntityFrameworkCore;
using ImagenService.Models;

namespace ImagenService.Data
{
    public class ImagenDbContext : DbContext
    {
        public ImagenDbContext(DbContextOptions<ImagenDbContext> options)
            : base(options)
        {
        }

        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<ImagenProcesada> ImagenesProcesadas { get; set; }
        public DbSet<Comparacion> Comparaciones { get; set; }
        public DbSet<AlgoritmoCompresion> AlgoritmosCompresion { get; set; }

        // (Opcional) Solo si alguna vez usas el contexto sin pasarle opciones:
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(
                    "Server=mi-sql,1433;Database=DigitalizacionImagenesBD;" +
                    "User Id=sa;Password=YourStrong!Passw0rd;" +
                    "TrustServerCertificate=True;Encrypt=False"
                );
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1) Imagen → ImagenesProcesadas (cascade delete)
            modelBuilder.Entity<ImagenProcesada>()
                .HasOne(p => p.ImagenOriginal)
                .WithMany(i => i.ImagenesProcesadas)
                .HasForeignKey(p => p.IdImagenOriginal)
                .OnDelete(DeleteBehavior.Cascade);

            // 2) ImagenProcesada → Comparaciones (cascade delete)
            modelBuilder.Entity<Comparacion>()
                .HasOne(c => c.ImagenProcesada)
                .WithMany(p => p.Comparaciones)
                .HasForeignKey(c => c.IdImagenProcesada)
                .OnDelete(DeleteBehavior.Cascade);

            // 3) Imagen → Comparaciones (restrict delete)
            modelBuilder.Entity<Comparacion>()
                .HasOne(c => c.ImagenOriginal)
                .WithMany(i => i.ComparacionesOriginal)
                .HasForeignKey(c => c.IdImagenOriginal)
                .OnDelete(DeleteBehavior.Restrict);

            // 4) Seed de algoritmos de compresión
            modelBuilder.Entity<AlgoritmoCompresion>().HasData(
                new AlgoritmoCompresion { IdAlgoritmoCompresion = 1, NombreAlgoritmo = "JPEG" },
                new AlgoritmoCompresion { IdAlgoritmoCompresion = 2, NombreAlgoritmo = "PNG" },
                new AlgoritmoCompresion { IdAlgoritmoCompresion = 3, NombreAlgoritmo = "RLE" }
            );
        }
    }
}
