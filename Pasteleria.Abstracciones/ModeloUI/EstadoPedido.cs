using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class EstadoPedido
    {
        public int IdEstadoPedido { get; set; }

        [DisplayName("Estado")]
        [Required(ErrorMessage = "El nombre del estado es requerido")]
        [StringLength(50, ErrorMessage = "El nombre no puede exceder 50 caracteres")]
        public string NombreEstado { get; set; }

        [DisplayName("Descripción")]
        [StringLength(200, ErrorMessage = "La descripción no puede exceder 200 caracteres")]
        public string Descripcion { get; set; }

        [DisplayName("Activo")]
        public bool Estado { get; set; }
    }
}