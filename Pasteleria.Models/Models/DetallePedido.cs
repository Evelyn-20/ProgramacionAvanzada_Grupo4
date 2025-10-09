using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Models.Models
{
    [Table("DetallePedido")]
    public class DetallePedido
    {
        [Column("IdDetalle")]
        public int IdDetalle { get; set; }

        [Column("IdPedido")]
        public int IdPedido { get; set; }

        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Precio")]
        public decimal Precio { get; set; }

        [Column("Descuento")]
        public decimal Descuento { get; set; }

        [Column("Subtotal")]
        public decimal Subtotal { get; set; }
    }
}