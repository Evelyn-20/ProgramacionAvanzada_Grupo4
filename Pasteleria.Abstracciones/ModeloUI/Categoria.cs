using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Categoria
    {
        public int IdCategoria { get; set; }

        [DisplayName("Nombre de la categoría")]
        [Required(ErrorMessage = "El nombre de la categoría es requerido")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder 100 caracteres")]
        public string NombreCategoria { get; set; }

        [DisplayName("Imagen de la categoría")]
        public byte[] Imagen { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        // Propiedades calculadas
        [DisplayName("Estado")]
        public string EstadoTexto
        {
            get
            {
                return Estado ? "Activo" : "Inactivo";
            }
        }
    }
}