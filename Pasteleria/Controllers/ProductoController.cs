using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Productos;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    public class ProductoController : Controller
    {
        private IListarProductos _listarProducto;
        private ICrearProducto _crearProducto;
        private IObtenerProducto _obtenerProductoPorId;
        private IActualizarProducto _actualizarProducto;
        private IEliminarProducto _eliminarProducto;

        public ProductoController()
        {
            try
            {
                _listarProducto = new ListarProductos();
                _crearProducto = new CrearProducto();
                _obtenerProductoPorId = new ObtenerProducto();
                _actualizarProducto = new ActualizarProducto();
                _eliminarProducto = new EliminarProducto();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        // GET: Producto/ListadoProductos
        public IActionResult ListadoProductos(string buscar, int? categoria)
        {
            try
            {
                List<Producto> productos = new List<Producto>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    System.Diagnostics.Debug.WriteLine($"Buscando por nombre: {buscar}");
                    productos = _listarProducto.BuscarPorNombre(buscar);
                    ViewBag.Buscar = buscar;
                }
                else if (categoria.HasValue && categoria.Value > 0)
                {
                    System.Diagnostics.Debug.WriteLine($"Buscando por categoría: {categoria.Value}");
                    productos = _listarProducto.BuscarPorCategoria(categoria.Value);
                    ViewBag.Categoria = categoria.Value;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Obteniendo todos los productos");
                    productos = _listarProducto.Obtener();
                }

                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar productos: {ex.Message}. Inner: {ex.InnerException?.Message}";
                return View(new List<Producto>());
            }
        }

        // GET: Producto/CrearProducto
        [HttpGet]
        public IActionResult CrearProducto()
        {
            return View();
        }

        // POST: Producto/CrearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(Producto producto, IFormFile archivo)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Nombre: {producto?.NombreProducto}");
                System.Diagnostics.Debug.WriteLine($"Categoría: {producto?.IdCategoria}");
                System.Diagnostics.Debug.WriteLine($"Precio: {producto?.Precio}");
                System.Diagnostics.Debug.WriteLine($"Archivo es null: {archivo == null}");

                // PASO 1: Remover validación automática de Imagen
                ModelState.Remove("Imagen");

                // PASO 2: Validar que se haya subido un archivo
                if (archivo == null || archivo.Length == 0)
                {
                    ModelState.AddModelError("archivo", "La imagen del producto es obligatoria");
                    System.Diagnostics.Debug.WriteLine("ERROR: No se proporcionó imagen");
                    return View(producto);
                }

                // PASO 3: Validar tipo de archivo
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF, BMP)");
                    return View(producto);
                }

                // PASO 4: Validar tamaño (máximo 5MB)
                if (archivo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                    return View(producto);
                }

                // PASO 5: Validar el resto del modelo
                if (ModelState.IsValid)
                {
                    // Procesar el archivo
                    using (var memoryStream = new MemoryStream())
                    {
                        await archivo.CopyToAsync(memoryStream);
                        producto.Imagen = memoryStream.ToArray();
                    }

                    // Establecer estado como activo
                    producto.Estado = true;

                    int resultado = await _crearProducto.Guardar(producto);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Producto creado exitosamente";
                        return RedirectToAction(nameof(ListadoProductos));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo crear el producto en la base de datos");
                    }
                }
                else
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {key}: {error.ErrorMessage}");
                        }
                    }
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}. Inner: {ex.InnerException?.Message}");
                return View(producto);
            }
        }

        // GET: Producto/EditarProducto/5
        [HttpGet]
        public IActionResult EditarProducto(int id)
        {
            try
            {
                var producto = _obtenerProductoPorId.Obtener(id);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(ListadoProductos));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el producto: " + ex.Message;
                return RedirectToAction(nameof(ListadoProductos));
            }
        }

        // POST: Producto/EditarProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(Producto producto, IFormFile archivo)
        {
            try
            {
                // Remover validación automática de Imagen
                ModelState.Remove("Imagen");

                if (ModelState.IsValid)
                {
                    // Si se proporcionó un nuevo archivo
                    if (archivo != null && archivo.Length > 0)
                    {
                        // Validar tipo
                        var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                        var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                        if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                        {
                            ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen");
                            return View(producto);
                        }

                        // Validar tamaño
                        if (archivo.Length > 5 * 1024 * 1024)
                        {
                            ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                            return View(producto);
                        }

                        using (var memoryStream = new MemoryStream())
                        {
                            await archivo.CopyToAsync(memoryStream);
                            producto.Imagen = memoryStream.ToArray();
                        }
                    }
                    else
                    {
                        // Mantener imagen existente
                        var productoExistente = _obtenerProductoPorId.Obtener(producto.IdProducto);
                        if (productoExistente != null)
                        {
                            producto.Imagen = productoExistente.Imagen;
                        }
                    }

                    int resultado = _actualizarProducto.Actualizar(producto);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Producto actualizado exitosamente";
                        return RedirectToAction(nameof(ListadoProductos));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el producto");
                    }
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el producto: " + ex.Message);
                return View(producto);
            }
        }

        // GET: Producto/DetalleProducto/5
        [HttpGet]
        public IActionResult DetalleProducto(int id)
        {
            try
            {
                var producto = _obtenerProductoPorId.Obtener(id);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(ListadoProductos));
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el producto: " + ex.Message;
                return RedirectToAction(nameof(ListadoProductos));
            }
        }

        // POST: Producto/EliminarProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(int IdProducto)
        {
            try
            {
                int resultado = _eliminarProducto.Eliminar(IdProducto);

                if (resultado > 0)
                {
                    TempData["Success"] = "Producto eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el producto";
                }

                return RedirectToAction(nameof(ListadoProductos));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el producto: " + ex.Message;
                return RedirectToAction(nameof(ListadoProductos));
            }
        }

        // GET: Producto/ObtenerImagenProducto/5
        [HttpGet]
        public IActionResult ObtenerImagenProducto(int id)
        {
            try
            {
                var producto = _obtenerProductoPorId.Obtener(id);

                if (producto == null || producto.Imagen == null || producto.Imagen.Length <= 1)
                {
                    return ImagenPlaceholder();
                }

                string contentType = DeterminarTipoImagen(producto.Imagen);
                return File(producto.Imagen, contentType);
            }
            catch (Exception)
            {
                return ImagenPlaceholder();
            }
        }

        private IActionResult ImagenPlaceholder()
        {
            string placeholderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "placeholder.png");

            if (System.IO.File.Exists(placeholderPath))
            {
                var bytes = System.IO.File.ReadAllBytes(placeholderPath);
                return File(bytes, "image/png");
            }

            byte[] emptyImage = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");
            return File(emptyImage, "image/png");
        }

        private string DeterminarTipoImagen(byte[] imageBytes)
        {
            if (imageBytes.Length < 4)
                return "image/png";

            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                return "image/jpeg";

            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return "image/png";

            if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46)
                return "image/gif";

            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return "image/bmp";

            return "image/png";
        }
    }
}