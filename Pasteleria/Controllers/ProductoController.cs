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
using System.Globalization;

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
                    // Búsqueda inteligente en todos los campos
                    var terminoBusqueda = buscar.Trim();

                    // Obtener todos los productos
                    var todosProductos = _listarProducto.Obtener();

                    // Buscar en múltiples campos simultáneamente
                    productos = todosProductos.Where(p =>
                        // Buscar en nombre
                        p.NombreProducto.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase) ||

                        // Buscar en descripción
                        p.DescripcionProducto.Contains(terminoBusqueda, StringComparison.OrdinalIgnoreCase) ||

                        // Buscar en ID (convertir a string)
                        p.IdProducto.ToString().Contains(terminoBusqueda) ||

                        // Buscar en cantidad (convertir a string)
                        p.Cantidad.ToString().Contains(terminoBusqueda) ||

                        // Buscar en precio (convertir a string y permitir búsqueda con o sin decimales)
                        p.Precio.ToString().Contains(terminoBusqueda) ||
                        p.Precio.ToString("N2").Contains(terminoBusqueda) ||
                        p.Precio.ToString("F0").Contains(terminoBusqueda)
                    ).ToList();

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
                ModelState.Remove("Imagen");
                ModelState.Remove("FechaCreacion");
                ModelState.Remove("FechaActualizacion");

                if (archivo == null || archivo.Length == 0)
                {
                    ModelState.AddModelError("archivo", "La imagen del producto es obligatoria");
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;
                    return View(producto);
                }

                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF, BMP)");
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;
                    return View(producto);
                }

                if (archivo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;
                    return View(producto);
                }

                if (!ModelState.IsValid)
                {
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;
                    return View(producto);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await archivo.CopyToAsync(memoryStream);
                    producto.Imagen = memoryStream.ToArray();
                }

                producto.Estado = true;

                int resultado = await _crearProducto.Guardar(producto);

                if (resultado > 0)
                {
                    return RedirectToAction(nameof(ListadoProductos));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear el producto en la base de datos");
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                var categorias = _listarCategorias.ObtenerActivas();
                ViewBag.Categorias = categorias;
                return View(producto);
            }
        }

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
        public async Task<IActionResult> EditarProducto(Producto producto, IFormFile archivo, string PrecioStr, string PorcentajeImpuestoStr)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                // Remover validación de campos que no vienen del formulario
                ModelState.Remove("Imagen");
                ModelState.Remove("FechaCreacion");
                ModelState.Remove("FechaActualizacion");
                ModelState.Remove("archivo");
                ModelState.Remove("Precio");
                ModelState.Remove("PorcentajeImpuesto");

                // Parsear Precio desde el string
                if (!string.IsNullOrEmpty(PrecioStr))
                {
                    if (decimal.TryParse(PrecioStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal precioParseado))
                    {
                        producto.Precio = precioParseado;
                    }
                    else
                    {
                        ModelState.AddModelError("Precio", "El precio no tiene un formato válido");
                    }
                }

                // Parsear PorcentajeImpuesto desde el string
                if (!string.IsNullOrEmpty(PorcentajeImpuestoStr))
                {
                    if (decimal.TryParse(PorcentajeImpuestoStr, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal impuestoParseado))
                    {
                        producto.PorcentajeImpuesto = impuestoParseado;
                    }
                    else
                    {
                        ModelState.AddModelError("PorcentajeImpuesto", "El porcentaje de impuesto no tiene un formato válido");
                    }
                }

                // Validar valores
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
                    var productoParaVista = _obtenerProductoPorId.Obtener(producto.IdProducto);
                    if (productoParaVista != null)
                    {
                        producto.Imagen = productoParaVista.Imagen;
                        producto.FechaCreacion = productoParaVista.FechaCreacion;
                    }

                    // Recargar las categorías
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;

                    return View(producto);
                }

                var productoExistente = _obtenerProductoPorId.Obtener(producto.IdProducto);
                if (productoExistente == null)
                {
                    TempData["Error"] = "Producto no encontrado";
                    return RedirectToAction(nameof(ListadoProductos));
                }

                producto.FechaCreacion = productoExistente.FechaCreacion;

                if (archivo != null && archivo.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen");
                        producto.Imagen = productoExistente.Imagen;
                        var categorias = _listarCategorias.ObtenerActivas();
                        ViewBag.Categorias = categorias;
                        return View(producto);
                    }

                    if (archivo.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                        producto.Imagen = productoExistente.Imagen;
                        var categorias2 = _listarCategorias.ObtenerActivas();
                        ViewBag.Categorias = categorias2;
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
                    return RedirectToAction(nameof(ListadoProductos));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar el producto");
                    producto.Imagen = productoExistente.Imagen;
                    var categorias = _listarCategorias.ObtenerActivas();
                    ViewBag.Categorias = categorias;
                }

                return View(producto);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar el producto: {ex.Message}");

                try
                {
                    var productoTemp = _obtenerProductoPorId.Obtener(producto.IdProducto);
                    if (productoTemp != null)
                    {
                        producto.Imagen = productoTemp.Imagen;
                    }
                }
                catch { }

                var categoriasError = _listarCategorias.ObtenerActivas();
                ViewBag.Categorias = categoriasError;

                return View(producto);
            }
        }

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarProducto(int IdProducto)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                int resultado = _eliminarProducto.Eliminar(IdProducto);

                if (resultado < 0)
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
            if (imageBytes == null || imageBytes.Length < 4)
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