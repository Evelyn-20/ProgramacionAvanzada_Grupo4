using System.Collections.Generic;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class ResumenCompra
    {
        public List<CarritoItem> Items { get; set; }
        public decimal Subtotal { get; set; }
        public decimal Descuento { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }

        public ResumenCompra()
        {
            Items = new List<CarritoItem>();
        }
    }
}