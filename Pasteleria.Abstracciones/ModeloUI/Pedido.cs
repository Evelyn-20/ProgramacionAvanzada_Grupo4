using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Pedido
    {
        public int IdPedido { get; set; }

        [DisplayName("Cliente")]
        public int IdCliente { get; set; }

        [DisplayName("Usuario")]
        public int IdUsuario { get; set; }

        [DisplayName("Fecha")]
        public DateTime Fecha { get; set; }

        [DisplayName("Subtotal")]
        public decimal Subtotal { get; set; }

        [DisplayName("Total")]
        public decimal Total { get; set; }

        [DisplayName("Estado")]
        public int IdEstadoPedido { get; set; }
    }
}