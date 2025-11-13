using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Productos;
using Pasteleria.LogicaDeNegocio.Categorias;
using System;
using System.Collections.Generic;

namespace Pasteleria.Controllers
{
    public class HomeController : Controller
    {
        private IListarProductos _listarProductos;
        private IListarCategorias _listarCategorias;

        public HomeController()
        {
            try
            {
                _listarProductos = new ListarProductos();
                _listarCategorias = new ListarCategorias();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CONSTRUCTOR: {ex.Message}");
                throw;
            }
        }

        //public IActionResult Test500()
        //{
        //    throw new Exception("Error de prueba para página 500");
        //}

        public IActionResult Index()
        {
            try
            {
                // Obtener productos activos para mostrar en la página principal
                var productos = _listarProductos.Obtener();
                var productosActivos = productos.FindAll(p => p.Estado);

                // Obtener categorías activas
                var categorias = _listarCategorias.ObtenerActivas();

                // Crear un ViewModel para pasar ambas listas
                var viewModel = new HomeViewModel
                {
                    Productos = productosActivos,
                    Categorias = categorias
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN INDEX: {ex.Message}");
                // Retornar vista con listas vacías en caso de error
                var viewModel = new HomeViewModel
                {
                    Productos = new List<Producto>(),
                    Categorias = new List<Categoria>()
                };
                return View(viewModel);
            }
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View();
        }
    }

    // ViewModel para la página de inicio
    public class HomeViewModel
    {
        public List<Producto> Productos { get; set; }
        public List<Categoria> Categorias { get; set; }
    }
}