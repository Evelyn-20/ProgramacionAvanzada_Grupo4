using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Pasteleria.AccesoADatos
{
    public class Contexto : DbContext
    {
        public Contexto()
        {
        }

        public Contexto(DbContextOptions<Contexto> options) : base(options)
        {
        }

        public DbSet<ProductoAD> Producto { get; set; }
        public DbSet<ClienteAD> Cliente { get; set; }
        public DbSet<AuditoriaAD> Auditoria { get; set; }
        public DbSet<CategoriaAD> Categoria { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Data Source=EVELYN\\SQLEXPRESS;Initial Catalog=PASTELERIA;Integrated Security=True;MultipleActiveResultSets=true;TrustServerCertificate=True");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configurar la tabla Producto
            modelBuilder.Entity<ProductoAD>(entity =>
            {
                entity.ToTable("Producto", "dbo");
                entity.HasKey(e => e.IdProducto);
                entity.Property(e => e.IdProducto)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.NombreProducto)
                    .IsRequired()
                    .HasMaxLength(200);
                entity.Property(e => e.DescripcionProducto)
                    .IsRequired()
                    .HasColumnType("TEXT");
                entity.Property(e => e.Precio)
                    .IsRequired()
                    .HasColumnType("decimal(10,2)");
                entity.Property(e => e.PorcentajeImpuesto)
                    .IsRequired()
                    .HasColumnType("decimal(5,2)");
                entity.Property(e => e.Imagen)
                    .HasColumnType("VARBINARY(MAX)");
                entity.Property(e => e.Estado)
                    .IsRequired();
                entity.Property(e => e.IdCategoria)
                    .IsRequired();
                entity.Property(e => e.Cantidad)
                    .IsRequired();
                entity.Property(e => e.FechaCreacion)
                    .IsRequired()
                    .HasColumnName("FechaCreacion")
                    .HasColumnType("datetime");
                entity.Property(e => e.FechaActualizacion)
                    .IsRequired()
                    .HasColumnName("FechaActualizacion")
                    .HasColumnType("datetime");
            });

            // Configurar la tabla Categoria
            modelBuilder.Entity<CategoriaAD>(entity =>
            {
                entity.ToTable("Categoria", "dbo");
                entity.HasKey(e => e.IdCategoria);
                entity.Property(e => e.IdCategoria)
                    .ValueGeneratedOnAdd();
                entity.Property(e => e.NombreCategoria)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.Imagen)
                    .IsRequired()
                    .HasColumnType("VARBINARY(MAX)");
                entity.Property(e => e.Estado)
                    .IsRequired();
            });

            modelBuilder.Entity<ClienteAD>(entity =>
            {
                entity.ToTable("Cliente");
                entity.HasKey(e => e.IdCliente);
                entity.Property(e => e.IdCliente)
                    .HasColumnName("IdCliente")
                    .UseIdentityColumn();
                entity.Property(e => e.NombreCliente)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("NombreCliente")
                    .HasColumnType("varchar(100)");
                entity.Property(e => e.Cedula)
                    .IsRequired()
                    .HasMaxLength(20)
                    .HasColumnName("Cedula")
                    .HasColumnType("varchar(20)");
                entity.Property(e => e.Correo)
                    .IsRequired()
                    .HasMaxLength(100)
                    .HasColumnName("Correo")
                    .HasColumnType("varchar(100)");
                entity.Property(e => e.Telefono)
                    .HasMaxLength(20)
                    .HasColumnName("Telefono")
                    .HasColumnType("varchar(20)");
                entity.Property(e => e.Direccion)
                    .HasMaxLength(200)
                    .HasColumnName("Direccion")
                    .HasColumnType("varchar(200)");
                entity.Property(e => e.Contrasenna)
                    .IsRequired()
                    .HasMaxLength(255)
                    .HasColumnName("Contrasenna")
                    .HasColumnType("varchar(255)");
                entity.Property(e => e.Estado)
                    .IsRequired()
                    .HasColumnName("Estado");
                entity.HasIndex(e => e.Cedula)
                    .IsUnique()
                    .HasDatabaseName("IX_Cliente_Cedula");
                entity.HasIndex(e => e.Correo)
                    .IsUnique()
                    .HasDatabaseName("IX_Cliente_Correo");
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}