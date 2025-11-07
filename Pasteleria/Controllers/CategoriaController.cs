using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Categorias;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
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

        // GET: Categoria/ListadoCategorias
        public IActionResult ListadoCategorias(string buscar)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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

        // GET: Categoria/CrearCategoria
        [HttpGet]
        public IActionResult CrearCategoria()
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Categoria/CrearCategoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCategoria(Categoria categoria, IFormFile archivo)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                // Remover validación automática de la imagen
                ModelState.Remove("Imagen");

                // Validar que se haya subido un archivo
                if (archivo == null || archivo.Length == 0)
                {
                    ModelState.AddModelError("archivo", "La imagen de la categoría es obligatoria");
                    return View(categoria);
                }

                // Validar tipo de archivo
                var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                {
                    ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen (JPG, JPEG, PNG, GIF, BMP)");
                    return View(categoria);
                }

                // Validar tamaño (máximo 5MB)
                if (archivo.Length > 5 * 1024 * 1024)
                {
                    ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                    return View(categoria);
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
                    return View(categoria);
                }

                // Procesar el archivo
                using (var memoryStream = new MemoryStream())
                {
                    await archivo.CopyToAsync(memoryStream);
                    categoria.Imagen = memoryStream.ToArray();
                }

                // Establecer estado como activo
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

        // GET: Categoria/EditarCategoria/5
        [HttpGet]
        public IActionResult EditarCategoria(int id)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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

        // POST: Categoria/EditarCategoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCategoria(Categoria categoria, IFormFile archivo)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                // Remover validación de la imagen
                ModelState.Remove("Imagen");
                ModelState.Remove("archivo");

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

                    // Recargar la categoría para mostrar la imagen actual
                    var categoriaParaVista = _obtenerCategoriaPorId.Obtener(categoria.IdCategoria);
                    if (categoriaParaVista != null)
                    {
                        categoria.Imagen = categoriaParaVista.Imagen;
                    }

                    return View(categoria);
                }

                // Obtener categoría existente
                var categoriaExistente = _obtenerCategoriaPorId.Obtener(categoria.IdCategoria);
                if (categoriaExistente == null)
                {
                    TempData["Error"] = "Categoría no encontrada";
                    return RedirectToAction(nameof(ListadoCategorias));
                }

                // Procesar imagen nueva si se proporcionó
                if (archivo != null && archivo.Length > 0)
                {
                    var extensionesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };
                    var extension = Path.GetExtension(archivo.FileName).ToLowerInvariant();

                    if (string.IsNullOrEmpty(extension) || !extensionesPermitidas.Contains(extension))
                    {
                        ModelState.AddModelError("archivo", "Solo se permiten archivos de imagen");
                        categoria.Imagen = categoriaExistente.Imagen;
                        return View(categoria);
                    }

                    if (archivo.Length > 5 * 1024 * 1024)
                    {
                        ModelState.AddModelError("archivo", "La imagen no puede superar los 5MB");
                        categoria.Imagen = categoriaExistente.Imagen;
                        return View(categoria);
                    }

                    using (var memoryStream = new MemoryStream())
                    {
                        await archivo.CopyToAsync(memoryStream);
                        categoria.Imagen = memoryStream.ToArray();
                    }
                }
                else
                {
                    categoria.Imagen = categoriaExistente.Imagen;
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
                }

                return View(categoria);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error al actualizar la categoría: {ex.Message}");

                // Intentar recargar la imagen
                try
                {
                    var categoriaTemp = _obtenerCategoriaPorId.Obtener(categoria.IdCategoria);
                    if (categoriaTemp != null)
                    {
                        categoria.Imagen = categoriaTemp.Imagen;
                    }
                }
                catch { }

                return View(categoria);
            }
        }

        // GET: Categoria/DetalleCategoria/5
        [HttpGet]
        public IActionResult DetalleCategoria(int id)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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

        // POST: Categoria/EliminarCategoria
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCategoria(int IdCategoria)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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

        // GET: Categoria/ObtenerImagenCategoria/5
        [HttpGet]
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