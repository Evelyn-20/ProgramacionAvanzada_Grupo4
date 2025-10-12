using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;
using System.Reflection.Emit;
using Microsoft.EntityFrameworkCore;


namespace Pasteleria.Models
{
    public class PasteleriaDbContext : DbContext
    {
        public PasteleriaDbContext(DbContextOptions<PasteleriaDbContext> options) : base(options) { }

        public DbSet<Producto> Productos => Set<Producto>();
        public DbSet<Categoria> Categorias => Set<Categoria>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Producto
            b.Entity<Producto>().HasKey(p => p.IdProducto);
            b.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(18,2)");
            b.Entity<Producto>().Property(p => p.PorcentajeImpuesto).HasColumnType("decimal(5,2)");

            // Categoria
            b.Entity<Categoria>().HasKey(c => c.IdCategoria);

            // Relación
            b.Entity<Producto>()
              .HasOne<Categoria>()
              .WithMany()
              .HasForeignKey(p => p.IdCategoria)
              .OnDelete(DeleteBehavior.Restrict);
        }
    }
}