using Microsoft.EntityFrameworkCore;
using swacv_api.Models;

namespace swacv_api.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) {}

        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Equipo> Equipos { get; set; }
        public DbSet<Empleado> Empleados { get; set; }
        public DbSet<Vacaciones> Vacaciones { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Empleado>().ToTable("empleado");
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Equipo>().ToTable("equipo");
        }
    }
}