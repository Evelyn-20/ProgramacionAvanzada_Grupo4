using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Usuario")]
    public class UsuarioAD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("IdUsuario")]
        public int IdUsuario { get; set; }

        [Required]
        [StringLength(100)]
        [Column("NombreUsuario", TypeName = "varchar(100)")]
        public string NombreUsuario { get; set; }

        [Required]
        [StringLength(100)]
        [Column("Email", TypeName = "varchar(100)")]
        public string Email { get; set; }

        [Required]
        [StringLength(255)]
        [Column("Contrasenna", TypeName = "varchar(255)")]
        public string Contrasenna { get; set; }

        [Required]
        [Column("IdRol")]
        public int IdRol { get; set; }

        [Required]
        [Column("Estado")]
        public bool Estado { get; set; }

        [ForeignKey("IdRol")]
        public virtual RolAD Rol { get; set; }
    }
}