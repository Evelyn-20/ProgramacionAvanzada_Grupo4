using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Pedidos;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Linq;
using System.Security.Claims;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    [Authorize] // Requiere autenticación para todos los métodos
    public class PedidoController : BaseController
    {
        private readonly IListarPedidos _listarPedidos;
        private readonly IObtenerPedido _obtenerPedido;
        private readonly IActualizarPedido _actualizarPedido;
        private readonly IGestionarEstadosPedido _gestionarEstados;
        private readonly IBuscarProductosParaPedido _buscarProductos;
        private readonly ICalcularTotales _calcularTotales;
        private readonly Contexto _contexto;

        public PedidoController(
            IListarPedidos listarPedidos,
            IObtenerPedido obtenerPedido,
            IActualizarPedido actualizarPedido,
            IGestionarEstadosPedido gestionarEstados,
            IBuscarProductosParaPedido buscarProductos,
            ICalcularTotales calcularTotales,
            Contexto contexto)
        {
            _listarPedidos = listarPedidos;
            _obtenerPedido = obtenerPedido;
            _actualizarPedido = actualizarPedido;
            _gestionarEstados = gestionarEstados;
            _buscarProductos = buscarProductos;
            _calcularTotales = calcularTotales;
            _contexto = contexto;
        }

        // Cliente

        // GET: /Pedido/MisPedidos (Solo para clientes)
        [HttpGet]
        public IActionResult MisPedidos()
        {
            try
            {
                // Validar que es cliente
                if (!EsCliente())
                {
                    TempData["Error"] = "Esta sección es solo para clientes";
                    return RedirectToAction("Index", "Home");
                }

                var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
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

        // POST: /Pedido/CancelarPedido (Solo clientes pueden cancelar sus pedidos)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CancelarPedido(int idPedido)
        {
            try
            {
                if (!EsCliente())
                {
                    TempData["Error"] = "No tiene permisos para esta acción";
                    return RedirectToAction("Index", "Home");
                }

                if (idPedido <= 0)
                {
                    TempData["Error"] = "Pedido inválido";
                    return RedirectToAction("MisPedidos");
                }

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

        // GET: /Pedido/ListadoPedidos
        [HttpGet]
        public IActionResult ListadoPedidos(string buscar = "", string estado = "")
        {
            try
            {
                // Validar permisos usando BaseController
                if (!PuedeGestionarPedidos())
                {
                    TempData["Error"] = "No tiene permisos para acceder a esta sección";
                    return RedirectToAction("Index", "Home");
                }

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

        // GET: /Pedido/CrearPedido
        [HttpGet]
        public IActionResult CrearPedido()
        {
            try
            {
                // Validar permisos usando BaseController
                if (!PuedeCrearPedidos())
                {
                    TempData["Error"] = "No tiene permisos para crear pedidos";
                    return RedirectToAction("ListadoPedidos");
                }

                // Obtener lista de clientes activos
                var clientes = _contexto.Cliente
                    .Where(c => c.Estado)
                    .OrderBy(c => c.NombreCliente)
                    .Select(c => new
                    {
                        c.IdCliente,
                        c.NombreCliente,
                        c.Cedula,
                        c.Correo
                    })
                    .ToList();

                ViewBag.Clientes = clientes;

                return View();
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar formulario: {ex.Message}";
                return RedirectToAction("ListadoPedidos");
            }
        }

        // POST: /Pedido/GuardarPedido
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GuardarPedido(int idCliente, string productosJson)
        {
            try
            {
                // Validar permisos
                if (!PuedeCrearPedidos())
                {
                    return Json(new { success = false, mensaje = "No tiene permisos para esta acción" });
                }

                // Validar cliente
                if (idCliente <= 0)
                {
                    return Json(new { success = false, mensaje = "Debe seleccionar un cliente" });
                }

                var clienteExiste = _contexto.Cliente
                    .Any(c => c.IdCliente == idCliente && c.Estado);

                if (!clienteExiste)
                {
                    return Json(new { success = false, mensaje = "Cliente no válido o inactivo" });
                }

                // Deserializar productos
                if (string.IsNullOrWhiteSpace(productosJson))
                {
                    return Json(new { success = false, mensaje = "Debe agregar al menos un producto" });
                }

                var items = Newtonsoft.Json.JsonConvert.DeserializeObject<List<ItemPedidoDto>>(productosJson);

                if (items == null || !items.Any())
                {
                    return Json(new { success = false, mensaje = "Debe agregar al menos un producto" });
                }

                // Convertir items a CarritoItem y validar stock
                var carritoItems = new List<CarritoItem>();

                foreach (var item in items)
                {
                    var producto = _buscarProductos.ObtenerPorId(item.IdProducto);

                    if (producto == null)
                    {
                        return Json(new { success = false, mensaje = $"Producto con ID {item.IdProducto} no existe" });
                    }

                    if (!producto.Estado)
                    {
                        return Json(new { success = false, mensaje = $"{producto.NombreProducto} no está disponible" });
                    }

                    if (producto.Cantidad < item.Cantidad)
                    {
                        return Json(new
                        {
                            success = false,
                            mensaje = $"Stock insuficiente para {producto.NombreProducto}. Disponible: {producto.Cantidad}"
                        });
                    }

                    // Calcular precio bruto
                    var bruto = producto.Precio * item.Cantidad;

                    // 1. Calcular descuento del PRODUCTO (porcentaje configurado)
                    decimal descuentoProducto = 0;
                    if (producto.PorcentajeDescuento.HasValue && producto.PorcentajeDescuento.Value > 0)
                    {
                        descuentoProducto = bruto * (producto.PorcentajeDescuento.Value / 100m);
                    }

                    // 2. Agregar descuento ADICIONAL del pedido (si existe en el DTO)
                    //    Solo si item.Descuento > 0 Y es diferente al descuento del producto
                    decimal descuentoAdicional = 0;
                    if (item.Descuento > 0)
                    {
                        // Si quieres que item.Descuento sea ADICIONAL al del producto:
                        descuentoAdicional = item.Descuento;

                        // O si item.Descuento es el TOTAL (reemplaza el del producto):
                        // descuentoAdicional = 0;
                        // descuentoProducto = item.Descuento;
                    }

                    // Total de descuentos
                    var descuentoTotal = descuentoProducto + descuentoAdicional;

                    // Validar que el descuento no supere el bruto
                    if (descuentoTotal > bruto)
                    {
                        descuentoTotal = bruto;
                    }

                    // Subtotal NETO (después de descuentos)
                    var subtotalNeto = bruto - descuentoTotal;

                    var carritoItem = new CarritoItem
                    {
                        IdProducto = producto.IdProducto,
                        NombreProducto = producto.NombreProducto,
                        Cantidad = item.Cantidad,
                        Precio = producto.Precio,

                        // Descuento TOTAL aplicado
                        Descuento = Math.Round(descuentoTotal, 2),

                        // Subtotal NETO (bruto - descuentos)
                        Subtotal = Math.Round(subtotalNeto, 2),

                        PorcentajeImpuesto = producto.PorcentajeImpuesto,
                        PorcentajeDescuento = producto.PorcentajeDescuento
                    };

                    carritoItems.Add(carritoItem);
                }

                // Calcular totales con CalcularTotales
                var resumen = _calcularTotales.CalcularResumen(carritoItems);

                // Obtener ID del usuario administrador/empleado
                var usuarioIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                int? usuarioId = null;
                if (int.TryParse(usuarioIdClaim, out int uid))
                {
                    var usuarioExiste = _contexto.Usuario.Any(u => u.IdUsuario == uid);
                    if (usuarioExiste)
                    {
                        usuarioId = uid;
                    }
                }

                // Crear pedido
                var pedido = new Pedido
                {
                    IdCliente = idCliente,
                    IdUsuario = usuarioId,
                    Subtotal = resumen.Subtotal,
                    Descuento = resumen.Descuento,
                    Impuesto = resumen.Impuesto,
                    Total = resumen.Total,
                    IdEstadoPedido = 1 // Pendiente
                };

                // Crear detalles del pedido
                var detalles = carritoItems.Select(item => new DetallePedido
                {
                    IdProducto = item.IdProducto,
                    Cantidad = item.Cantidad,
                    Precio = item.Precio,
                    Descuento = item.Descuento,
                    Subtotal = item.Subtotal
                }).ToList();

                // Guardar pedido en la base de datos
                var crearPedido = new CrearPedido();
                var idPedido = await crearPedido.Guardar(pedido, detalles);

                if (idPedido > 0)
                {
                    return Json(new
                    {
                        success = true,
                        mensaje = $"Pedido #{idPedido} creado exitosamente",
                        idPedido = idPedido,
                        total = resumen.Total
                    });
                }
                else
                {
                    return Json(new { success = false, mensaje = "Error al procesar el pedido" });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error: {ex.Message}" });
            }
        }

        // GET: /Pedido/BuscarCliente (para autocompletado)
        [HttpGet]
        public IActionResult BuscarCliente(string termino)
        {
            try
            {
                if (!PuedeCrearPedidos())
                {
                    return Json(new List<object>());
                }

                if (string.IsNullOrWhiteSpace(termino))
                {
                    return Json(new List<object>());
                }

                var clientes = _contexto.Cliente
                    .Where(c => c.Estado &&
                        (c.NombreCliente.Contains(termino) ||
                         c.Cedula.Contains(termino) ||
                         c.Correo.Contains(termino)))
                    .OrderBy(c => c.NombreCliente)
                    .Take(10)
                    .Select(c => new
                    {
                        id = c.IdCliente,
                        nombre = c.NombreCliente,
                        cedula = c.Cedula,
                        correo = c.Correo,
                        telefono = c.Telefono,
                        direccion = c.Direccion
                    })
                    .ToList();

                return Json(clientes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = $"Error al buscar clientes: {ex.Message}" });
            }
        }

        // POST: /Pedido/ActualizarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEstado(int idPedido, string estado)
        {
            try
            {
                // Solo Admin puede cambiar estados
                if (!PuedeCambiarEstadoPedidos())
                {
                    TempData["Error"] = "Solo administradores pueden cambiar estados de pedidos";
                    return RedirectToAction("ListadoPedidos");
                }

                if (idPedido <= 0 || string.IsNullOrWhiteSpace(estado))
                {
                    TempData["Error"] = "Datos inválidos";
                    return RedirectToAction("ListadoPedidos");
                }

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

                // Verificar permisos según rol
                var tipoUsuario = ObtenerTipoUsuario();

                if (tipoUsuario == "Cliente")
                {
                    // Los clientes solo ven sus propios pedidos
                    var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!int.TryParse(clienteIdClaim, out int clienteId) || pedido.IdCliente != clienteId)
                    {
                        return Forbid();
                    }
                }
                else if (!PuedeGestionarPedidos())
                {
                    // Otros roles deben tener permiso de gestión
                    return Forbid();
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

    // DTO para recibir productos en GuardarPedido
    public class ItemPedidoDto
    {
        public int IdProducto { get; set; }
        public int Cantidad { get; set; }
        public decimal Descuento { get; set; }
    }
}