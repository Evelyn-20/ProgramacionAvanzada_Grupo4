using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Pasteleria.Models
{
    public class ProductoFiltroVM
    {
        public string? Nombre { get; set; }
        public int? IdCategoria { get; set; }
        public int Pagina { get; set; } = 1;

        public SelectList? Categorias { get; set; }
    }

    public class ProductoFormVM
    {
        public int? IdProducto { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(120)]
        public string NombreProducto { get; set; } = "";

        [Required(ErrorMessage = "La categoría es obligatoria")]
        public int IdCategoria { get; set; }

        [Required, Range(0.01, 999999.99, ErrorMessage = "El precio debe ser mayor a 0")]
        public decimal Precio { get; set; }

        [Display(Name = "Impuesto (%)"), Range(0, 100)]
        public decimal PorcentajeImpuesto { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "El stock debe ser ≥ 0")]
        public int Cantidad { get; set; }

        
        [Display(Name = "Descripción")]
        [Required(ErrorMessage = "La descripción es obligatoria")]
        [StringLength(500)]
        public string DescripcionProducto { get; set; } = "";

        public bool Estado { get; set; } = true;

        public byte[]? Imagen { get; set; }
        public IFormFile? Archivo { get; set; }
        public SelectList? Categorias { get; set; }
    }
}