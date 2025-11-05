using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Categoria")]
    public class CategoriaAD
    {
        [Key]
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(100)]
        public string NombreCategoria { get; set; }

        [Required]
        public byte[] Imagen { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}