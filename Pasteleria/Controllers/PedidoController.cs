
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
        
        // GET: /Pedido/ListadoPedidos
        [HttpGet]
        public IActionResult ListadoPedidos()
        {
            return View();
        }
    }
}
