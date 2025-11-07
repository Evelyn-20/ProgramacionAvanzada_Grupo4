using Microsoft.AspNetCore.Mvc;

namespace Pasteleria.Controllers
{
    public class CarritoController : Controller
    {
        // GET: Carrito
        public IActionResult Carrito()
        {
            // Verificar si el cliente está logueado
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para acceder al carrito";
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        // GET: Carrito/ProcederPago
        public IActionResult ProcederPago()
        {
            // Verificar si el cliente está logueado
            var clienteId = HttpContext.Session.GetInt32("ClienteId");
            if (clienteId == null)
            {
                TempData["Error"] = "Debe iniciar sesión para proceder con el pago";
                return RedirectToAction("Login", "Account");
            }

            return View("FinalizacionCompra");
        }

        // POST: Carrito/ProcesarPago
        [HttpPost]
        public IActionResult ProcesarPago()
        {
            // Aquí irá la lógica para procesar el pago
            TempData["Success"] = "¡Pago procesado exitosamente!";
            return RedirectToAction("Index", "Home");
        }

        // POST: Carrito/AgregarProducto
        [HttpPost]
        public IActionResult AgregarProducto(int idProducto, int cantidad = 1)
        {
            // Aquí irá la lógica para agregar producto al carrito
            TempData["Success"] = "Producto agregado al carrito";
            return RedirectToAction("Index");
        }

        // POST: Carrito/ActualizarCantidad
        [HttpPost]
        public IActionResult ActualizarCantidad(int idProducto, int cantidad)
        {
            // Aquí irá la lógica para actualizar la cantidad del producto
            return Json(new { success = true, mensaje = "Cantidad actualizada" });
        }

        // POST: Carrito/EliminarProducto
        [HttpPost]
        public IActionResult EliminarProducto(int idProducto)
        {
            // Aquí irá la lógica para eliminar producto del carrito
            TempData["Success"] = "Producto eliminado del carrito";
            return RedirectToAction("Index");
        }

        // GET: Carrito/ObtenerTotal
        public IActionResult ObtenerTotal()
        {
            // Aquí irá la lógica para obtener el total del carrito
            return Json(new { subtotal = 0, descuento = 0, impuesto = 0, total = 0 });
        }

        // GET: Carrito/ObtenerCantidadProductos
        public IActionResult ObtenerCantidadProductos()
        {
            // Aquí irá la lógica para obtener la cantidad de productos en el carrito
            return Json(new { cantidad = 0 });
        }
    }
}