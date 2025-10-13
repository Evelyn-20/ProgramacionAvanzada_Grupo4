using System.ComponentModel.DataAnnotations;

namespace Pasteleria.Models
{
    // ViewModel para filtrar clientes en el listado
    public class ClienteFiltroVM
    {
        public string? Buscar { get; set; }
        public int Pagina { get; set; } = 1;
    }

    // ViewModel para el formulario de crear/editar clientes
    public class ClienteFormVM
    {
        public int? IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        [Display(Name = "Nombre del cliente")]
        public string NombreCliente { get; set; } = "";

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [RegularExpression(@"^\d{1}-?\d{4}-?\d{4}$|^\d{9}$", ErrorMessage = "Formato de cédula inválido. Use: 1-2345-6789 o 123456789")]
        [StringLength(20, ErrorMessage = "La cédula no puede exceder 20 caracteres")]
        [Display(Name = "Cédula")]
        public string Cedula { get; set; } = "";

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        [Display(Name = "Correo electrónico")]
        public string Correo { get; set; } = "";

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{4}-?\d{4}$|^\d{8}$", ErrorMessage = "Formato de teléfono inválido. Use: 8888-8888 o 88888888")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [Display(Name = "Teléfono")]
        public string Telefono { get; set; } = "";

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 200 caracteres")]
        [Display(Name = "Dirección")]
        public string Direccion { get; set; } = "";

        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string? Contrasenna { get; set; }

        [Display(Name = "Estado activo")]
        public bool Estado { get; set; } = true;
    }
}