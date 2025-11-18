using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace Pasteleria.LogicaDeNegocio.Productos
{
    public class ListarProductos : IListarProductos
    {
        private IListarProductos _listarProductos;

        public ListarProductos()
        {
            _listarProductos = new AccesoADatos.Productos.ListarProductos();
        }

        public List<Producto> Obtener()
        {
            return _listarProductos.Obtener();
        }

        public List<Producto> BuscarPorNombre(string nombre)
        {
            return _listarProductos.BuscarPorNombre(nombre);
        }

        public List<Producto> BuscarPorCategoria(int idCategoria)
        {
            return _listarProductos.BuscarPorCategoria(idCategoria);
        }

        public List<Producto> BuscarPorDescripcion(string descripcion)
        {
            return _listarProductos.BuscarPorDescripcion(descripcion);
        }

        public List<Producto> BuscarPorPrecio(decimal precio)
        {
            return _listarProductos.BuscarPorPrecio(precio);
        }

        public List<Producto> BuscarPorCantidad(int cantidad)
        {
            return _listarProductos.BuscarPorCantidad(cantidad);
        }
    }
}