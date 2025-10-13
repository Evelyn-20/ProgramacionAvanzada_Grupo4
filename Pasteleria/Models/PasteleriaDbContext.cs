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
        public DbSet<Cliente> Clientes => Set<Cliente>();

        protected override void OnModelCreating(ModelBuilder b)
        {
            base.OnModelCreating(b);

            // Producto
            b.Entity<Producto>().ToTable("Producto");//tuve que agregar esto sino la borran
            b.Entity<Producto>().HasKey(p => p.IdProducto);
            b.Entity<Producto>().Property(p => p.Precio).HasColumnType("decimal(18,2)");
            b.Entity<Producto>().Property(p => p.PorcentajeImpuesto).HasColumnType("decimal(5,2)");

            // Categoria
            b.Entity<Categoria>().ToTable("Categoria");
            b.Entity<Categoria>().HasKey(c => c.IdCategoria);

            // Relación
            b.Entity<Producto>()
              .HasOne<Categoria>()
              .WithMany()
              .HasForeignKey(p => p.IdCategoria)
              .OnDelete(DeleteBehavior.Restrict);

            // Cliente
            b.Entity<Cliente>().ToTable("Cliente"); 
            b.Entity<Cliente>().HasKey(c => c.IdCliente);
            b.Entity<Cliente>().Property(c => c.NombreCliente).IsRequired().HasMaxLength(100);
            b.Entity<Cliente>().Property(c => c.Cedula).IsRequired().HasMaxLength(20);
            b.Entity<Cliente>().HasIndex(c => c.Cedula).IsUnique();
            b.Entity<Cliente>().Property(c => c.Correo).IsRequired().HasMaxLength(100);
            b.Entity<Cliente>().Property(c => c.Telefono).HasMaxLength(20);
            b.Entity<Cliente>().Property(c => c.Direccion).HasMaxLength(200);
            b.Entity<Cliente>().Property(c => c.Contrasenna).HasMaxLength(255);
        }
    }
}