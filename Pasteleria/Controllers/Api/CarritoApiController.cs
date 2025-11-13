using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.Controllers.Api
{
    [ApiController]
    [Route("api/carrito")]
    [AllowAnonymous] 
    public class CarritoApiController : ControllerBase
    {
        private const string KEY = "CARRITO";

        
        public class Linea
        {
            public int ProductoId { get; set; }
            public string Nombre { get; set; } = string.Empty;
            public decimal Precio { get; set; }
            public decimal Impuesto { get; set; } // %
            public int Stock { get; set; }
            public decimal Cantidad { get; set; }
            public decimal? DescuentoPorc { get; set; }
            public decimal? DescuentoFijo { get; set; }
        }

        [HttpGet]
        public IActionResult Obtener()
        {
            var carrito = HttpContext.Session.GetJson<List<Linea>>(KEY) ?? new List<Linea>();
            return Ok(carrito);
        }

        // Sincroniza el carrito completo desde el cliente
        [HttpPost("sincronizar")]
        public IActionResult Sincronizar([FromBody] List<Linea> lineas)
        {
            HttpContext.Session.SetJson(KEY, lineas ?? new List<Linea>());
            return Ok(new { ok = true, count = (lineas?.Count ?? 0) });
        }

        // Agregar (o sumar cantidad si ya existe)
        [HttpPost("agregar")]
        public IActionResult Agregar([FromBody] Linea l)
        {
            var carrito = HttpContext.Session.GetJson<List<Linea>>(KEY) ?? new List<Linea>();
            var ex = carrito.FirstOrDefault(x => x.ProductoId == l.ProductoId);
            if (ex is null)
            {
                carrito.Add(l);
            }
            else
            {
                ex.Cantidad = System.Math.Min(ex.Cantidad + l.Cantidad, ex.Stock);
                // si quieres también refrescar precio/imp:
                ex.Precio = l.Precio;
                ex.Impuesto = l.Impuesto;
                ex.Stock = l.Stock;
            }
            HttpContext.Session.SetJson(KEY, carrito);
            return Ok(carrito);
        }

        // Actualizar línea existente (cantidad y descuentos)
        [HttpPut("actualizar")]
        public IActionResult Actualizar([FromBody] Linea l)
        {
            var carrito = HttpContext.Session.GetJson<List<Linea>>(KEY) ?? new List<Linea>();
            var ex = carrito.FirstOrDefault(x => x.ProductoId == l.ProductoId);
            if (ex is not null)
            {
                ex.Cantidad = System.Math.Max(0, System.Math.Min(l.Cantidad, ex.Stock));
                ex.DescuentoPorc = l.DescuentoPorc;
                ex.DescuentoFijo = l.DescuentoFijo;
            }
            HttpContext.Session.SetJson(KEY, carrito);
            return Ok(carrito);
        }

        [HttpDelete("{productoId:int}")]
        public IActionResult Quitar(int productoId)
        {
            var carrito = HttpContext.Session.GetJson<List<Linea>>(KEY) ?? new List<Linea>();
            carrito.RemoveAll(x => x.ProductoId == productoId);
            HttpContext.Session.SetJson(KEY, carrito);
            return Ok(carrito);
        }

        [HttpDelete]
        public IActionResult Vaciar()
        {
            HttpContext.Session.Remove(KEY);
            return Ok(new { ok = true });
        }
    }
}
