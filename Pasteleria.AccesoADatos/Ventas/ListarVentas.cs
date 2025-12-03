using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Venta;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;

namespace Pasteleria.AccesoADatos.Ventas
{
    public class ListarVentas : IListarVentas
    {
        private readonly Contexto _contexto;

        public ListarVentas()
        {
            _contexto = new Contexto();
        }

        public List<Venta> Obtener()
        {
            try
            {
                var ventasRaw = _contexto.Venta
                    .AsNoTracking()
                    .Select(v => new
                    {
                        v.IdVenta,
                        v.IdPedido,
                        v.IdCliente,
                        v.IdUsuario,
                        v.FechaVenta,
                        v.Subtotal,
                        v.Impuesto,
                        v.Total,
                        v.MetodoPago
                    })
                    .ToList();

                var resultado = new List<Venta>();

                foreach (var v in ventasRaw)
                {
                    var cliente = _contexto.Cliente
                        .AsNoTracking()
                        .FirstOrDefault(c => c.IdCliente == v.IdCliente);

                    var usuario = _contexto.Usuario
                        .AsNoTracking()
                        .FirstOrDefault(u => u.IdUsuario == v.IdUsuario);

                    resultado.Add(new Venta
                    {
                        IdVenta = v.IdVenta,
                        IdPedido = v.IdPedido,
                        IdCliente = v.IdCliente,
                        IdUsuario = v.IdUsuario,
                        FechaVenta = v.FechaVenta,
                        Subtotal = v.Subtotal,
                        Impuesto = v.Impuesto,
                        Total = v.Total,
                        MetodoPago = v.MetodoPago,
                        NombreCliente = cliente?.NombreCliente ?? "Cliente no encontrado",
                        NombreUsuario = usuario?.NombreUsuario ?? "Usuario desconocido"
                    });
                }

                return resultado.OrderByDescending(x => x.FechaVenta).ToList();
            }
            catch
            {
                throw;
            }
        }

        public List<Venta> ObtenerPorFecha(DateTime inicio, DateTime fin)
        {
            return Obtener().Where(x => x.FechaVenta >= inicio && x.FechaVenta <= fin).ToList();
        }

        public List<Venta> ObtenerPorCliente(int idCliente)
        {
            return Obtener().Where(x => x.IdCliente == idCliente).ToList();
        }

        public List<Venta> ObtenerPorUsuario(int idUsuario)
        {
            return Obtener().Where(x => x.IdUsuario == idUsuario).ToList();
        }
    }
}
