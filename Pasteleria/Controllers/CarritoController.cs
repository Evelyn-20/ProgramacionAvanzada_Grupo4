using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Pedidos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    public class CarritoController : Controller
    {
        private readonly IBuscarProductosParaPedido _buscarProductos;
        private readonly ICalcularTotales _calcularTotales;
        private const string CarritoSessionKey = "CarritoCompras";

        public CarritoController()
        {
            _buscarProductos = new BuscarProductosParaPedido();
            _calcularTotales = new CalcularTotales();
        }

        // OBTENER CARRITO DE LA SESIÓN
        private List<CarritoItem> ObtenerCarrito()
        {
            var carritoJson = HttpContext.Session.GetString(CarritoSessionKey);

            if (string.IsNullOrEmpty(carritoJson))
            {
                return new List<CarritoItem>();
            }

            var carrito = JsonConvert.DeserializeObject<List<CarritoItem>>(carritoJson);

            return carrito;
        }

        // GUARDAR CARRITO EN LA SESIÓN
        private void GuardarCarrito(List<CarritoItem> carrito)
        {
            var carritoJson = JsonConvert.SerializeObject(carrito);
            HttpContext.Session.SetString(CarritoSessionKey, carritoJson);
        }

        // GET: Carrito (Vista principal)
        public IActionResult Carrito()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");

            if (clienteId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para acceder al carrito";
                return RedirectToAction("Login", "Account");
            }

            var carrito = ObtenerCarrito();

            // Calcular resumen en el controlador
            var resumen = _calcularTotales.CalcularResumen(carrito);

            // Pasar ambos: items y resumen
            ViewBag.Resumen = resumen;

            return View(carrito);
        }

        // POST: AgregarAlCarrito
        [HttpPost]
        public IActionResult AgregarAlCarrito(int idProducto, int cantidad = 1)
        {
            try
            {
                // Verificar autenticación
                var clienteId = HttpContext.Session.GetInt32("ClienteId");

                if (clienteId == null)
                {
                    return Json(new { success = false, mensaje = "Debe iniciar sesión para agregar productos al carrito" });
                }

                // Obtener producto
                var producto = _buscarProductos.ObtenerPorId(idProducto);
                if (producto == null)
                {
                    return Json(new { success = false, mensaje = "Producto no encontrado" });
                }

                // Validar stock
                if (producto.Cantidad < cantidad)
                {
                    return Json(new
                    {
                        success = false,
                        mensaje = $"Stock insuficiente. Disponible: {producto.Cantidad}"
                    });
                }

                // Obtener carrito actual
                var carrito = ObtenerCarrito();

                // Verificar si el producto ya está en el carrito
                var itemExistente = carrito.FirstOrDefault(c => c.IdProducto == idProducto);

                if (itemExistente != null)
                {
                    // Actualizar cantidad si ya existe
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
                    itemExistente.Subtotal = itemExistente.Precio * itemExistente.Cantidad;
                }
                else
                {
                    // Agregar nuevo item
                    var nuevoItem = new CarritoItem
                    {
                        IdProducto = producto.IdProducto,
                        NombreProducto = producto.NombreProducto,
                        Cantidad = cantidad,
                        Precio = producto.Precio,
                        Descuento = 0,
                        Subtotal = producto.Precio * cantidad,
                        PorcentajeImpuesto = producto.PorcentajeImpuesto
                    };
                    carrito.Add(nuevoItem);
                }

                // Guardar carrito actualizado
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

                // Validar stock ACTUAL del producto
                var producto = _buscarProductos.ObtenerPorId(idProducto);
                if (producto == null || !producto.Estado)
                {
                    TempData["Error"] = "Producto no disponible";
                    carrito.Remove(item);
                    GuardarCarrito(carrito);
                    return RedirectToAction("Carrito");
                }

                // Actualizar cantidad según acción
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

                // Recalcular subtotal
                item.Subtotal = item.Precio * item.Cantidad;

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
        public IActionResult VaciarCarrito()
        {
            GuardarCarrito(new List<CarritoItem>());
            TempData["Success"] = "Carrito vaciado correctamente";
            return RedirectToAction("Carrito");
        }

        // GET: FinalizacionCompra
        public IActionResult FinalizacionCompra()
        {
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para proceder con el pago";
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
        public async Task<IActionResult> ProcesarPago()
        {
            try
            {
                var clienteId = HttpContext.Session.GetInt32("ClienteId");

                if (clienteId == null)
                {
                    TempData["Error"] = "Debe iniciar sesión";
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
                    IdUsuario = 0, // Se guardará como NULL en la BD
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
                var crearPedido = new Pasteleria.LogicaDeNegocio.Pedidos.CrearPedido();

                var idPedido = await crearPedido.Guardar(pedido, detalles);

                if (idPedido > 0)
                {
                    // Limpiar carrito
                    GuardarCarrito(new List<CarritoItem>());

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