using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pasteleria.AccesoADatos.Modelos; 
using Pasteleria;
using Microsoft.AspNetCore.Authorization;

namespace Pasteleria.Controllers.Api
{
    [AllowAnonymous]
    [ApiController]
    [Route("api/clientes")]
    public class ClientesApiController : ControllerBase
    {
        // GET /api/clientes?buscar=&page=1&pageSize=10
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string? buscar, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (page < 1) page = 1;
            if (pageSize < 1 || pageSize > 100) pageSize = 10;

            using var db = new Contexto();

            var q = db.Cliente.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var term = buscar.Trim();
                q = q.Where(c =>
                    EF.Functions.Like(c.NombreCliente, $"%{term}%") ||
                    EF.Functions.Like(c.Cedula, $"%{term}%") ||
                    EF.Functions.Like(c.Correo, $"%{term}%") ||
                    EF.Functions.Like(c.Telefono, $"%{term}%"));
            }

            var total = await q.CountAsync();

            var items = await q
                .OrderBy(c => c.NombreCliente)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new {
                    idCliente = c.IdCliente,
                    cedula = c.Cedula,
                    nombre = c.NombreCliente,
                    correo = c.Correo,
                    telefono = c.Telefono,
                    direccion = c.Direccion,
                    estado = c.Estado
                })
                .ToListAsync();

            return Ok(new { items, total, page, pageSize });
        }
    }
}
