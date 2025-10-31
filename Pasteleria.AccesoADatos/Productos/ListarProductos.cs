using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Productos
{
    public class ListarProductos : IListarProductos
    {
        private Contexto _contexto;

        public ListarProductos()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Producto> Obtener()
        {
            List<ProductoAD> productosAD = _contexto.Producto.ToList();
            return productosAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
        }

        public List<Abstracciones.ModeloUI.Producto> BuscarPorNombre(string nombre)
        {
            List<ProductoAD> productosAD = _contexto.Producto
                .Where(p => p.NombreProducto.Contains(nombre))
                .ToList();
            return productosAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
        }

        public List<Abstracciones.ModeloUI.Producto> BuscarPorCategoria(int idCategoria)
        {
            List<ProductoAD> productosAD = _contexto.Producto
                .Where(p => p.IdCategoria == idCategoria)
                .ToList();
            return productosAD.Select(p => ConvertirObjetoParaUI(p)).ToList();
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
                PorcentajeImpuesto = productoAD.PorcentajeImpuesto,
                Imagen = productoAD.Imagen,
                Estado = productoAD.Estado
            };
        }
    }
}