using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using ProductoUI = Pasteleria.Abstracciones.ModeloUI.Producto;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class BuscarProductosParaPedido : IBuscarProductosParaPedido
    {
        private Contexto _contexto;

        public BuscarProductosParaPedido()
        {
            _contexto = new Contexto();
        }

        public List<ProductoUI> BuscarPorTermino(string termino)
        {
            if (string.IsNullOrWhiteSpace(termino))
                return new List<ProductoUI>();

            var productosAD = _contexto.Producto
                .Where(p => p.Estado &&
                           p.Cantidad > 0 &&
                           (p.NombreProducto.Contains(termino) ||
                            p.DescripcionProducto.Contains(termino)))
                .OrderBy(p => p.NombreProducto)
                .Take(20) // Limitar a 20 resultados para mejor rendimiento
                .ToList();

            return productosAD.Select(p => ConvertirAProductoUI(p)).ToList();
        }

        public List<ProductoUI> BuscarActivos()
        {
            var productosAD = _contexto.Producto
                .Where(p => p.Estado && p.Cantidad > 0)
                .OrderBy(p => p.NombreProducto)
                .ToList();

            return productosAD.Select(p => ConvertirAProductoUI(p)).ToList();
        }

        public ProductoUI ObtenerPorId(int idProducto)
        {
            var productoAD = _contexto.Producto
                .FirstOrDefault(p => p.IdProducto == idProducto && p.Estado);

            if (productoAD == null)
                return null;

            return ConvertirAProductoUI(productoAD);
        }

        private ProductoUI ConvertirAProductoUI(ProductoAD productoAD)
        {
            var categoria = _contexto.Categoria
                .FirstOrDefault(c => c.IdCategoria == productoAD.IdCategoria);

            return new ProductoUI
            {
                IdProducto = productoAD.IdProducto,
                IdCategoria = productoAD.IdCategoria,
                NombreProducto = productoAD.NombreProducto,
                DescripcionProducto = productoAD.DescripcionProducto,
                Cantidad = productoAD.Cantidad,
                Precio = productoAD.Precio,
                PorcentajeImpuesto = productoAD.PorcentajeImpuesto,
                Imagen = productoAD.Imagen,
                Estado = productoAD.Estado,
                FechaCreacion = productoAD.FechaCreacion,
                FechaActualizacion = productoAD.FechaActualizacion
            };
        }
    }
}