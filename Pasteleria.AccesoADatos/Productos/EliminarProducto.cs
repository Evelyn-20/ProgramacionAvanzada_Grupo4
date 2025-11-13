using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pasteleria.AccesoADatos.Productos
{
    public class EliminarProducto : IEliminarProducto
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public EliminarProducto()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Eliminar(int idProducto)
        {
            ProductoAD productoAEliminar = _contexto.Producto
                .FirstOrDefault(p => p.IdProducto == idProducto);

            if (productoAEliminar != null)
            {
                // Guardar información antes de eliminar para auditoría
                var infoProducto = new
                {
                    productoAEliminar.IdProducto,
                    productoAEliminar.NombreProducto,
                    productoAEliminar.IdCategoria,
                    productoAEliminar.Precio,
                    productoAEliminar.Cantidad,
                    productoAEliminar.Estado
                };

                _contexto.Producto.Remove(productoAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosEliminados > 0)
                {
                    _auditoria.RegistrarEliminacion("Producto", idProducto, infoProducto);
                }

                return cantidadDeDatosEliminados;
            }

            return 0;
        }
    }
}