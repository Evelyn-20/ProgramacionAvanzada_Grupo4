using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos
{
    public class Contexto : DbContext
    {
        public Contexto() : base("name=Contexto")
        {
            
        }

        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<Cliente> Cliente { get; set; }
        public DbSet<DetallePedido> DetallePedido { get; set; }
        public DbSet<Pedido> Pedido { get; set; }
        public DbSet<Producto> Producto { get; set; }
        public DbSet<Rol> Rol { get; set; }

        public DbSet<Usuario> Usuario { get; set; }
    }
}