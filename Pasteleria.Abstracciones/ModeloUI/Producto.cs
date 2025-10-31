using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Producto
    {
        public int IdProducto { get; set; }
        
        [DisplayName("Categoría")]
        [Required(ErrorMessage = "La categoría es requerida")]
        public int IdCategoria { get; set; }
        
        [DisplayName("Nombre del producto")]
        [Required(ErrorMessage = "El nombre del producto es requerido")]
        [StringLength(200, ErrorMessage = "El nombre no puede exceder 200 caracteres")]
        public string NombreProducto { get; set; }
        
        [DisplayName("Descripción del producto")]
        [Required(ErrorMessage = "La descripción es requerida")]
        public string DescripcionProducto { get; set; }
        
        [DisplayName("Cantidad")]
        [Required(ErrorMessage = "La cantidad es requerida")]
        [Range(0, int.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0")]
        public int Cantidad { get; set; }
        
        [DisplayName("Precio")]
        [Required(ErrorMessage = "El precio es requerido")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El precio debe ser mayor a 0")]
        [DisplayFormat(DataFormatString = "{0:C2}", ApplyFormatInEditMode = false)]
        public decimal Precio { get; set; }
        
        [DisplayName("Porcentaje de impuesto")]
        [Required(ErrorMessage = "El porcentaje de impuesto es requerido")]
        [Range(0, 100, ErrorMessage = "El porcentaje debe estar entre 0 y 100")]
        [DisplayFormat(DataFormatString = "{0:N2}%", ApplyFormatInEditMode = false)]
        public decimal PorcentajeImpuesto { get; set; }

        [DisplayName("Imagen del producto")]
        public byte[] Imagen { get; set; }

        [DisplayName("Estado")]
        public bool Estado { get; set; }

        // Propiedades calculadas útiles para las vistas
        [DisplayName("Precio con Impuesto")]
        public decimal PrecioConImpuesto
        {
            get
            {
                return Precio + (Precio * PorcentajeImpuesto / 100);
            }
        }

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