using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("EstadoPedido")]
    public class EstadoPedido
    {
        [Column("IdEstadoPedido")]
        public int IdEstadoPedido { get; set; }

        [Column("NombreEstado")]
        public string NombreEstado { get; set; }

        [Column("Descripcion")]
        public string Descripcion { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}