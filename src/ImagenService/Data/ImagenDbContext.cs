using Microsoft.EntityFrameworkCore;
using Backend.Models;

namespace ImagenService.Data
{
    public class ImagenDbContext : DbContext
    {
        public ImagenDbContext(DbContextOptions<ImagenDbContext> options) : base(options) { }

        public DbSet<Imagen> Imagenes { get; set; }
        public DbSet<Comparacion> Comparaciones { get; set; }
        public DbSet<ImagenProcesada> ImagenesProcesadas { get; set; }
        public DbSet<AlgoritmoCompresion> AlgoritmosCompresion { get; set; }
    }
}
