using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using System.Linq;

namespace Pasteleria.AccesoADatos.Productos
{
    public class ObtenerProducto : IObtenerProducto
    {
        private Contexto _contexto;

        public ObtenerProducto()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Producto Obtener(int idProducto)
        {
            ProductoAD productoAD = _contexto.Producto
                .FirstOrDefault(p => p.IdProducto == idProducto);

            if (productoAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(productoAD);
        }

        private Abstracciones.ModeloUI.Producto ConvertirObjetoParaUI(ProductoAD productoAD)
        {
            return new Abstracciones.ModeloUI.Producto
            {
                IdProducto = productoAD.IdProducto,
                IdCategoria = productoAD.IdCategoria,
                NombreProducto = productoAD.NombreProducto,
                DescripcionProducto = productoAD.DescripcionProducto,
                Cantidad = productoAD.Cantidad,
                Precio = productoAD.Precio,
                PorcentajeDescuento = productoAD.PorcentajeDescuento,
                PorcentajeImpuesto = productoAD.PorcentajeImpuesto,
                Imagen = productoAD.Imagen,
                ImagenThumbnail = productoAD.ImagenThumbnail,
                Estado = productoAD.Estado,
                FechaCreacion = productoAD.FechaCreacion,
                FechaActualizacion = productoAD.FechaActualizacion
            };
        }
    }
}