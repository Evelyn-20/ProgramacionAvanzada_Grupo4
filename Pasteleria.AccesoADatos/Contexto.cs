using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;

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
    public DbSet<RolAD> Rol { get; set; }
    public DbSet<UsuarioAD> Usuario { get; set; }
    public DbSet<PedidoAD> Pedido { get; set; }
    public DbSet<DetallePedidoAD> DetallePedido { get; set; }
    public DbSet<EstadoPedidoAD> EstadoPedido { get; set; }


    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            // Leer la cadena de conexión de Azure o usar la local
            string connectionString = Environment.GetEnvironmentVariable("SQLAZURECONNSTR_Contexto")
                ?? Environment.GetEnvironmentVariable("SQLCONNSTR_Contexto")
                ?? "Data Source=EVELYN\\SQLEXPRESS;Initial Catalog=PASTELERIA;Integrated Security=True;TrustServerCertificate=True";

            optionsBuilder.UseSqlServer(
                connectionString,
                sqlServerOptions =>
                {
                    // Aumentar el timeout de comandos a 160 segundos
                    sqlServerOptions.CommandTimeout(160);
                    // Habilitar retry logic para conexiones inestables
                    sqlServerOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: System.TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                });
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Configurar la tabla Producto
        modelBuilder.Entity<ProductoAD>(entity =>
        {
            entity.ToTable("Producto", "dbo");
            entity.HasKey(e => e.IdProducto);
            entity.Property(e => e.IdProducto).ValueGeneratedOnAdd();
            entity.Property(e => e.NombreProducto).IsRequired().HasMaxLength(200);
            entity.Property(e => e.DescripcionProducto).IsRequired().HasColumnType("TEXT");
            entity.Property(e => e.Precio).IsRequired().HasColumnType("decimal(10,2)");

            // Configuración del descuento (NULLABLE - OPCIONAL)
            entity.Property(e => e.PorcentajeDescuento)
                .IsRequired(false)  // Explícitamente opcional
                .HasColumnType("decimal(5,2)")
                .HasDefaultValue(null);  // Valor por defecto NULL

            entity.Property(e => e.PorcentajeImpuesto).IsRequired().HasColumnType("decimal(5,2)");
            entity.Property(e => e.Imagen).HasColumnType("VARBINARY(MAX)");
            entity.Property(e => e.ImagenThumbnail).HasColumnType("VARBINARY(MAX)");
            entity.Property(e => e.Estado).IsRequired();
            entity.Property(e => e.IdCategoria).IsRequired();
            entity.Property(e => e.Cantidad).IsRequired();
            entity.Property(e => e.FechaCreacion).IsRequired().HasColumnName("FechaCreacion").HasColumnType("datetime");
            entity.Property(e => e.FechaActualizacion).IsRequired().HasColumnName("FechaActualizacion").HasColumnType("datetime");
        });

        // Configurar la tabla Categoria
        modelBuilder.Entity<CategoriaAD>(entity =>
        {
            entity.ToTable("Categoria", "dbo");
            entity.HasKey(e => e.IdCategoria);
            entity.Property(e => e.IdCategoria).ValueGeneratedOnAdd();
            entity.Property(e => e.NombreCategoria).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Imagen).HasColumnType("VARBINARY(MAX)");
            entity.Property(e => e.ImagenThumbnail).HasColumnType("VARBINARY(MAX)");
            entity.Property(e => e.Estado).IsRequired();
        });

        // Configurar la tabla Cliente
        modelBuilder.Entity<ClienteAD>(entity =>
        {
            entity.ToTable("Cliente");
            entity.HasKey(e => e.IdCliente);
            entity.Property(e => e.IdCliente).HasColumnName("IdCliente").UseIdentityColumn();
            entity.Property(e => e.NombreCliente).IsRequired().HasMaxLength(100).HasColumnName("NombreCliente").HasColumnType("varchar(100)");
            entity.Property(e => e.Cedula).IsRequired().HasMaxLength(20).HasColumnName("Cedula").HasColumnType("varchar(20)");
            entity.Property(e => e.Correo).IsRequired().HasMaxLength(100).HasColumnName("Correo").HasColumnType("varchar(100)");
            entity.Property(e => e.Telefono).HasMaxLength(20).HasColumnName("Telefono").HasColumnType("varchar(20)");
            entity.Property(e => e.Direccion).HasMaxLength(200).HasColumnName("Direccion").HasColumnType("varchar(200)");
            entity.Property(e => e.Contrasenna).IsRequired().HasMaxLength(255).HasColumnName("Contrasenna").HasColumnType("varchar(255)");
            entity.Property(e => e.Estado).IsRequired().HasColumnName("Estado");
            entity.HasIndex(e => e.Cedula).IsUnique().HasDatabaseName("IX_Cliente_Cedula");
            entity.HasIndex(e => e.Correo).IsUnique().HasDatabaseName("IX_Cliente_Correo");
        });

        // Configurar la tabla Rol
        modelBuilder.Entity<RolAD>(entity =>
        {
            entity.ToTable("Rol");
            entity.HasKey(e => e.IdRol);
            entity.Property(e => e.IdRol).HasColumnName("IdRol").UseIdentityColumn();
            entity.Property(e => e.NombreRol).IsRequired().HasMaxLength(50).HasColumnName("NombreRol").HasColumnType("varchar(50)");
            entity.Property(e => e.Estado).IsRequired().HasColumnName("Estado");
            entity.HasIndex(e => e.NombreRol).IsUnique().HasDatabaseName("IX_Rol_NombreRol");
        });

        // Configurar la tabla Usuario
        modelBuilder.Entity<UsuarioAD>(entity =>
        {
            entity.ToTable("Usuario");
            entity.HasKey(e => e.IdUsuario);
            entity.Property(e => e.IdUsuario).HasColumnName("IdUsuario").UseIdentityColumn();
            entity.Property(e => e.NombreUsuario).IsRequired().HasMaxLength(100).HasColumnName("NombreUsuario").HasColumnType("varchar(100)");
            entity.Property(e => e.Email).IsRequired().HasMaxLength(100).HasColumnName("Email").HasColumnType("varchar(100)");
            entity.Property(e => e.Contrasenna).IsRequired().HasMaxLength(255).HasColumnName("Contrasenna").HasColumnType("varchar(255)");
            entity.Property(e => e.IdRol).IsRequired().HasColumnName("IdRol");
            entity.Property(e => e.Estado).IsRequired().HasColumnName("Estado");
            entity.HasIndex(e => e.Email).IsUnique().HasDatabaseName("IX_Usuario_Email");
            entity.HasIndex(e => e.NombreUsuario).IsUnique().HasDatabaseName("IX_Usuario_NombreUsuario");
        });

        // Configurar la tabla Auditoria
        modelBuilder.Entity<AuditoriaAD>(entity =>
        {
            entity.ToTable("Auditoria", "dbo");
            entity.HasKey(e => e.IdAuditoria);
            entity.Property(e => e.IdAuditoria).ValueGeneratedOnAdd();
            entity.Property(e => e.Tabla).HasMaxLength(100).IsRequired(false);
            entity.Property(e => e.IdRegistro).IsRequired();
            entity.Property(e => e.Accion).HasMaxLength(50).IsRequired(false);
            entity.Property(e => e.UsuarioId).IsRequired(false);
            entity.Property(e => e.UsuarioNombre).HasMaxLength(100).IsRequired(false);
            entity.Property(e => e.ValoresAnteriores).IsRequired(false);
            entity.Property(e => e.ValoresNuevos).IsRequired(false);
            entity.Property(e => e.Descripcion).HasMaxLength(500).IsRequired(false);
            entity.Property(e => e.FechaAccion).IsRequired();
        });

        // Configurar la tabla EstadoPedido
        modelBuilder.Entity<EstadoPedidoAD>(entity =>
        {
            entity.ToTable("EstadoPedido", "dbo");
            entity.HasKey(e => e.IdEstadoPedido);
            entity.Property(e => e.IdEstadoPedido).ValueGeneratedOnAdd();
            entity.Property(e => e.NombreEstado).IsRequired().HasMaxLength(50);
            entity.Property(e => e.Descripcion).HasMaxLength(200);
            entity.Property(e => e.Estado).IsRequired();
        });

        // Configurar la tabla Pedido con campos nullable
        modelBuilder.Entity<PedidoAD>(entity =>
        {
            entity.ToTable("Pedido", "dbo");
            entity.HasKey(e => e.IdPedido);
            entity.Property(e => e.IdPedido).ValueGeneratedOnAdd();
            entity.Property(e => e.IdCliente).IsRequired();
            entity.Property(e => e.IdUsuario).IsRequired(false);
            entity.Property(e => e.Fecha).IsRequired().HasColumnType("datetime");
            entity.Property(e => e.Subtotal).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Descuento).IsRequired(false).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Impuesto).IsRequired(false).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Total).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.IdEstadoPedido).IsRequired();
        });

        // Configurar la tabla DetallePedido
        modelBuilder.Entity<DetallePedidoAD>(entity =>
        {
            entity.ToTable("DetallePedido", "dbo");
            entity.HasKey(e => e.IdDetalle);
            entity.Property(e => e.IdDetalle).ValueGeneratedOnAdd();
            entity.Property(e => e.IdPedido).IsRequired();
            entity.Property(e => e.IdProducto).IsRequired();
            entity.Property(e => e.Cantidad).IsRequired();
            entity.Property(e => e.Precio).IsRequired().HasColumnType("decimal(10,2)");
            entity.Property(e => e.Descuento).HasColumnType("decimal(10,2)");
            entity.Property(e => e.Subtotal).IsRequired().HasColumnType("decimal(10,2)");
        });

        base.OnModelCreating(modelBuilder);
    }
}