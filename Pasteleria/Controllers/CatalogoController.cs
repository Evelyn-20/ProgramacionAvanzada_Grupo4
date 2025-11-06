using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Categorias;
using Pasteleria.LogicaDeNegocio.Productos;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.Controllers
{
    public class CatalogoController : Controller
    {
        private IListarCategorias _listarCategorias;
        private IObtenerCategoria _obtenerCategoria;
        private IListarProductos _listarProductos;
        private IObtenerProducto _obtenerProducto;

        public CatalogoController()
        {
            _listarCategorias = new ListarCategorias();
            _obtenerCategoria = new ObtenerCategoria();
            _listarProductos = new ListarProductos();
            _obtenerProducto = new ObtenerProducto();
        }

        // GET: Catalogo - Muestra las categorías
        public IActionResult Index()
        {
            try
            {
                List<Categoria> categorias = _listarCategorias.ObtenerActivas();
                return View(categorias);
            }
            catch (Exception ex)
            {
                // Log del error
                System.Diagnostics.Debug.WriteLine($"Error en Catalogo/Index: {ex.Message}");
                TempData["Error"] = "Error al cargar las categorías. Por favor, intenta nuevamente.";

                // Retornar lista vacía en lugar de error
                return View(new List<Categoria>());
            }
        }

        // GET: Catalogo/Productos/5 - Muestra los productos de una categoría
        public IActionResult Productos(int id)
        {
            // Obtener la categoría
            Categoria categoria = _obtenerCategoria.Obtener(id);

            if (categoria == null)
            {
                TempData["Error"] = "La categoría no existe";
                return RedirectToAction("Index");
            }

            // Obtener los productos de la categoría que estén activos y con stock
            List<Producto> productos = _listarProductos.BuscarPorCategoria(id)
                .Where(p => p.Estado)
                .ToList();

            // Pasar la categoría al ViewBag para mostrarla en la vista
            ViewBag.Categoria = categoria;

            return View(productos);
        }

        // GET: Catalogo/DetalleProducto/5 - Muestra el detalle de un producto
        public IActionResult DetalleProducto(int id)
        {
            Producto producto = _obtenerProducto.Obtener(id);

            if (producto == null)
            {
                TempData["Error"] = "El producto no existe";
                return RedirectToAction("Index");
            }

            // Obtener la categoría del producto
            Categoria categoria = _obtenerCategoria.Obtener(producto.IdCategoria);
            ViewBag.Categoria = categoria;

            return View(producto);
        }
    }
}