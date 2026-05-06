using Microsoft.EntityFrameworkCore;
using RestauranteAPI.Domain.Entities;

namespace RestauranteAPI.DataAccess.Context
{
    public class RestauranteDbContext : DbContext
    {
        // Constructor obligatorio para recibir las opciones de conexión
        public RestauranteDbContext(DbContextOptions<RestauranteDbContext> options) : base(options)
        {
        }

        // Estas propiedades DbSet se convertirán en las tablas de tu base de datos
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Table> Tables { get; set; }
        public DbSet<MenuItem> MenuItems { get; set; }
        public DbSet<Reservation> Reservations { get; set; }
        public DbSet<ReservationMenuItem> ReservationMenuItems { get; set; }

        // Fluent API: Aquí configuramos reglas estrictas de la base de datos
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. Configuración de la llave primaria compuesta para la tabla intermedia N:M
            modelBuilder.Entity<ReservationMenuItem>()
                .HasKey(rm => new { rm.ReservationId, rm.MenuItemId });

            // 2. Especificar la precisión de los decimales para evitar errores y advertencias en SQL Server
            modelBuilder.Entity<ReservationMenuItem>()
                .Property(rm => rm.UnitPrice)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<MenuItem>()
                .Property(m => m.Price)
                .HasColumnType("decimal(18,2)");

            // 3. Regla de negocio: El email del cliente debe ser único
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.Email)
                .IsUnique();
        }
    }
}