using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Models;
using Pasteleria.Abstracciones.ModeloUI;
using X.PagedList;

namespace Pasteleria.Controllers
{
    public class ProductoController : Controller
    {
        private readonly PasteleriaDbContext _db;
        public ProductoController(PasteleriaDbContext db) => _db = db;

        private async Task<SelectList> GetCategoriasAsync(int? sel = null)
        {
            var cats = await _db.Categorias.AsNoTracking()
                           .OrderBy(c => c.NombreCategoria)
                           .ToListAsync();
            return new SelectList(cats, nameof(Categoria.IdCategoria), nameof(Categoria.NombreCategoria), sel);
        }

        // LISTADO con filtros y paginación (desde la web)
        public async Task<IActionResult> ListadoProductos([FromQuery] ProductoFiltroVM f)
        {
            f.Categorias = await GetCategoriasAsync(f.IdCategoria);

            var q = _db.Productos.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(f.Nombre))
                q = q.Where(p => p.NombreProducto.Contains(f.Nombre));

            if (f.IdCategoria.HasValue)
                q = q.Where(p => p.IdCategoria == f.IdCategoria.Value);

            q = q.OrderBy(p => p.NombreProducto);

            var page = f.Pagina <= 0 ? 1 : f.Pagina;
            var paged = await q.ToPagedListAsync(page, 10);

            // Vista web
            ViewBag.Filtro = f; // para recordar filtros en la vista
            return View("ListadoProductos", paged);
        }

        // GET: formulario web Crear
        public async Task<IActionResult> CrearProducto()
        {
            var vm = new ProductoFormVM { Categorias = await GetCategoriasAsync() };
            return View("CrearProducto", vm);
        }

        // POST: crear desde la web (Imagen obligatoria)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearProducto(ProductoFormVM vm)
        {
            vm.Categorias = await GetCategoriasAsync(vm.IdCategoria);

            if (vm.Archivo == null || vm.Archivo.Length == 0)
                ModelState.AddModelError(nameof(vm.Archivo), "La imagen es obligatoria");

            if (!ModelState.IsValid) return View("CrearProducto", vm);

            byte[] imagen;
            using (var ms = new MemoryStream())
            {
                await vm.Archivo!.CopyToAsync(ms);
                imagen = ms.ToArray();
            }

            var p = new Producto
            {
                NombreProducto = vm.NombreProducto.Trim(),
                IdCategoria = vm.IdCategoria,
                Precio = vm.Precio,
                PorcentajeImpuesto = vm.PorcentajeImpuesto,
                Cantidad = vm.Cantidad,
                Estado = vm.Estado,
                DescripcionProducto = (vm.DescripcionProducto ?? "").Trim(),
                Imagen = imagen
            };

            _db.Add(p);
            await _db.SaveChangesAsync();
            TempData["ok"] = "Producto creado";
            return RedirectToAction(nameof(ListadoProductos));
        }

        // GET: formulario web Editar
        public async Task<IActionResult> EditarProducto(int id)
        {
            var p = await _db.Productos.FindAsync(id);
            if (p == null) return RedirectToAction(nameof(ListadoProductos));

            var vm = new ProductoFormVM
            {
                IdProducto = p.IdProducto,
                NombreProducto = p.NombreProducto,
                IdCategoria = p.IdCategoria,
                Precio = p.Precio,
                PorcentajeImpuesto = p.PorcentajeImpuesto,
                Cantidad = p.Cantidad,
                Estado = p.Estado,
                Imagen = p.Imagen,
                Categorias = await GetCategoriasAsync(p.IdCategoria)
            };
            return View("EditarProducto", vm);
        }

        // POST: guardar edición desde la web (imagen opcional)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarProducto(ProductoFormVM vm)
        {
            vm.Categorias = await GetCategoriasAsync(vm.IdCategoria);
            if (!ModelState.IsValid) return View("EditarProducto", vm);

            var p = await _db.Productos.FindAsync(vm.IdProducto);
            if (p == null) return RedirectToAction(nameof(ListadoProductos));

            p.NombreProducto = vm.NombreProducto.Trim();
            p.IdCategoria = vm.IdCategoria;
            p.Precio = vm.Precio;
            p.PorcentajeImpuesto = vm.PorcentajeImpuesto;
            p.Cantidad = vm.Cantidad;
            p.Estado = vm.Estado;

            // si subieron nueva imagen desde la página web
            if (vm.Archivo is not null && vm.Archivo.Length > 0)
            {
                using var ms = new MemoryStream();
                await vm.Archivo.CopyToAsync(ms);
                p.Imagen = ms.ToArray();
            }

            await _db.SaveChangesAsync();
            TempData["ok"] = "Producto actualizado";
            return RedirectToAction(nameof(ListadoProductos));
        }

        // POST: eliminar desde la web (botón en listado o modal)
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarProducto(int IdProducto)
        {
            var p = await _db.Productos.FindAsync(IdProducto);
            if (p != null)
            {
                _db.Productos.Remove(p);
                await _db.SaveChangesAsync();
                TempData["ok"] = "Producto eliminado";
            }
            return RedirectToAction(nameof(ListadoProductos));
        }
    }
}
