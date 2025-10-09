using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Rol")]
    public class Rol
    {
        [Column("IdRol")]
        public int IdRol { get; set; }

        [Column("NombreRol")]
        public string NombreRol { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
} 