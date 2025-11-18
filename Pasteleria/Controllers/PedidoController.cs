using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.LogicaDeNegocio.Pedidos;
using System;
using System.Linq;
using System.Security.Claims;

namespace Pasteleria.Controllers
{
    [Authorize] // Requiere autenticación para todos los métodos
    public class PedidoController : Controller
    {
        private readonly IListarPedidos _listarPedidos;
        private readonly IObtenerPedido _obtenerPedido;
        private readonly IActualizarPedido _actualizarPedido;
        private readonly IGestionarEstadosPedido _gestionarEstados;

        public PedidoController(
            IListarPedidos listarPedidos,
            IObtenerPedido obtenerPedido,
            IActualizarPedido actualizarPedido,
            IGestionarEstadosPedido gestionarEstados)
        {
            _listarPedidos = listarPedidos;
            _obtenerPedido = obtenerPedido;
            _actualizarPedido = actualizarPedido;
            _gestionarEstados = gestionarEstados;
        }

        // GET: /Pedido/MisPedidos (Solo para clientes)
        [HttpGet]
        public IActionResult MisPedidos()
        {
            try
            {
                // Obtener ID del cliente desde Claims
                var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

                if (string.IsNullOrEmpty(clienteIdClaim) || tipoUsuario != "Cliente")
                {
                    TempData["Error"] = "Acceso no autorizado";
                    return RedirectToAction("Index", "Home");
                }

                if (!int.TryParse(clienteIdClaim, out int clienteId))
                {
                    TempData["Error"] = "Error al obtener información del cliente";
                    return RedirectToAction("Index", "Home");
                }

                var pedidos = _listarPedidos.ObtenerPorCliente(clienteId);

                return View(pedidos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar pedidos: {ex.Message}";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: /Pedido/ListadoPedidos
        [HttpGet]
        [Authorize(Roles = "Administrador,Empleado")]
        public IActionResult ListadoPedidos(string buscar = "", string estado = "")
        {
            try
            {
                var pedidos = _listarPedidos.Obtener();

                // Filtrar por búsqueda (nombre de cliente)
                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    pedidos = pedidos.Where(p =>
                        p.NombreCliente != null &&
                        p.NombreCliente.Contains(buscar, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    ViewBag.Buscar = buscar;
                }

                // Filtrar por estado
                if (!string.IsNullOrWhiteSpace(estado))
                {
                    pedidos = pedidos.Where(p =>
                        p.Estado != null &&
                        p.Estado.Equals(estado, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    ViewBag.EstadoFiltro = estado;
                }

                return View(pedidos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar pedidos: {ex.Message}";
                return View();
            }
        }

        // POST: /Pedido/ActualizarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador")]
        public IActionResult ActualizarEstado(int idPedido, string estado)
        {
            try
            {
                if (idPedido <= 0 || string.IsNullOrWhiteSpace(estado))
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("ListadoPedidos");
                }

                // Obtener el ID del estado por nombre
                var estados = _gestionarEstados.ObtenerEstados();
                var estadoObj = estados.FirstOrDefault(e =>
                    e.NombreEstado.Equals(estado, StringComparison.OrdinalIgnoreCase));

                if (estadoObj == null)
                {
                    TempData["Error"] = "Estado no válido";
                    return RedirectToAction("ListadoPedidos");
                }

                var resultado = _actualizarPedido.ActualizarEstado(idPedido, estadoObj.IdEstadoPedido);

                if (resultado > 0)
                {
                    TempData["Success"] = $"Estado del pedido #{idPedido} actualizado a '{estado}'";
                }
                else
                {
                    TempData["Error"] = "No se pudo actualizar el estado";
                }

                return RedirectToAction("ListadoPedidos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al actualizar estado: {ex.Message}";
                return RedirectToAction("ListadoPedidos");
            }
        }

        // POST: /Pedido/CancelarPedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarPedido(int idPedido)
        {
            try
            {
                if (idPedido <= 0)
                {
                    TempData["Error"] = "Pedido inválido";
                    return RedirectToAction("MisPedidos");
                }

                // Verificar que el pedido pertenece al cliente
                var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (!int.TryParse(clienteIdClaim, out int clienteId))
                {
                    TempData["Error"] = "Error de autenticación";
                    return RedirectToAction("Login", "Account");
                }

                var pedido = _obtenerPedido.Obtener(idPedido);

                if (pedido == null)
                {
                    TempData["Error"] = "Pedido no encontrado";
                    return RedirectToAction("MisPedidos");
                }

                if (pedido.IdCliente != clienteId)
                {
                    TempData["Error"] = "No tiene permiso para cancelar este pedido";
                    return RedirectToAction("MisPedidos");
                }

                if (pedido.Estado != "Pendiente")
                {
                    TempData["Error"] = "Solo se pueden cancelar pedidos en estado Pendiente";
                    return RedirectToAction("MisPedidos");
                }

                // Obtener ID del estado "Cancelado"
                var estados = _gestionarEstados.ObtenerEstados();
                var estadoCancelado = estados.FirstOrDefault(e =>
                    e.NombreEstado.Equals("Cancelado", StringComparison.OrdinalIgnoreCase));

                if (estadoCancelado == null)
                {
                    TempData["Error"] = "No se encontró el estado 'Cancelado'";
                    return RedirectToAction("MisPedidos");
                }

                var resultado = _actualizarPedido.ActualizarEstado(idPedido, estadoCancelado.IdEstadoPedido);

                if (resultado > 0)
                {
                    TempData["Success"] = $"Pedido #{idPedido} cancelado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo cancelar el pedido";
                }

                return RedirectToAction("MisPedidos");
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cancelar pedido: {ex.Message}";
                return RedirectToAction("MisPedidos");
            }
        }

        // GET: /Pedido/ObtenerDetalles/{id}
        [HttpGet]
        public IActionResult ObtenerDetalles(int id)
        {
            try
            {
                if (id <= 0)
                {
                    return BadRequest(new { error = "ID de pedido inválido" });
                }

                var pedido = _obtenerPedido.Obtener(id);
                if (pedido == null)
                {
                    return NotFound(new { error = "Pedido no encontrado" });
                }

                // Verificar permisos
                var clienteIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

                if (tipoUsuario == "Cliente")
                {
                    if (!int.TryParse(clienteIdClaim, out int clienteId) || pedido.IdCliente != clienteId)
                    {
                        return Forbid();
                    }
                }

                var detalles = _obtenerPedido.ObtenerDetalles(id);

                return Ok(new
                {
                    pedido = new
                    {
                        id = pedido.IdPedido,
                        cliente = pedido.NombreCliente,
                        fecha = pedido.FechaPedidoFormateada,
                        subtotal = pedido.Subtotal,
                        descuento = pedido.Descuento ?? 0,
                        impuesto = pedido.Impuesto ?? 0,
                        total = pedido.Total,
                        estado = pedido.Estado
                    },
                    productos = detalles.Select(d => new
                    {
                        nombre = d.NombreProducto ?? "Producto",
                        cantidad = d.Cantidad,
                        precio = d.Precio,
                        descuento = d.Descuento,
                        subtotal = d.Subtotal
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al obtener detalles: {ex.Message}" });
            }
        }
    }
}