using Microsoft.AspNetCore.Mvc;
using System;

namespace Pasteleria.Controllers
{
    public class PedidoController : Controller
    {
        public PedidoController()
        {
        }

        // GET: Pedido/ListadoPedidos
        public IActionResult ListadoPedidos()
        {
            return View();
        }

        // POST: Pedido/ActualizarEstado
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ActualizarEstado()
        {
            return RedirectToAction(nameof(ListadoPedidos));
        }
    }
}