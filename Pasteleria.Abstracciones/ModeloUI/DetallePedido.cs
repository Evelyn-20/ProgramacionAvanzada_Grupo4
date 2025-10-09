using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class DetallePedido
    {
        public int IdDetalle { get; set; }
        [DisplayName("Pedido")]
        public int IdPedido { get; set; }
        [DisplayName("Producto")]
        public int IdProducto { get; set; }
        [DisplayName("Cantidad")]
        public int Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Precio { get; set; }
        [DisplayName("Descuento")]
        public decimal Descuento { get; set; }
        [DisplayName("Subtotal")]
        public decimal Subtotal { get; set; }
    }
} 