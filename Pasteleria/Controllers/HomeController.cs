using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Models;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly PasteleriaDbContext _db;

        public HomeController(ILogger<HomeController> logger, PasteleriaDbContext db)
        {
            _logger = logger;
            _db = db;
        }

        public async Task<IActionResult> Index()
        {
            var vm = new HomeVM
            {
                // últimos 6 productos activos
                Productos = await _db.Productos
                                     .AsNoTracking()
                                     .Where(p => p.Estado)
                                     .OrderByDescending(p => p.IdProducto)
                                     .Take(6)
                                     .ToListAsync(),
                // hasta 8 categorías activas con imagen (o sin imagen)
                Categorias = await _db.Categorias
                                      .AsNoTracking()
                                      .Where(c => c.Estado)
                                      .OrderBy(c => c.NombreCategoria)
                                      .Take(8)
                                      .ToListAsync()
            };

            ViewBag.TotalProductos = await _db.Productos.CountAsync();
            ViewBag.StockTotal = await _db.Productos.SumAsync(p => p.Cantidad);
            ViewBag.TotalCategorias = await _db.Categorias.CountAsync();

            return View(vm);
        }

        public IActionResult Privacy() => View();

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
            => View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
