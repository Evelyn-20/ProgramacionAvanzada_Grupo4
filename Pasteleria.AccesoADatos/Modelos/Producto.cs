using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Producto")]
    public class Producto
    {
        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdCategoria")]
        public int IdCategoria { get; set; }

        [Column("NombreProducto")]
        public string NombreProducto { get; set; }

        [Column("DescripcionProducto")]
        public string DescripcionProducto { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Precio")]
        public decimal Precio { get; set; }

        [Column("PorcentajeImpuesto")]
        public decimal PorcentajeImpuesto { get; set; }

        [Column("Imagen")]
        public byte[] Imagen { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}