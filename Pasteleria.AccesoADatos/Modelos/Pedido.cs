using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Pedido")]
    public class PedidoAD
    {
        [Key]
        public int IdPedido { get; set; }

        [Required]
        public int IdCliente { get; set; }

        public int? IdUsuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Subtotal { get; set; }

        [Required]
        [Column(TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        public int IdEstadoPedido { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Descuento { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? Impuesto { get; set; }

        // Solo para vista
        [NotMapped]
        public string MetodoPago { get; set; }
    }
}