using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class ListarPedidos : IListarPedidos
    {
        private Contexto _contexto;

        public ListarPedidos()
        {
            _contexto = new Contexto();
        }

        public List<PedidoUI> Obtener()
        {
            try
            {
                // Seleccionar directamente a un objeto anónimo para forzar conversiones
                var pedidos = _contexto.Pedido
                    .AsNoTracking()
                    .Select(p => new
                    {
                        p.IdPedido,
                        p.IdCliente,
                        IdUsuario = p.IdUsuario ?? (int?)null,
                        p.Fecha,
                        p.Subtotal,
                        Descuento = p.Descuento ?? (decimal?)null,
                        Impuesto = p.Impuesto ?? (decimal?)null,
                        p.Total,
                        p.IdEstadoPedido
                    })
                    .ToList();

                var resultado = new List<PedidoUI>();

                foreach (var p in pedidos)
                {
                    var cliente = _contexto.Cliente
                        .AsNoTracking()
                        .FirstOrDefault(c => c.IdCliente == p.IdCliente);

                    var estado = _contexto.EstadoPedido
                        .AsNoTracking()
                        .FirstOrDefault(e => e.IdEstadoPedido == p.IdEstadoPedido);

                    var cantidadProductos = _contexto.DetallePedido
                        .AsNoTracking()
                        .Where(d => d.IdPedido == p.IdPedido)
                        .Sum(d => (int?)d.Cantidad) ?? 0;

                    resultado.Add(new PedidoUI
                    {
                        IdPedido = p.IdPedido,
                        IdCliente = p.IdCliente,
                        IdUsuario = p.IdUsuario,
                        Fecha = p.Fecha,
                        Subtotal = p.Subtotal,
                        Descuento = p.Descuento,
                        Impuesto = p.Impuesto,
                        Total = p.Total,
                        IdEstadoPedido = p.IdEstadoPedido,
                        NombreCliente = cliente?.NombreCliente ?? "Cliente no encontrado",
                        Estado = estado?.NombreEstado ?? "Estado desconocido",
                        CantidadProductos = cantidadProductos
                    });
                }

                return resultado.OrderByDescending(p => p.Fecha).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PedidoUI> ObtenerPorCliente(int idCliente)
        {
            try
            {
                var pedidos = _contexto.Pedido
                    .AsNoTracking()
                    .Where(p => p.IdCliente == idCliente)
                    .Select(p => new
                    {
                        p.IdPedido,
                        p.IdCliente,
                        IdUsuario = p.IdUsuario ?? (int?)null,
                        p.Fecha,
                        p.Subtotal,
                        Descuento = p.Descuento ?? (decimal?)null,
                        Impuesto = p.Impuesto ?? (decimal?)null,
                        p.Total,
                        p.IdEstadoPedido
                    })
                    .ToList();

                var resultado = new List<PedidoUI>();

                foreach (var p in pedidos)
                {
                    var cliente = _contexto.Cliente
                        .AsNoTracking()
                        .FirstOrDefault(c => c.IdCliente == p.IdCliente);

                    var estado = _contexto.EstadoPedido
                        .AsNoTracking()
                        .FirstOrDefault(e => e.IdEstadoPedido == p.IdEstadoPedido);

                    var cantidadProductos = _contexto.DetallePedido
                        .AsNoTracking()
                        .Where(d => d.IdPedido == p.IdPedido)
                        .Sum(d => (int?)d.Cantidad) ?? 0;

                    resultado.Add(new PedidoUI
                    {
                        IdPedido = p.IdPedido,
                        IdCliente = p.IdCliente,
                        IdUsuario = p.IdUsuario,
                        Fecha = p.Fecha,
                        Subtotal = p.Subtotal,
                        Descuento = p.Descuento,
                        Impuesto = p.Impuesto,
                        Total = p.Total,
                        IdEstadoPedido = p.IdEstadoPedido,
                        NombreCliente = cliente?.NombreCliente ?? "Cliente no encontrado",
                        Estado = estado?.NombreEstado ?? "Estado desconocido",
                        CantidadProductos = cantidadProductos
                    });
                }

                return resultado.OrderByDescending(p => p.Fecha).ToList();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public List<PedidoUI> ObtenerPorEstado(int idEstado)
        {
            // Similar implementation
            return Obtener().Where(p => p.IdEstadoPedido == idEstado).ToList();
        }

        public List<PedidoUI> ObtenerPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            // Similar implementation
            return Obtener().Where(p => p.Fecha >= fechaInicio && p.Fecha <= fechaFin).ToList();
        }
    }
}