using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Cliente
    {
        public int IdCliente { get; set; }

        [Required(ErrorMessage = "El nombre del cliente es obligatorio")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "El nombre debe tener entre 3 y 100 caracteres")]
        [DisplayName("Nombre del cliente")]
        public string NombreCliente { get; set; }

        [Required(ErrorMessage = "La cédula es obligatoria")]
        [RegularExpression(@"^\d{1}-?\d{4}-?\d{4}$|^\d{9}$", ErrorMessage = "Formato de cédula inválido. Use: 1-2345-6789 o 123456789")]
        [StringLength(20, ErrorMessage = "La cédula no puede exceder 20 caracteres")]
        [DisplayName("Cédula")]
        public string Cedula { get; set; }

        [Required(ErrorMessage = "El correo electrónico es obligatorio")]
        [EmailAddress(ErrorMessage = "El formato del correo electrónico no es válido")]
        [StringLength(100, ErrorMessage = "El correo no puede exceder 100 caracteres")]
        [DisplayName("Correo electrónico")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [RegularExpression(@"^\d{4}-?\d{4}$|^\d{8}$", ErrorMessage = "Formato de teléfono inválido. Use: 8888-8888 o 88888888")]
        [StringLength(20, ErrorMessage = "El teléfono no puede exceder 20 caracteres")]
        [DisplayName("Teléfono")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "La dirección es obligatoria")]
        [StringLength(200, MinimumLength = 10, ErrorMessage = "La dirección debe tener entre 10 y 200 caracteres")]
        [DisplayName("Dirección")]
        public string Direccion { get; set; }

        [StringLength(255, MinimumLength = 6, ErrorMessage = "La contraseña debe tener al menos 6 caracteres")]
        [DataType(DataType.Password)]
        [DisplayName("Contraseña")]
        public string Contrasenna { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }
    }
}