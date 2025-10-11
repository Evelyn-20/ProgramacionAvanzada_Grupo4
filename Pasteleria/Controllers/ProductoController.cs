using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Controllers
{
    public class ProductoController : Controller
    {
        // GET: Producto/ListadoProductos
        public IActionResult ListadoProductos()
        {
            // Por ahora retorna una lista vacía, luego conectarás con tu lógica de negocio
            var productos = new List<Producto>();
            return View(productos);
        }

        // GET: Producto/CrearProducto
        public IActionResult CrearProducto()
        {
            return View();
        }

        // POST: Producto/CrearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CrearProducto(Producto producto, IFormFile archivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Aquí procesarías el archivo si existe
                    if (archivo != null && archivo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            archivo.CopyTo(memoryStream);
                            producto.Imagen = memoryStream.ToArray();
                        }
                    }

                    // Aquí implementarás la lógica para guardar el producto
                    // Por ahora solo redirige
                    return RedirectToAction(nameof(ListadoProductos));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                // Log del error
                ModelState.AddModelError("", "Error al crear el producto: " + ex.Message);
                return View(producto);
            }
        }

        // GET: Producto/EditarProducto/5
        public IActionResult EditarProducto(int id)
        {
            try
            {
                // Aquí buscarás el producto por id desde tu lógica de negocio
                // Por ahora creamos un producto de ejemplo con datos
                var producto = new Producto
                {
                    IdProducto = id,
                    NombreProducto = "Producto de Ejemplo",
                    IdCategoria = 1,
                    DescripcionProducto = "Esta es una descripción de ejemplo",
                    Cantidad = 10,
                    Precio = 5000,
                    PorcentajeImpuesto = 13,
                    Estado = true
                };

                return View(producto);
            }
            catch (Exception ex)
            {
                // Si hay error, redirigir al listado
                TempData["Error"] = "Error al cargar el producto: " + ex.Message;
                return RedirectToAction(nameof(ListadoProductos));
            }
        }

        // POST: Producto/EditarProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarProducto(Producto producto, IFormFile archivo)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Aquí procesarías el archivo si existe
                    if (archivo != null && archivo.Length > 0)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            archivo.CopyTo(memoryStream);
                            producto.Imagen = memoryStream.ToArray();
                        }
                    }

                    // Aquí implementarás la lógica para actualizar el producto
                    TempData["Success"] = "Producto actualizado exitosamente";
                    return RedirectToAction(nameof(ListadoProductos));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el producto: " + ex.Message);
                return View(producto);
            }
        }

        // POST: Producto/EliminarProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(int IdProducto)
        {
            try
            {
                // Aquí implementarás la lógica de eliminación
                TempData["Success"] = "Producto eliminado exitosamente";
                return RedirectToAction(nameof(ListadoProductos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el producto: " + ex.Message;
                return RedirectToAction(nameof(ListadoProductos));
            }
        }
    }
}