using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Producto")]
    public class ProductoAD
    {
        [Key]
        public int IdProducto { get; set; }

        [Required]
        public int IdCategoria { get; set; }

        [Required]
        [StringLength(200)]
        public string NombreProducto { get; set; }

        [Required]
        public string DescripcionProducto { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Precio { get; set; }

        [Required]
        public decimal PorcentajeImpuesto { get; set; }

        public byte[] Imagen { get; set; }

        [Required]
        public bool Estado { get; set; }
    }
}