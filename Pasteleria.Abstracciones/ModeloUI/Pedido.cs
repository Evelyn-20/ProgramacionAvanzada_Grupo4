using System;
using System.ComponentModel.DataAnnotations;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Pedido
    {
        public int IdPedido { get; set; }

        [Required(ErrorMessage = "El cliente es obligatorio")]
        public int IdCliente { get; set; }

        public int? IdUsuario { get; set; }

        [Required(ErrorMessage = "La fecha es obligatoria")]
        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El subtotal es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El subtotal debe ser mayor a 0")]
        public decimal Subtotal { get; set; }

        public decimal? Descuento { get; set; }

        public decimal? Impuesto { get; set; }

        [Required(ErrorMessage = "El total es obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor a 0")]
        public decimal Total { get; set; }

        [Required(ErrorMessage = "El estado es obligatorio")]
        public int IdEstadoPedido { get; set; }

        // Propiedades adicionales para la UI
        public string NombreCliente { get; set; }
        public string NombreUsuario { get; set; }
        public string Estado { get; set; }
        public int CantidadProductos { get; set; }

        // Propiedad calculada para mostrar fecha formateada
        public string FechaPedidoFormateada
        {
            get
            {
                return Fecha.ToString("dd/MM/yyyy HH:mm");
            }
        }

        // Constructor
        public Pedido()
        {
            Fecha = DateTime.Now;
            Descuento = 0;
            Impuesto = 0;
            IdEstadoPedido = 1; // Pendiente por defecto
        }
    }
}