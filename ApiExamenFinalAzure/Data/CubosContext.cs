using ApiExamenFinalAzure.Models;
using Microsoft.EntityFrameworkCore;

namespace ApiExamenFinalAzure.Data
{
    public class CubosContext : DbContext
    {
        public CubosContext(DbContextOptions options) : base(options) { }
        public DbSet<Cubo> Cubos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<CompraCubo> Compras { get; set; }
      
    }
}
