using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;
using DetallePedidoUI = Pasteleria.Abstracciones.ModeloUI.DetallePedido;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class ObtenerPedido : IObtenerPedido
    {
        private Contexto _contexto;

        public ObtenerPedido()
        {
            _contexto = new Contexto();
        }

        public PedidoUI Obtener(int idPedido)
        {
            try
            {
                var pedidoData = _contexto.Pedido
                    .AsNoTracking()
                    .Where(p => p.IdPedido == idPedido)
                    .Select(p => new
                    {
                        p.IdPedido,
                        p.IdCliente,
                        p.IdUsuario,
                        p.Fecha,
                        p.Subtotal,
                        p.Descuento,
                        p.Impuesto,
                        p.Total,
                        p.IdEstadoPedido
                    })
                    .FirstOrDefault();

                if (pedidoData == null)
                {
                    return null;
                }

                var cliente = _contexto.Cliente
                    .AsNoTracking()
                    .FirstOrDefault(c => c.IdCliente == pedidoData.IdCliente);

                var estado = _contexto.EstadoPedido
                    .AsNoTracking()
                    .FirstOrDefault(e => e.IdEstadoPedido == pedidoData.IdEstadoPedido);

                // Obtener nombre del usuario si existe
                string nombreUsuario = null;
                if (pedidoData.IdUsuario.HasValue && pedidoData.IdUsuario.Value > 0)
                {
                    var usuario = _contexto.Usuario
                        .AsNoTracking()
                        .FirstOrDefault(u => u.IdUsuario == pedidoData.IdUsuario.Value);

                    nombreUsuario = usuario?.NombreUsuario;
                }

                var cantidadProductos = _contexto.DetallePedido
                    .AsNoTracking()
                    .Where(d => d.IdPedido == pedidoData.IdPedido)
                    .Sum(d => (int?)d.Cantidad) ?? 0;

                return new PedidoUI
                {
                    IdPedido = pedidoData.IdPedido,
                    IdCliente = pedidoData.IdCliente,
                    IdUsuario = pedidoData.IdUsuario ?? 0,
                    Fecha = pedidoData.Fecha,
                    Subtotal = pedidoData.Subtotal,
                    Descuento = pedidoData.Descuento ?? 0m,
                    Impuesto = pedidoData.Impuesto ?? 0m,
                    Total = pedidoData.Total,
                    IdEstadoPedido = pedidoData.IdEstadoPedido,
                    NombreCliente = cliente?.NombreCliente ?? "Cliente no encontrado",
                    NombreUsuario = nombreUsuario,
                    Estado = estado?.NombreEstado ?? "Estado desconocido",
                    CantidadProductos = cantidadProductos
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<DetallePedidoUI> ObtenerDetalles(int idPedido)
        {
            try
            {
                var detalles = _contexto.DetallePedido
                    .AsNoTracking()
                    .Where(d => d.IdPedido == idPedido)
                    .ToList();

                var detallesUI = new List<DetallePedidoUI>();

                foreach (var detalle in detalles)
                {
                    if (detalle == null) continue;

                    var producto = _contexto.Producto
                        .AsNoTracking()
                        .FirstOrDefault(p => p.IdProducto == detalle.IdProducto);

                    detallesUI.Add(new DetallePedidoUI
                    {
                        IdDetalle = detalle.IdDetalle,
                        IdPedido = detalle.IdPedido,
                        IdProducto = detalle.IdProducto,
                        Cantidad = detalle.Cantidad,
                        Precio = detalle.Precio,
                        Descuento = detalle.Descuento,
                        Subtotal = detalle.Subtotal,
                        NombreProducto = producto?.NombreProducto ?? "Producto no disponible"
                    });
                }

                return detallesUI;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}