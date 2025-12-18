using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Pedidos;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IBuscarProductosParaPedido _buscarProductos;
        private readonly ICalcularTotales _calcularTotales;

        public CarritoController(
            IBuscarProductosParaPedido buscarProductos,
            ICalcularTotales calcularTotales)
        {
            _buscarProductos = buscarProductos;
            _calcularTotales = calcularTotales;
        }

        // OBTENER CARRITO DESDE COOKIE (no Session)
        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = Request.Cookies["CarritoCompras"];

            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItem>();
            }

            try
            {
                var carrito = JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);
                return carrito ?? new List<CarritoItem>();
            }
            catch
            {
                return new List<CarritoItem>();
            }
        }

        // GUARDAR CARRITO EN COOKIE (no Session)
        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            var carritoJson = JsonConvert.SerializeObject(carrito);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                IsEssential = true,
                Expires = DateTimeOffset.UtcNow.AddDays(7), // Expira en 7 días
                SameSite = SameSiteMode.Lax
            };

            Response.Cookies.Append("CarritoCompras", carritoJson, cookieOptions);
        }

        // LIMPIAR CARRITO
        private void LimpiarCarrito()
        {
            Response.Cookies.Delete("CarritoCompras");
        }

        // OBTENER ID DEL CLIENTE DESDE CLAIMS
        private int? ObtenerClienteId()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
                return null;

            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            if (tipoUsuario != "Cliente")
                return null;

            var clienteIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(clienteIdClaim, out int clienteId))
                return clienteId;

            return null;
        }

        // GET: Carrito
        [Authorize]
        public IActionResult Carrito()
        {
            var clienteId = ObtenerClienteId();

            if (clienteId == null)
            {
                TempData["Error"] = "Debe iniciar sesión como cliente para acceder al carrito";
                return RedirectToAction("Login", "Account");
            }

            var carrito = ObtenerCarrito();

            var resumen = _calcularTotales.CalcularResumen(carrito);
            ViewBag.Resumen = resumen;

            return View(carrito);
        }

        // POST: AgregarAlCarrito
        [HttpPost]
        [Authorize]
        public IActionResult AgregarAlCarrito(int idProducto, int cantidad = 1)
        {
            try
            {
                var clienteId = ObtenerClienteId();

                if (clienteId == null)
                {
                    return Json(new { success = false, mensaje = "Debe iniciar sesión como cliente para agregar productos al carrito" });
                }

                var producto = _buscarProductos.ObtenerPorId(idProducto);
                if (producto == null)
                {
                    return Json(new { success = false, mensaje = "Producto no encontrado" });
                }

                if (producto.Cantidad < cantidad)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = $"Stock insuficiente. Disponible: {producto.Cantidad}"
                    });
                }

                var carrito = ObtenerCarrito();
                var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == idProducto);

                if (itemExistente != null)
                {
                    var nuevaCantidad = itemExistente.Cantidad + cantidad;

                    if (nuevaCantidad > producto.Cantidad)
                    {
                        return Json(new
                        {
                            success = false,
                            mensaje = $"No puede agregar más. Stock disponible: {producto.Cantidad}"
                        });
                    }

                    itemExistente.Cantidad = nuevaCantidad;

                    // RECALCULAR con descuento
                    var bruto = producto.Precio * itemExistente.Cantidad;
                    decimal descuentoProducto = 0;
                    if (producto.PorcentajeDescuento.HasValue && producto.PorcentajeDescuento.Value > 0)
                    {
                        descuentoProducto = bruto * (producto.PorcentajeDescuento.Value / 100m);
                    }

                    itemExistente.Descuento = Math.Round(descuentoProducto, 2);
                    itemExistente.Subtotal = Math.Round(bruto - descuentoProducto, 2);
                    itemExistente.PorcentajeDescuento = producto.PorcentajeDescuento;
                }
                else
                {
                    // CALCULAR descuento para nuevo item
                    var bruto = producto.Precio * cantidad;
                    decimal descuentoProducto = 0;
                    if (producto.PorcentajeDescuento.HasValue && producto.PorcentajeDescuento.Value > 0)
                    {
                        descuentoProducto = bruto * (producto.PorcentajeDescuento.Value / 100m);
                    }

                    var nuevoItem = new CarritoItem
                    {
                        IdProducto = producto.IdProducto,
                        NombreProducto = producto.NombreProducto,
                        Cantidad = cantidad,
                        Precio = producto.Precio,
                        Descuento = Math.Round(descuentoProducto, 2),
                        Subtotal = Math.Round(bruto - descuentoProducto, 2),
                        PorcentajeImpuesto = producto.PorcentajeImpuesto,
                        PorcentajeDescuento = producto.PorcentajeDescuento
                    };
                    carrito.Add(nuevoItem);
                }

                GuardarCarrito(carrito);

                var cantidadTotal = carrito.Sum(c => c.Cantidad);

                return Json(new
                {
                    success = true,
                    mensaje = "Producto agregado al carrito",
                    cantidadTotal = cantidadTotal
                });
            }
            catch (System.Exception ex)
            {
                return Json(new { success = false, mensaje = $"Error: {ex.Message}" });
            }
        }

        // POST: ActualizarCantidad
        [HttpPost]
        [Authorize]
        public IActionResult ActualizarCantidad(int idProducto, string accion)
        {
            try
            {
                var carrito = ObtenerCarrito();
                var item = carrito.FirstOrDefault(c => c.IdProducto == idProducto);

                if (item == null)
                {
                    TempData["Error"] = "Producto no encontrado en el carrito";
                    return RedirectToAction("Carrito");
                }

                var producto = _buscarProductos.ObtenerPorId(idProducto);
                if (producto == null || !producto.Estado)
                {
                    TempData["Error"] = "Producto no disponible";
                    carrito.Remove(item);
                    GuardarCarrito(carrito);
                    return RedirectToAction("Carrito");
                }

                if (accion == "aumentar")
                {
                    if (item.Cantidad >= producto.Cantidad)
                    {
                        TempData["Error"] = $"Stock máximo alcanzado ({producto.Cantidad} unidades disponibles)";
                        return RedirectToAction("Carrito");
                    }
                    item.Cantidad++;
                }
                else if (accion == "disminuir")
                {
                    if (item.Cantidad > 1)
                    {
                        item.Cantidad--;
                    }
                    else
                    {
                        carrito.Remove(item);
                        GuardarCarrito(carrito);
                        TempData["Success"] = "Producto eliminado del carrito";
                        return RedirectToAction("Carrito");
                    }
                }

                // RECALCULAR con descuento
                var bruto = producto.Precio * item.Cantidad;
                decimal descuentoProducto = 0;
                if (producto.PorcentajeDescuento.HasValue && producto.PorcentajeDescuento.Value > 0)
                {
                    descuentoProducto = bruto * (producto.PorcentajeDescuento.Value / 100m);
                }

                item.Descuento = Math.Round(descuentoProducto, 2);
                item.Subtotal = Math.Round(bruto - descuentoProducto, 2);
                item.PorcentajeDescuento = producto.PorcentajeDescuento;

                GuardarCarrito(carrito);
                TempData["Success"] = "Cantidad actualizada";
                return RedirectToAction("Carrito");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"Error al actualizar cantidad: {ex.Message}";
                return RedirectToAction("Carrito");
            }
        }

        // POST: EliminarDelCarrito
        [HttpPost]
        [Authorize]
        public IActionResult EliminarDelCarrito(int idProducto)
        {
            try
            {
                var carrito = ObtenerCarrito();
                var item = carrito.FirstOrDefault(c => c.IdProducto == idProducto);

                if (item != null)
                {
                    carrito.Remove(item);
                    GuardarCarrito(carrito);
                    TempData["Success"] = "Producto eliminado del carrito";
                }
                else
                {
                    TempData["Error"] = "Producto no encontrado en el carrito";
                }

                return RedirectToAction("Carrito");
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"Error al eliminar producto: {ex.Message}";
                return RedirectToAction("Carrito");
            }
        }

        // POST: VaciarCarrito
        [HttpPost]
        [Authorize]
        public IActionResult VaciarCarrito()
        {
            LimpiarCarrito();
            TempData["Success"] = "Carrito vaciado correctamente";
            return RedirectToAction("Carrito");
        }

        // GET: FinalizacionCompra
        [Authorize]
        public IActionResult FinalizacionCompra()
        {
            var clienteId = ObtenerClienteId();
            if (clienteId == null)
            {
                TempData["Error"] = "Debe iniciar sesión como cliente para proceder con el pago";
                return RedirectToAction("Login", "Account");
            }

            var carrito = ObtenerCarrito();

            if (!carrito.Any())
            {
                TempData["Error"] = "El carrito está vacío";
                return RedirectToAction("Carrito");
            }

            // Validar que haya Stock antes de procesar el pedido
            foreach (var item in carrito)
            {
                var producto = _buscarProductos.ObtenerPorId(item.IdProducto);

                if (producto == null || !producto.Estado)
                {
                    TempData["Error"] = $"El producto '{item.NombreProducto}' ya no está disponible";
                    return RedirectToAction("Carrito");
                }

                if (producto.Cantidad < item.Cantidad)
                {
                    TempData["Error"] = $"Stock insuficiente para '{item.NombreProducto}'. Disponible: {producto.Cantidad}, en carrito: {item.Cantidad}";
                    return RedirectToAction("Carrito");
                }
            }

            // Calcular resumen
            var resumen = _calcularTotales.CalcularResumen(carrito);

            return View(resumen);
        }

        // POST: ProcesarPago
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> ProcesarPago()
        {
            try
            {
                var clienteId = ObtenerClienteId();

                if (clienteId == null)
                {
                    TempData["Error"] = "Debe iniciar sesión como cliente";
                    return RedirectToAction("Login", "Account");
                }

                var carrito = ObtenerCarrito();

                if (!carrito.Any())
                {
                    TempData["Error"] = "El carrito está vacío";
                    return RedirectToAction("Carrito");
                }

                // Validar que haya Stock antes de procesar el pedido
                foreach (var item in carrito)
                {
                    var producto = _buscarProductos.ObtenerPorId(item.IdProducto);

                    if (producto == null)
                    {
                        TempData["Error"] = $"El producto '{item.NombreProducto}' no está disponible";
                        return RedirectToAction("Carrito");
                    }

                    if (!producto.Estado)
                    {
                        TempData["Error"] = $"El producto '{item.NombreProducto}' no está disponible";
                        return RedirectToAction("Carrito");
                    }

                    if (producto.Cantidad < item.Cantidad)
                    {
                        TempData["Error"] = $"Stock insuficiente para '{item.NombreProducto}'. Disponible: {producto.Cantidad}";
                        return RedirectToAction("Carrito");
                    }
                }

                // Calcular totales
                var resumen = _calcularTotales.CalcularResumen(carrito);

                var pedido = new Pedido
                {
                    IdCliente = clienteId.Value,
                    IdUsuario = null,
                    Subtotal = resumen.Subtotal,
                    Descuento = resumen.Descuento,
                    Impuesto = resumen.Impuesto,
                    Total = resumen.Total,
                    IdEstadoPedido = 1 // Estado "Pendiente"
                };

                // Crear detalles del pedido
                var detalles = carrito.Select(item => new DetallePedido
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
                    // Limpiar carrito
                    LimpiarCarrito();

                    TempData["Success"] = $"¡Pedido #{idPedido} creado exitosamente! Total: ₡{resumen.Total:N2}";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["Error"] = "Error al procesar el pedido";
                    return RedirectToAction("FinalizacionCompra");
                }
            }
            catch (System.Exception ex)
            {
                TempData["Error"] = $"Error al procesar el pago: {ex.Message}";

                if (ex.InnerException != null)
                {
                    TempData["Error"] += $" | Detalle: {ex.InnerException.Message}";
                }

                return RedirectToAction("FinalizacionCompra");
            }
        }

        // GET: ObtenerCantidadProductos (API JSON)
        [HttpGet]
        public IActionResult ObtenerCantidadProductos()
        {
            var carrito = ObtenerCarrito();
            var cantidad = carrito.Sum(c => c.Cantidad);
            return Json(new { cantidad });
        }

        // GET: ObtenerTotal (API JSON)
        [HttpGet]
        public IActionResult ObtenerTotal()
        {
            var carrito = ObtenerCarrito();
            var resumen = _calcularTotales.CalcularResumen(carrito);

            return Json(new
            {
                subtotal = resumen.Subtotal,
                descuento = resumen.Descuento,
                impuesto = resumen.Impuesto,
                total = resumen.Total
            });
        }
    }
}