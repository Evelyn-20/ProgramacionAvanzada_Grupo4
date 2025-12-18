using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

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
            try
            {
                // Verificar si el producto existe
                ProductoAD productoAEliminar = _contexto.Producto
                    .FirstOrDefault(p => p.IdProducto == idProducto);

                if (productoAEliminar == null)
                {
                    return 0;
                }

                // Obtener los estados de pedidos que NO permiten eliminar el producto
                // Solo validamos: Pendiente (1) y En Proceso (2)
                // Usamos ToLower() que sí se puede traducir a SQL
                var estadosActivos = _contexto.EstadoPedido
                    .Where(e => e.NombreEstado.ToLower() == "pendiente" ||
                               e.NombreEstado.ToLower() == "en proceso")
                    .Select(e => e.IdEstadoPedido)
                    .ToList();

                if (estadosActivos.Any())
                {
                    // Verificar si el producto está en algún pedido Pendiente o En Proceso
                    // Usamos un JOIN manual entre DetallePedido y Pedido
                    var pedidosConProducto = (from dp in _contexto.DetallePedido
                                              join p in _contexto.Pedido on dp.IdPedido equals p.IdPedido
                                              where dp.IdProducto == idProducto &&
                                                    estadosActivos.Contains(p.IdEstadoPedido)
                                              select p.IdPedido).Distinct().Count();

                    if (pedidosConProducto > 0)
                    {
                        // El producto está en pedidos Pendientes o En Proceso, no se puede eliminar
                        // Retornar un valor negativo que indique cuántos pedidos tienen el producto
                        return -(pedidosConProducto);
                    }
                }

                // Si llegamos aquí, el producto NO está en pedidos Pendientes ni En Proceso
                // PERO puede estar en DetallePedido de pedidos finalizados (Completado, Entregado, Cancelado)
                // No podemos eliminar físicamente porque rompería la FK, entonces INACTIVAMOS

                var tieneDetallesPedido = _contexto.DetallePedido
                    .Any(dp => dp.IdProducto == idProducto);

                if (tieneDetallesPedido)
                {
                    // El producto tiene historial, solo lo inactivamos
                    var infoProductoInactivar = new
                    {
                        productoAEliminar.IdProducto,
                        productoAEliminar.NombreProducto,
                        productoAEliminar.IdCategoria,
                        productoAEliminar.Precio,
                        productoAEliminar.Cantidad,
                        EstadoAnterior = productoAEliminar.Estado,
                        EstadoNuevo = false,
                        Accion = "Inactivación por eliminación (tiene historial de pedidos)"
                    };

                    productoAEliminar.Estado = false;
                    productoAEliminar.FechaActualizacion = DateTime.Now;

                    int resultado = _contexto.SaveChanges();

                    if (resultado > 0)
                    {
                        _auditoria.RegistrarEliminacion("Producto", idProducto, infoProductoInactivar);
                    }

                    // Retornar código especial -500 para indicar que se inactivó en lugar de eliminar
                    return -500;
                }

                // Si NO tiene detalles de pedido, sí podemos eliminar físicamente
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
            catch (System.Exception ex)
            {
                // Registrar el error para debugging
                System.Diagnostics.Debug.WriteLine($"Error al eliminar producto {idProducto}: {ex.Message}");
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine($"Inner Exception: {ex.InnerException.Message}");
                }

                // Retornar -998 para indicar error de base de datos
                return -998;
            }
        }
    }
}