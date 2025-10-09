using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Models.Models
{
    [Table("Pedido")]
    public class Pedido
    {
        [Column("IdPedido")]
        public int IdPedido { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; }

        [Column("Subtotal")]
        public decimal Subtotal { get; set; }

        [Column("Total")]
        public decimal Total { get; set; }

        [Column("Estado")]
        public string Estado { get; set; }
    }
}