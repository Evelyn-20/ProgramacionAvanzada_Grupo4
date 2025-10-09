using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Producto
    {
        public int IdProducto { get; set; }
        [DisplayName("Categoría")]
        public int IdCategoria { get; set; }
        [DisplayName("Nombre del producto")]
        public string NombreProducto { get; set; }
        [DisplayName("Descripción del producto")]
        public string DescripcionProducto { get; set; }
        [DisplayName("Cantidad")]
        public int Cantidad { get; set; }
        [DisplayName("Precio")]
        public decimal Precio { get; set; }
        [DisplayName("Porcentaje de impuesto")]
        public decimal PorcentajeImpuesto { get; set; }
        public byte[] Imagen { get; set; }
        public bool Estado { get; set; }
        // public HttpPostedFileBase archivo { get; set; }
    }
}