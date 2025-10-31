using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Models;
using Pasteleria.Abstracciones.Logica.Producto;

namespace Pasteleria.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IListarProductos _listarProductos;

        public HomeController(ILogger<HomeController> logger, IListarProductos listarProductos)
        {
            _logger = logger;
            _listarProductos = listarProductos;
        }

        public IActionResult Index()
        {
            try
            {
                // Obtener todos los productos activos
                var productos = _listarProductos.Obtener()
                    .Where(p => p.Estado == true)
                    .Take(6)
                    .ToList();

                _logger.LogInformation($"Se cargaron {productos.Count} productos en la página de inicio");

                // Pasar los productos a la vista
                return View(productos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al cargar los productos en la página de inicio");

                // En caso de error, pasar una lista vacía
                return View(new List<Pasteleria.Abstracciones.ModeloUI.Producto>());
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}