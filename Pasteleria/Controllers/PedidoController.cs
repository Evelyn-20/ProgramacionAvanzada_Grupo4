
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Controllers.Api;       
using System.Collections.Generic;

namespace Pasteleria.Controllers
{
    public class PedidoController : Controller
    {
        // GET: /Pedido/Verificar
        [HttpGet]
        [AllowAnonymous] // evita redirección a Login cuando solo se muestra el carrito
        public IActionResult Verificar()
        {
            List<CarritoApiController.Linea> lineas;
            try
            {
                lineas = HttpContext.Session.GetJson<List<CarritoApiController.Linea>>("CARRITO") ?? new();
            }
            catch
            {
                // si el JSON en sesión está corrupto, limpia y continúa sin tumbar la vista
                HttpContext.Session.Remove("CARRITO");
                lineas = new();
            }
            return View(lineas);
        }

        // GET: /Pedido/ListadoPedidos
        [HttpGet]
        public IActionResult ListadoPedidos()
        {
            
            return View(); // busca Views/Pedido/ListadoPedidos.cshtml
        }
    }
}
