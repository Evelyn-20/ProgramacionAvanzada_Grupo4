using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class EliminarPedido : IEliminarPedido
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public EliminarPedido()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Eliminar(int idPedido)
        {
            var strategy = _contexto.Database.CreateExecutionStrategy();

            return strategy.Execute(() =>
            {
                using (var context = new Contexto()) // Nuevo contexto
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var pedido = context.Pedido
                            .FirstOrDefault(p => p.IdPedido == idPedido);

                        if (pedido == null)
                            return 0;

                        // Guardar información para auditoría
                        var infoPedido = new
                        {
                            pedido.IdPedido,
                            pedido.IdCliente,
                            pedido.Total,
                            pedido.Fecha
                        };

                        // Obtener detalles antes de eliminar para restaurar inventario
                        var detalles = context.DetallePedido
                            .Where(d => d.IdPedido == idPedido)
                            .ToList();

                        // Restaurar inventario
                        foreach (var detalle in detalles)
                        {
                            var producto = context.Producto
                                .FirstOrDefault(p => p.IdProducto == detalle.IdProducto);

                            if (producto != null)
                            {
                                producto.Cantidad += detalle.Cantidad;
                                producto.FechaActualizacion = DateTime.Now;
                            }
                        }

                        // Eliminar detalles
                        context.DetallePedido.RemoveRange(detalles);

                        // Eliminar pedido
                        context.Pedido.Remove(pedido);

                        int resultado = context.SaveChanges();

                        if (resultado > 0)
                        {
                            _auditoria.RegistrarEliminacion("Pedido", idPedido, infoPedido);
                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                        return resultado;
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        throw new Exception($"Error al eliminar el pedido: {ex.Message}", ex);
                    }
                }
            });
        }
    }
}