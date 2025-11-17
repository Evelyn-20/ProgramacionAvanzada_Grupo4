using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class ActualizarPedido : IActualizarPedido
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public ActualizarPedido()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Actualizar(PedidoUI pedido)
        {
            var strategy = _contexto.Database.CreateExecutionStrategy();

            return strategy.Execute(() =>
            {
                using (var context = new Contexto())
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        var pedidoExistente = context.Pedido
                            .FirstOrDefault(p => p.IdPedido == pedido.IdPedido);

                        if (pedidoExistente == null)
                            return 0;

                        var valoresAnteriores = new
                        {
                            pedidoExistente.Subtotal,
                            pedidoExistente.Descuento,
                            pedidoExistente.Impuesto,
                            pedidoExistente.Total,
                            pedidoExistente.IdEstadoPedido
                        };

                        // Actualizar solo los campos necesarios
                        pedidoExistente.Subtotal = pedido.Subtotal;
                        pedidoExistente.Total = pedido.Total;
                        pedidoExistente.IdEstadoPedido = pedido.IdEstadoPedido;

                        // Manejar valores nulos correctamente
                        pedidoExistente.Descuento = pedido.Descuento ?? 0m;
                        pedidoExistente.Impuesto = pedido.Impuesto ?? 0m;

                        int resultado = context.SaveChanges();

                        if (resultado > 0)
                        {
                            _auditoria.RegistrarActualizacion("Pedido", pedido.IdPedido,
                                valoresAnteriores,
                                new
                                {
                                    pedidoExistente.Subtotal,
                                    pedidoExistente.Descuento,
                                    pedidoExistente.Impuesto,
                                    pedidoExistente.Total,
                                    pedidoExistente.IdEstadoPedido
                                });

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
                        
                        throw new Exception($"Error al actualizar el pedido: {ex.Message}", ex);
                    }
                }
            });
        }

        public int ActualizarEstado(int idPedido, int idEstado)
        {
            var strategy = _contexto.Database.CreateExecutionStrategy();

            return strategy.Execute(() =>
            {
                using (var context = new Contexto())
                using (var transaction = context.Database.BeginTransaction())
                {
                    try
                    {
                        // Cargar el pedido
                        var pedido = context.Pedido
                            .FirstOrDefault(p => p.IdPedido == idPedido);

                        if (pedido == null)
                        {
                            return 0;
                        }

                        int estadoAnterior = pedido.IdEstadoPedido;

                        // Validar que el estado existe
                        var estadoExiste = context.EstadoPedido
                            .Any(e => e.IdEstadoPedido == idEstado && e.Estado);

                        if (!estadoExiste)
                        {
                            throw new Exception($"Estado {idEstado} no es válido");
                        }

                        // Si se cancela, restaurar inventario
                        if (idEstado == 4 && estadoAnterior != 4) // 4 = Cancelado
                        {
                            var detalles = context.DetallePedido
                                .Where(d => d.IdPedido == idPedido)
                                .ToList();

                            foreach (var detalle in detalles)
                            {
                                var producto = context.Producto
                                    .FirstOrDefault(p => p.IdProducto == detalle.IdProducto);

                                if (producto != null)
                                {
                                    int cantidadAnterior = producto.Cantidad;
                                    producto.Cantidad += detalle.Cantidad;
                                    producto.FechaActualizacion = DateTime.Now;
                                }
                            }
                        }

                        // Actualizar estado del pedido
                        pedido.IdEstadoPedido = idEstado;

                        int resultado = context.SaveChanges();

                        if (resultado > 0)
                        {
                            _auditoria.RegistrarActualizacion("Pedido", idPedido,
                                new { IdEstadoPedido = estadoAnterior },
                                new { IdEstadoPedido = idEstado });

                            transaction.Commit();
                        }
                        else
                        {
                            transaction.Rollback();
                        }

                        return resultado;
                    }
                    catch (DbUpdateException dbEx)
                    {
                        transaction.Rollback();
                        
                        throw new Exception($"Error de base de datos al actualizar estado: {dbEx.InnerException?.Message ?? dbEx.Message}", dbEx);
                    }
                    catch (Exception ex)
                    {
                        transaction.Rollback();
                        
                        throw new Exception($"Error al actualizar estado del pedido: {ex.Message}", ex);
                    }
                }
            });
        }
    }
}