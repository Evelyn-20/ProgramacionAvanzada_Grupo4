using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;
using DetallePedidoUI = Pasteleria.Abstracciones.ModeloUI.DetallePedido;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class CrearPedido : ICrearPedido
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public CrearPedido()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public async Task<int> Guardar(PedidoUI pedido, List<DetallePedidoUI> detalles)
        {
            var strategy = _contexto.Database.CreateExecutionStrategy();

            return await strategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _contexto.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Verificar que el cliente existe
                        var clienteExiste = await _contexto.Cliente
                            .AnyAsync(c => c.IdCliente == pedido.IdCliente);

                        if (!clienteExiste)
                        {
                            throw new Exception($"Cliente con ID {pedido.IdCliente} no existe");
                        }

                        // Verificar que el usuario existe (si se proporciona)
                        if (pedido.IdUsuario > 0)
                        {
                            var usuarioExiste = await _contexto.Usuario
                                .AnyAsync(u => u.IdUsuario == pedido.IdUsuario);

                            if (!usuarioExiste)
                            {
                                pedido.IdUsuario = 0;
                            }
                        }

                        // Verificar estado del pedido
                        var estadoExiste = await _contexto.EstadoPedido
                            .AnyAsync(e => e.IdEstadoPedido == pedido.IdEstadoPedido);

                        if (!estadoExiste)
                        {
                            pedido.IdEstadoPedido = 1;
                        }

                        // Validar inventario antes de crear
                        foreach (var detalle in detalles)
                        {
                            var producto = await _contexto.Producto
                                .FirstOrDefaultAsync(p => p.IdProducto == detalle.IdProducto);

                            if (producto == null)
                            {
                                throw new Exception($"Producto con ID {detalle.IdProducto} no existe");
                            }

                            if (!producto.Estado)
                            {
                                throw new Exception($"El producto {producto.NombreProducto} no está activo");
                            }

                            if (producto.Cantidad < detalle.Cantidad)
                            {
                                throw new Exception($"Stock insuficiente para {producto.NombreProducto}. Disponible: {producto.Cantidad}, Solicitado: {detalle.Cantidad}");
                            }
                        }

                        // Asignar fecha actual
                        pedido.Fecha = DateTime.Now;

                        // Crear el pedido
                        int? usuarioNullable = null;
                        if (pedido.IdUsuario > 0)
                        {
                            usuarioNullable = pedido.IdUsuario;
                        }

                        var pedidoAD = new PedidoAD
                        {
                            IdCliente = pedido.IdCliente,
                            IdUsuario = usuarioNullable,
                            Fecha = pedido.Fecha,
                            Subtotal = pedido.Subtotal,
                            Descuento = pedido.Descuento,
                            Impuesto = pedido.Impuesto,
                            Total = pedido.Total,
                            IdEstadoPedido = pedido.IdEstadoPedido
                        };

                        _contexto.Pedido.Add(pedidoAD);
                        await _contexto.SaveChangesAsync();

                        int idPedido = pedidoAD.IdPedido;

                        // Crear los detalles del pedido
                        foreach (var detalle in detalles)
                        {
                            var detalleAD = new DetallePedidoAD
                            {
                                IdPedido = idPedido,
                                IdProducto = detalle.IdProducto,
                                Cantidad = detalle.Cantidad,
                                Precio = detalle.Precio,
                                Descuento = detalle.Descuento,
                                Subtotal = detalle.Subtotal
                            };

                            _contexto.DetallePedido.Add(detalleAD);

                            var producto = await _contexto.Producto
                                .FirstOrDefaultAsync(p => p.IdProducto == detalle.IdProducto);

                            // RE-VALIDAR stock (concurrencia)
                            if (producto.Cantidad < detalle.Cantidad)
                            {
                                throw new Exception($"Stock insuficiente para {producto.NombreProducto} (concurrencia detectada)");
                            }

                            producto.Cantidad -= detalle.Cantidad;

                            // Validar que no quede negativo
                            if (producto.Cantidad < 0)
                            {
                                throw new Exception($"Error de concurrencia: stock negativo para {producto.NombreProducto}");
                            }

                            producto.FechaActualizacion = DateTime.Now;

                        }

                        // Guardar todos los cambios
                        await _contexto.SaveChangesAsync();

                        // Registrar auditoría
                        _auditoria.RegistrarCreacion("Pedido", idPedido, new
                        {
                            pedidoAD.IdPedido,
                            pedidoAD.IdCliente,
                            pedidoAD.IdUsuario,
                            pedidoAD.Subtotal,
                            pedidoAD.Descuento,
                            pedidoAD.Impuesto,
                            pedidoAD.Total,
                            pedidoAD.IdEstadoPedido,
                            CantidadProductos = detalles.Count
                        });

                        // Confirmar transacción
                        await transaction.CommitAsync();

                        return idPedido;
                    }
                    catch (DbUpdateException dbEx)
                    {
                        await transaction.RollbackAsync();

                        // Mensaje más detallado
                        string innerMsg = dbEx.InnerException != null ? dbEx.InnerException.Message : dbEx.Message;
                        throw new Exception($"Error al guardar en base de datos: {innerMsg}", dbEx);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        throw new Exception($"Error al crear el pedido: {ex.Message}", ex);
                    }
                }
            });
        }
    }
}