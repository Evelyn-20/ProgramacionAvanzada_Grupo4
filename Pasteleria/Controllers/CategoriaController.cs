using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Models;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Controllers
{
    public class CategoriaController : Controller
    {
        private readonly PasteleriaDbContext _db;
        public CategoriaController(PasteleriaDbContext db) => _db = db;

       
        // LISTADO DE CATEGORÍAS (con filtro y paginación)
        
        [HttpGet]
        public async Task<IActionResult> ListadoCategorias(string? nombre, int pagina = 1, int tam = 10)
        {
            if (pagina < 1) pagina = 1;
            if (tam <= 0) tam = 10;

            var q = _db.Categorias.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(nombre))
                q = q.Where(c => c.NombreCategoria.Contains(nombre));

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(c => c.NombreCategoria)
                .Skip((pagina - 1) * tam)
                .Take(tam)
                .ToListAsync();

            ViewBag.Nombre = nombre;
            ViewBag.Pagina = pagina;
            ViewBag.Tam = tam;
            ViewBag.Total = total;

            return View(items); //View: Views/Categoria/ListadoCategorias.cshtml
        }

  
        // CREAR CATEGORÍA
        
        [HttpGet]
        public IActionResult Crear() => View(new CategoriaFormVM { Estado = true });

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Crear(CategoriaFormVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.NombreCategoria))
                ModelState.AddModelError(nameof(vm.NombreCategoria), "El nombre es obligatorio.");

            bool existe = await _db.Categorias.AnyAsync(c => c.NombreCategoria == vm.NombreCategoria);
            if (existe)
                ModelState.AddModelError(nameof(vm.NombreCategoria), "Ya existe una categoría con ese nombre.");

            if (!ModelState.IsValid)
                return View(vm);

            byte[]? imagen = null;
            if (vm.Archivo is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await vm.Archivo.CopyToAsync(ms);
                imagen = ms.ToArray();
            }

            var categoria = new Categoria
            {
                NombreCategoria = vm.NombreCategoria.Trim(),
                Estado = vm.Estado,
                
            };

            _db.Categorias.Add(categoria);
            await _db.SaveChangesAsync();

            TempData["ok"] = "Categoría creada correctamente.";
            return RedirectToAction(nameof(ListadoCategorias));
        }

        
        // EDITAR CATEGORÍA
        
        [HttpGet]
        public async Task<IActionResult> Editar(int id)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            if (categoria == null)
                return RedirectToAction(nameof(ListadoCategorias));

            var vm = new CategoriaFormVM
            {
                IdCategoria = categoria.IdCategoria,
                NombreCategoria = categoria.NombreCategoria,
                Estado = categoria.Estado,
                
            };

            return View(vm);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Editar(CategoriaFormVM vm)
        {
            if (string.IsNullOrWhiteSpace(vm.NombreCategoria))
                ModelState.AddModelError(nameof(vm.NombreCategoria), "El nombre es obligatorio.");

            bool duplicado = await _db.Categorias
                .AnyAsync(c => c.IdCategoria != vm.IdCategoria && c.NombreCategoria == vm.NombreCategoria);
            if (duplicado)
                ModelState.AddModelError(nameof(vm.NombreCategoria), "Ya existe una categoría con ese nombre.");

            if (!ModelState.IsValid)
                return View(vm);

            var categoria = await _db.Categorias.FindAsync(vm.IdCategoria);
            if (categoria == null)
                return RedirectToAction(nameof(ListadoCategorias));

            categoria.NombreCategoria = vm.NombreCategoria.Trim();
            categoria.Estado = vm.Estado;

            if (vm.Archivo is { Length: > 0 })
            {
                using var ms = new MemoryStream();
                await vm.Archivo.CopyToAsync(ms);
                
            }

            await _db.SaveChangesAsync();

            TempData["ok"] = "Categoría actualizada correctamente.";
            return RedirectToAction(nameof(ListadoCategorias));
        }

        // ELIMINAR CATEGORÍA
        
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Eliminar(int id)
        {
            var categoria = await _db.Categorias.FindAsync(id);
            if (categoria != null)
            {
                bool tieneProductos = await _db.Productos.AnyAsync(p => p.IdCategoria == id);
                if (tieneProductos)
                {
                    TempData["err"] = "No se puede eliminar: hay productos asociados.";
                }
                else
                {
                    _db.Categorias.Remove(categoria);
                    await _db.SaveChangesAsync();
                    TempData["ok"] = "Categoría eliminada correctamente.";
                }
            }

            return RedirectToAction(nameof(ListadoCategorias));
        }
        [HttpGet]
        public async Task<IActionResult> ActivasJson()
        {
            var cats = await _db.Categorias
                .AsNoTracking()
                .Where(c => c.Estado)
                .OrderBy(c => c.NombreCategoria)
                .Select(c => new { c.IdCategoria, c.NombreCategoria })
                .ToListAsync();

            return Json(cats);
        }

    }
}
