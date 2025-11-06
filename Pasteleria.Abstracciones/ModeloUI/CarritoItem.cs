using System;
using System.ComponentModel;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class CarritoItem
    {
        public int IdProducto { get; set; }

        [DisplayName("Producto")]
        public string NombreProducto { get; set; }

        [DisplayName("Categoría")]
        public string NombreCategoria { get; set; }

        [DisplayName("Cantidad")]
        public int Cantidad { get; set; }

        [DisplayName("Precio")]
        public decimal Precio { get; set; }

        [DisplayName("Descuento")]
        public decimal Descuento { get; set; }

        [DisplayName("Subtotal")]
        public decimal Subtotal { get; set; }

        // Propiedad calculada
        public decimal SubtotalConDescuento => Subtotal - Descuento;
    }
}