using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Usuario")]
    public class Usuario
    {
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Column("NombreUsuario")]
        public string NombreUsuario { get; set; }

        [Column("Email")]
        public string Email { get; set; }

        [Column("Contrasenna")]
        public string Contrasenna { get; set; }

        [Column("IdRol")]
        public int IdRol { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}