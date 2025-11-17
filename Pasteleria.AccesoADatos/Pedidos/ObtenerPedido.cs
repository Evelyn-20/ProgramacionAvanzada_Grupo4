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
                // Leer directamente permitiendo NULL en IdUsuario
                var pedidoData = _contexto.Pedido
                    .AsNoTracking()
                    .Where(p => p.IdPedido == idPedido)
                    .Select(p => new
                    {
                        p.IdPedido,
                        p.IdCliente,
                        p.IdUsuario,  // Esto es int? (nullable), SQL Server puede retornar NULL
                        p.Fecha,
                        p.Subtotal,
                        p.Descuento,  // decimal? (nullable)
                        p.Impuesto,   // decimal? (nullable)
                        p.Total,
                        p.IdEstadoPedido
                    })
                    .FirstOrDefault();

                if (pedidoData == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Pedido {idPedido} no encontrado");
                    return null;
                }

                // Obtener cliente
                var cliente = _contexto.Cliente
                    .AsNoTracking()
                    .FirstOrDefault(c => c.IdCliente == pedidoData.IdCliente);

                // Obtener estado
                var estado = _contexto.EstadoPedido
                    .AsNoTracking()
                    .FirstOrDefault(e => e.IdEstadoPedido == pedidoData.IdEstadoPedido);

                // Calcular cantidad de productos
                var cantidadProductos = _contexto.DetallePedido
                    .AsNoTracking()
                    .Where(d => d.IdPedido == pedidoData.IdPedido)
                    .Sum(d => (int?)d.Cantidad) ?? 0;

                // Crear objeto Pedido UI con valores seguros
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
                var detallesAD = _contexto.DetallePedido
                    .AsNoTracking()
                    .Where(d => d.IdPedido == idPedido)
                    .ToList();

                var detallesUI = new List<DetallePedidoUI>();

                foreach (var detalle in detallesAD)
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
                        Subtotal = detalle.Subtotal
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