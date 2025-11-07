using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Productos;
using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.LogicaDeNegocio.Categorias;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    public class ProductoController : BaseController
    {
        private IListarProductos _listarProducto;
        private ICrearProducto _crearProducto;
        private IObtenerProducto _obtenerProductoPorId;
        private IActualizarProducto _actualizarProducto;
        private IEliminarProducto _eliminarProducto;
        private IListarCategorias _listarCategorias;

        public ProductoController()
        {
            try
            {
                _listarProducto = new ListarProductos();
                _crearProducto = new CrearProducto();
                _obtenerProductoPorId = new ObtenerProducto();
                _actualizarProducto = new ActualizarProducto();
                _eliminarProducto = new EliminarProducto();
                _listarCategorias = new ListarCategorias();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CONSTRUCTOR: {ex.Message}");
                throw;
            }
        }

        // GET: Producto/ListadoProductos
        public IActionResult ListadoProductos(string buscar, int? categoria)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                List<Producto> productos = new List<Producto>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    productos = _listarProducto.BuscarPorNombre(buscar);
                    ViewBag.Buscar = buscar;
                }
                else if (categoria.HasValue && categoria.Value > 0)
                {
                    productos = _listarProducto.BuscarPorCategoria(categoria.Value);
                    ViewBag.Categoria = categoria.Value;
                }
                else
                {
                    productos = _listarProducto.Obtener();
                }

                // Pasar categorías activas a la vista para el filtro
                var categorias = _listarCategorias.ObtenerActivas();
                ViewBag.TodasCategorias = categorias;

                return View(productos);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar productos: {ex.Message}";
                return View(new List<Producto>());
            }
        }

        // GET: Producto/CrearProducto
        [HttpGet]
        public IActionResult CrearProducto()
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            // Cargar categorías activas para el dropdown
            var categorias = _listarCategorias.ObtenerActivas();
            ViewBag.Categorias = categorias;
            return View();
        }

        // POST: Producto/CrearProducto
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(Producto producto, IFormFile archivo)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                // Remover validación automática de campos que se manejan manualmente
                ModelState.Remove("Imagen");
                ModelState.Remove("FechaCreacion");
                ModelState.Remove("FechaActualizacion");

                // Validar que se haya subido un archivo
                if (archivo == null || archivo.Length == 0)
                {
                    ModelState.AddModelError("archivo", "La imagen del producto es obligatoria");
                    return View(producto);
                }

                // Validar tipo de archivo
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF, BMP)");
                    return View(producto);
                }

                // Validar tamaño (máximo 5MB)
                if (archivo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                    return View(producto);
                }

                // Validar el resto del modelo
                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {key}: {error.ErrorMessage}");
                        }
                    }
                    return View(producto);
                }

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

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(producto);
            }
        }

        // Reemplaza temporalmente tu método GET EditarProducto con este para hacer debug:

        [HttpGet]
        public IActionResult EditarProducto(int id)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                var producto = _obtenerProductoPorId.Obtener(id);

                if (producto == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(ListadoProductos));
                }

                // Cargar categorías activas para el dropdown
                var categorias = _listarCategorias.ObtenerActivas();
                ViewBag.Categorias = categorias;

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
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                // Remover validación de campos que no vienen del formulario o no son requeridos en edición
                ModelState.Remove("Imagen");
                ModelState.Remove("FechaCreacion");
                ModelState.Remove("FechaActualizacion");
                ModelState.Remove("archivo");

                // VALIDAR EXPLÍCITAMENTE LOS VALORES CRÍTICOS
                if (producto.Precio <= 0)
                {
                    ModelState.AddModelError("Precio", "El precio debe ser mayor a 0");
                }

                if (producto.PorcentajeImpuesto < 0 || producto.PorcentajeImpuesto > 100)
                {
                    ModelState.AddModelError("PorcentajeImpuesto", "El porcentaje de impuesto debe estar entre 0 y 100");
                }

                if (!ModelState.IsValid)
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {key}: {error.ErrorMessage}");
                        }
                    }

                    // Recargar el producto para mostrar la imagen actual
                    var productoParaVista = _obtenerProductoPorId.Obtener(producto.IdProducto);
                    if (productoParaVista != null)
                    {
                        // Mantener los valores del formulario pero recuperar la imagen
                        producto.Imagen = productoParaVista.Imagen;
                        producto.FechaCreacion = productoParaVista.FechaCreacion;
                    }

                    return View(producto);
                }

                // Obtener producto existente
                var productoExistente = _obtenerProductoPorId.Obtener(producto.IdProducto);
                if (productoExistente == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(ListadoProductos));
                }

                // Mantener fecha de creación original
                producto.FechaCreacion = productoExistente.FechaCreacion;

                // Procesar imagen nueva si se proporcionó
                if (archivo != null && archivo.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen");
                        producto.Imagen = productoExistente.Imagen;
                        return View(producto);
                    }

                    if (archivo.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                        producto.Imagen = productoExistente.Imagen;
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
                    producto.Imagen = productoExistente.Imagen;
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
                    producto.Imagen = productoExistente.Imagen;
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el producto: {ex.Message}");

                // Intentar recargar la imagen
                try
                {
                    var productoTemp = _obtenerProductoPorId.Obtener(producto.IdProducto);
                    if (productoTemp != null)
                    {
                        producto.Imagen = productoTemp.Imagen;
                    }
                }
                catch { }

                return View(producto);
            }
        }

        // GET: Producto/DetalleProducto/5
        [HttpGet]
        public IActionResult DetalleProducto(int id)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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

                if (producto == null || producto.Imagen == null || producto.Imagen.Length == 0)
                {
                    return ImagenPlaceholder();
                }

                string contentType = DeterminarTipoImagen(producto.Imagen);
                return File(producto.Imagen, contentType);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error obteniendo imagen: {ex.Message}");
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

            // Imagen base64 de 1x1 pixel transparente
            byte[] emptyImage = Convert.FromBase64String("iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAQAAAC1HAwCAAAAC0lEQVR42mNkYAAAAAYAAjCB0C8AAAAASUVORK5CYII=");
            return File(emptyImage, "image/png");
        }

        private string DeterminarTipoImagen(byte[] imageBytes)
        {
            if (imageBytes == null || imageBytes.Length < 4)
                return "image/png";

            // JPEG
            if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                return "image/jpeg";

            // PNG
            if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50 && imageBytes[2] == 0x4E && imageBytes[3] == 0x47)
                return "image/png";

            // GIF
            if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49 && imageBytes[2] == 0x46)
                return "image/gif";

            // BMP
            if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                return "image/bmp";

            return "image/png";
        }
    }
}