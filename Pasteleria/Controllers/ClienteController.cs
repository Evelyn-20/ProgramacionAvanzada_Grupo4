using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Controllers
{
    public class ClienteController : Controller
    {
        // GET: Cliente/ListadoClientes
        public IActionResult ListadoClientes()
        {
            // Por ahora retorna una lista vacía, luego conectarás con tu lógica de negocio
            var clientes = new List<Cliente>();
            return View(clientes);
        }

        // GET: Cliente/CrearCliente
        public IActionResult CrearCliente()
        {
            return View();
        }

        // GET: Cliente/EditarCliente/5
        public IActionResult EditarCliente(int id)
        {
            // Aquí buscarás el cliente por id
            var cliente = new Cliente { IdCliente = id };
            return View(cliente);
        }

        // POST: Cliente/EliminarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCliente(int id)
        {
            // Aquí implementarás la lógica de eliminación
            // Por ahora solo redirige de vuelta al listado
            return RedirectToAction(nameof(ListadoClientes));
        }
    }
}