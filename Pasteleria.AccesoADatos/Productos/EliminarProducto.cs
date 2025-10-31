using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pasteleria.AccesoADatos.Productos
{
    public class EliminarProducto : IEliminarProducto
    {
        private Contexto _contexto;

        public EliminarProducto()
        {
            _contexto = new Contexto();
        }

        public int Eliminar(int idProducto)
        {
            ProductoAD productoAEliminar = _contexto.Producto
                .FirstOrDefault(p => p.IdProducto == idProducto);

            if (productoAEliminar != null)
            {
                _contexto.Producto.Remove(productoAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();
                return cantidadDeDatosEliminados;
            }

            return 0;
        }
    }
}