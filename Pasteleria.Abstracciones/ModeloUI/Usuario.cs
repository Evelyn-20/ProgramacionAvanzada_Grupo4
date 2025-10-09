using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Usuario
    {
        public int IdUsuario { get; set; }
        [DisplayName("Nombre de usuario")]
        public string NombreUsuario { get; set; }
        [DisplayName("Email")]
        public string Email { get; set; }
        [DisplayName("Contraseña")]
        public string Contrasenna { get; set; }
        [DisplayName("Rol")]
        public int IdRol { get; set; }
        public bool Estado { get; set; }
    }
} 