using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Categorias;
using Pasteleria.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    [Authorize] // Requiere autenticación
    public class CategoriaController : BaseController
    {
        private IListarCategorias _listarCategoria;
        private ICrearCategoria _crearCategoria;
        private IObtenerCategoria _obtenerCategoriaPorId;
        private IActualizarCategoria _actualizarCategoria;
        private IEliminarCategoria _eliminarCategoria;

        public CategoriaController()
        {
            try
            {
                _listarCategoria = new ListarCategorias();
                _crearCategoria = new CrearCategoria();
                _obtenerCategoriaPorId = new ObtenerCategoria();
                _actualizarCategoria = new ActualizarCategoria();
                _eliminarCategoria = new EliminarCategoria();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CONSTRUCTOR: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public IActionResult ListadoCategorias(string buscar)
        {
            if (!PuedeGestionarCategorias())
            {
                return RedirectSinPermiso();
            }

            try
            {
                List<Categoria> categorias = new List<Categoria>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    categorias = _listarCategoria.BuscarPorNombre(buscar);
                    ViewBag.Buscar = buscar;
                }
                else
                {
                    categorias = _listarCategoria.Obtener();
                }

                return View(categorias);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar categorías: {ex.Message}";
                return View(new List<Categoria>());
            }
        }

        [HttpGet]
        public IActionResult CrearCategoria()
        {
            // Admin y Operaciones pueden gestionar categorías
            if (!PuedeGestionarCategorias())
            {
                return RedirectSinPermiso();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(Categoria categoria, IFormFile archivo)
        {
            // Admin y Operaciones pueden crear categorías
            if (!PuedeGestionarCategorias())
            {
                return RedirectSinPermiso();
            }

            try
            {
                ModelState.Remove("Imagen");
                ModelState.Remove("ImagenThumbnail");

                // Procesar y validar imagen
                var (imagenOptimizada, thumbnail, error) = await ProcesarImagenCategoria(archivo);

                if (!string.IsNullOrEmpty(error))
                {
                    ModelState.AddModelError("archivo", error);
                    return View(categoria);
                }

                if (!ModelState.IsValid)
                {
                    return View(categoria);
                }

                // Asignar imágenes procesadas
                categoria.Imagen = imagenOptimizada;
                categoria.ImagenThumbnail = thumbnail;
                categoria.Estado = true;

                int resultado = await _crearCategoria.Guardar(categoria);

                if (resultado > 0)
                {
                    TempData["Success"] = "Categoría creada exitosamente";
                    return RedirectToAction(nameof(ListadoCategorias));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo crear la categoría en la base de datos");
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(categoria);
            }
        }

        [HttpGet]
        public IActionResult EditarCategoria(int id)
        {
            // Admin y Operaciones pueden editar categorías
            if (!PuedeGestionarCategorias())
            {
                return RedirectSinPermiso();
            }

            try
            {
                var categoria = _obtenerCategoriaPorId.Obtener(id);

                if (categoria == null)
                {
                    TempData["Error"] = "Categoría no encontrada";
                    return RedirectToAction(nameof(ListadoCategorias));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la categoría: " + ex.Message;
                return RedirectToAction(nameof(ListadoCategorias));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCategoria(Categoria categoria, IFormFile archivo)
        {
            // Admin y Operaciones pueden editar categorías
            if (!PuedeGestionarCategorias())
            {
                return RedirectSinPermiso();
            }

            try
            {
                ModelState.Remove("Imagen");
                ModelState.Remove("ImagenThumbnail");
                ModelState.Remove("archivo");

                if (!ModelState.IsValid)
                {
                    var categoriaParaVista = _obtenerCategoriaPorId.Obtener(categoria.IdCategoria);
                    if (categoriaParaVista != null)
                    {
                        categoria.Imagen = categoriaParaVista.Imagen;
                        categoria.ImagenThumbnail = categoriaParaVista.ImagenThumbnail;
                    }

                    return View(categoria);
                }

                var categoriaExistente = _obtenerCategoriaPorId.Obtener(categoria.IdCategoria);
                if (categoriaExistente == null)
                {
                    TempData["Error"] = "Categoría no encontrada";
                    return RedirectToAction(nameof(ListadoCategorias));
                }

                // Procesar nueva imagen si se subió
                if (archivo != null && archivo.Length > 0)
                {
                    var (imagenOptimizada, thumbnail, error) = await ProcesarImagenCategoria(archivo);

                    if (!string.IsNullOrEmpty(error))
                    {
                        ModelState.AddModelError("archivo", error);
                        categoria.Imagen = categoriaExistente.Imagen;
                        categoria.ImagenThumbnail = categoriaExistente.ImagenThumbnail;
                        return View(categoria);
                    }

                    categoria.Imagen = imagenOptimizada;
                    categoria.ImagenThumbnail = thumbnail;
                }
                else
                {
                    // Mantener imágenes existentes
                    categoria.Imagen = categoriaExistente.Imagen;
                    categoria.ImagenThumbnail = categoriaExistente.ImagenThumbnail;
                }

                int resultado = _actualizarCategoria.Actualizar(categoria);

                if (resultado > 0)
                {
                    TempData["Success"] = "Categoría actualizada exitosamente";
                    return RedirectToAction(nameof(ListadoCategorias));
                }
                else
                {
                    ModelState.AddModelError("", "No se pudo actualizar la categoría");
                    categoria.Imagen = categoriaExistente.Imagen;
                    categoria.ImagenThumbnail = categoriaExistente.ImagenThumbnail;
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar la categoría: {ex.Message}");

                try
                {
                    var categoriaTemp = _obtenerCategoriaPorId.Obtener(categoria.IdCategoria);
                    if (categoriaTemp != null)
                    {
                        categoria.Imagen = categoriaTemp.Imagen;
                        categoria.ImagenThumbnail = categoriaTemp.ImagenThumbnail;
                    }
                }
                catch { }

                return View(categoria);
            }
        }

        [HttpGet]
        public IActionResult DetalleCategoria(int id)
        {
            if (!PuedeGestionarCategorias())
            {
                return RedirectSinPermiso();
            }

            try
            {
                var categoria = _obtenerCategoriaPorId.Obtener(id);

                if (categoria == null)
                {
                    TempData["Error"] = "Categoría no encontrada";
                    return RedirectToAction(nameof(ListadoCategorias));
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar la categoría: " + ex.Message;
                return RedirectToAction(nameof(ListadoCategorias));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoria(int IdCategoria)
        {
            // Solo Admin puede eliminar categorías
            if (!EsAdministrador())
            {
                TempData["Error"] = "Solo administradores pueden eliminar categorías";
                return RedirectToAction(nameof(ListadoCategorias));
            }

            try
            {
                int resultado = _eliminarCategoria.Eliminar(IdCategoria);

                if (resultado > 0)
                {
                    TempData["Success"] = "Categoría eliminada exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar la categoría";
                }

                return RedirectToAction(nameof(ListadoCategorias));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar la categoría: " + ex.Message;
                return RedirectToAction(nameof(ListadoCategorias));
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerImagenCategoria(int id)
        {
            try
            {
                var categoria = _obtenerCategoriaPorId.Obtener(id);

                if (categoria == null || categoria.Imagen == null || categoria.Imagen.Length == 0)
                {
                    return ImagenPlaceholder();
                }

                string contentType = DeterminarTipoImagen(categoria.Imagen);
                return File(categoria.Imagen, contentType);
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

        // Obtiene el thumbnail de la categoría
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ObtenerThumbnailCategoria(int id)
        {
            try
            {
                var categoria = _obtenerCategoriaPorId.Obtener(id);

                if (categoria == null)
                {
                    return ImagenPlaceholder();
                }

                // Si tiene thumbnail, devolverlo
                if (categoria.ImagenThumbnail != null && categoria.ImagenThumbnail.Length > 0)
                {
                    return File(categoria.ImagenThumbnail, "image/jpeg");
                }

                // Si no tiene thumbnail pero tiene imagen completa, generar thumbnail al vuelo
                if (categoria.Imagen != null && categoria.Imagen.Length > 0)
                {
                    try
                    {
                        var thumbnail = ImageHelper.GenerarThumbnail(categoria.Imagen);
                        if (thumbnail != null && thumbnail.Length > 0)
                        {
                            return File(thumbnail, "image/jpeg");
                        }
                    }
                    catch
                    {
                        // Si falla, devolver imagen completa
                        string contentType = DeterminarTipoImagen(categoria.Imagen);
                        return File(categoria.Imagen, contentType);
                    }
                }

                return ImagenPlaceholder();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener thumbnail: {ex.Message}");
                return ImagenPlaceholder();
            }
        }

        /// Procesa y valida la imagen subida, generando tanto la versión optimizada como el thumbnail
        private async Task<(byte[] imagenOptimizada, byte[] thumbnail, string error)> ProcesarImagenCategoria(IFormFile archivo)
        {
            try
            {
                // Validar que se subió un archivo
                if (archivo == null || archivo.Length == 0)
                {
                    return (null, null, "Debe seleccionar una imagen");
                }

                // Validar extensión
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                {
                    return (null, null, "Solo se permiten imágenes (JPG, JPEG, PNG, GIF, BMP)");
                }

                // Validar tamaño del archivo (máximo 5MB)
                if (archivo.Length > 5 * 1024 * 1024)
                {
                    return (null, null, "La imagen no puede superar los 5MB");
                }

                // Leer bytes de la imagen
                byte[] imagenOriginal;
                using (var memoryStream = new MemoryStream())
                {
                    await archivo.CopyToAsync(memoryStream);
                    imagenOriginal = memoryStream.ToArray();
                }

                // Validar que sea una imagen válida
                if (!ImageHelper.EsImagenValida(imagenOriginal))
                {
                    return (null, null, "El archivo no es una imagen válida");
                }

                // Validar dimensiones mínimas
                if (!ImageHelper.CumpleDimensionesMinimas(imagenOriginal, out string mensajeError))
                {
                    return (null, null, mensajeError);
                }

                // Optimizar imagen completa (para detalles)
                byte[] imagenOptimizada = ImageHelper.OptimizarImagen(imagenOriginal);

                // Generar thumbnail (para listados, home, catálogo)
                byte[] thumbnail = ImageHelper.GenerarThumbnail(imagenOriginal);

                return (imagenOptimizada, thumbnail, null);
            }
            catch (Exception ex)
            {
                return (null, null, $"Error al procesar la imagen: {ex.Message}");
            }
        }

    }
}