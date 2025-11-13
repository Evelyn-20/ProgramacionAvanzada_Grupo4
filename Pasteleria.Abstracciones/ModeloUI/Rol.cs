using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Rol
    {
        public int IdRol { get; set; }

        [Required(ErrorMessage = "El nombre del rol es obligatorio")]
        [StringLength(50, ErrorMessage = "El nombre del rol no puede exceder 50 caracteres")]
        [Display(Name = "Nombre del Rol")]
        public string NombreRol { get; set; }

        [Display(Name = "Estado")]
        public bool Estado { get; set; }

        public string EstadoTexto => Estado ? "Activo" : "Inactivo";
    }
}