using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("EstadoPedido")]
    public class EstadoPedidoAD
    {
        [Key]
        public int IdEstadoPedido { get; set; }

        [Required]
        [MaxLength(50)]
        public string NombreEstado { get; set; }

        [MaxLength(200)]
        public string Descripcion { get; set; }

        public bool Estado { get; set; }
    }
}