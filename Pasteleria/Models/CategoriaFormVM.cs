using System.ComponentModel.DataAnnotations;

namespace Pasteleria.Models
{
    public class CategoriaFormVM
    {
        public int? IdCategoria { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [StringLength(120)]
        public string NombreCategoria { get; set; } = "";

        public bool Estado { get; set; } = true;

        // archivo subido desde la web
        [Display(Name = "Imagen")]
        public IFormFile? Archivo { get; set; }
    }
}

