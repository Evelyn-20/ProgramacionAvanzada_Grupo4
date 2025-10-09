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
        [DisplayName("Nombre del rol")]
        public string NombreRol { get; set; }
        public bool Estado { get; set; }
    }
}