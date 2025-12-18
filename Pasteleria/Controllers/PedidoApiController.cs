using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.Controllers.Api
{
    [Authorize]
    [ApiController]
    [Route("api/pedidosapi")]
    public class PedidoApiController : ControllerBase
    {
        private readonly IBuscarProductosParaPedido _buscarProductos;
        private readonly ICalcularTotales _calcularTotales;

        public PedidoApiController(IBuscarProductosParaPedido buscarProductos, ICalcularTotales calcularTotales)
        {
            _buscarProductos = buscarProductos;
            _calcularTotales = calcularTotales;
        }

        // GET /api/pedidosapi/buscar-productos?q=chocolate
        [HttpGet("buscar-productos")]
        public IActionResult BuscarProductos([FromQuery] string q = "")
        {
            try
            {
                if (string.IsNullOrWhiteSpace(q))
                {
                    return Ok(new List<object>());
                }

                var productos = _buscarProductos.BuscarPorTermino(q);

                var resultado = productos.Take(10).Select(p => new
                {
                    id = p.IdProducto,
                    nombre = p.NombreProducto,
                    descripcion = p.DescripcionProducto,
                    precio = p.Precio,
                    impuesto = p.PorcentajeImpuesto,
                    stock = p.Cantidad,
                    descuento = p.PorcentajeDescuento ?? 0
                }).ToList();

                return Ok(resultado);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al buscar productos: {ex.Message}" });
            }
        }

        // POST /api/pedidosapi/calcular-totales
        [HttpPost("calcular-totales")]
        public IActionResult CalcularTotales([FromBody] List<ItemCalculoRequest> items)
        {
            try
            {
                if (items == null || !items.Any())
                {
                    return BadRequest(new { error = "Debe enviar al menos un item" });
                }

                var carritoItems = new List<CarritoItem>();

                foreach (var item in items)
                {
                    var producto = _buscarProductos.ObtenerPorId(item.ProductoId);

                    if (producto == null)
                    {
                        return BadRequest(new { error = $"Producto con ID {item.ProductoId} no existe" });
                    }

                    if (producto.Cantidad < item.Cantidad)
                    {
                        return BadRequest(new { error = $"Stock insuficiente para {producto.NombreProducto}. Disponible: {producto.Cantidad}" });
                    }

                    var bruto = producto.Precio * item.Cantidad;

                    // Calcular descuento del producto
                    decimal descuentoProducto = 0;
                    if (producto.PorcentajeDescuento.HasValue && producto.PorcentajeDescuento.Value > 0)
                    {
                        descuentoProducto = bruto * (producto.PorcentajeDescuento.Value / 100m);
                    }

                    // Sumar descuento del producto + descuento adicional
                    var descuentoTotal = descuentoProducto + item.Descuento;
                    if (descuentoTotal > bruto) descuentoTotal = bruto;

                    var subtotalConDescuento = bruto - descuentoTotal;

                    var carritoItem = new CarritoItem
                    {
                        IdProducto = producto.IdProducto,
                        NombreProducto = producto.NombreProducto,
                        Cantidad = item.Cantidad,
                        Precio = producto.Precio,
                        Descuento = Math.Round(descuentoTotal, 2),
                        Subtotal = Math.Round(subtotalConDescuento, 2),
                        PorcentajeImpuesto = producto.PorcentajeImpuesto,
                        PorcentajeDescuento = producto.PorcentajeDescuento
                    };

                    carritoItems.Add(carritoItem);
                }

                var resumen = _calcularTotales.CalcularResumen(carritoItems);

                return Ok(new
                {
                    subtotal = resumen.Subtotal,
                    descuento = resumen.Descuento,
                    impuesto = resumen.Impuesto,
                    total = resumen.Total,
                    items = carritoItems.Select(c => new
                    {
                        productoId = c.IdProducto,
                        nombre = c.NombreProducto,
                        cantidad = c.Cantidad,
                        precio = c.Precio,
                        descuento = c.Descuento,
                        subtotal = c.Subtotal,
                        subtotalConDescuento = c.SubtotalConDescuento
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al calcular totales: {ex.Message}" });
            }
        }

        [HttpGet("validar-stock/{productoId}")]
        public IActionResult ValidarStock(int productoId, [FromQuery] int cantidad = 1)
        {
            try
            {
                var producto = _buscarProductos.ObtenerPorId(productoId);

                if (producto == null)
                {
                    return NotFound(new { error = "Producto no encontrado" });
                }

                var disponible = producto.Cantidad >= cantidad;

                return Ok(new
                {
                    disponible = disponible,
                    stockActual = producto.Cantidad,
                    cantidadSolicitada = cantidad,
                    mensaje = disponible
                        ? "Stock disponible"
                        : $"Stock insuficiente. Disponible: {producto.Cantidad}"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al validar stock: {ex.Message}" });
            }
        }
    }

    public class ItemCalculoRequest
    {
        public int ProductoId { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; } = 0;
    }
}