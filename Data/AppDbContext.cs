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
        public DbSet<Lider> Lideres { get; set; }
        public DbSet<CatalogoVacacionesAntiguedad> CatalogoVacacionesAntiguedad { get; set; }

        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Empleado>().ToTable("empleado");
            modelBuilder.Entity<Usuario>().ToTable("usuario");
            modelBuilder.Entity<Equipo>().ToTable("equipo");
            modelBuilder.Entity<Lider>().ToTable("lider");

            modelBuilder.Entity<CatalogoVacacionesAntiguedad>()
                .ToTable("catalogo_vacaciones_antiguedad");
        }
    }
}