using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    public class ClienteAD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string NombreCliente { get; set; }

        [Required]
        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Cedula { get; set; }

        [Required]
        [StringLength(100)]
        [Column(TypeName = "varchar(100)")]
        public string Correo { get; set; }

        [StringLength(20)]
        [Column(TypeName = "varchar(20)")]
        public string Telefono { get; set; }

        [StringLength(200)]
        [Column(TypeName = "varchar(200)")]
        public string Direccion { get; set; }

        [Required]
        [StringLength(255)]
        [Column(TypeName = "varchar(255)")]
        public string Contrasenna { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}