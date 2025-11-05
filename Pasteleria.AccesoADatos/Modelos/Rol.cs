using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Rol")]
    public class RolAD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("IdRol")]
        public int IdRol { get; set; }

        [Required]
        [StringLength(50)]
        [Column("NombreRol", TypeName = "varchar(50)")]
        public string NombreRol { get; set; }

        [Required]
        [Column("Estado")]
        public bool Estado { get; set; }
    }
}