using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/productos")]
public class ProductosApiController : ControllerBase
{

    // GET /api/productos/buscar?q=texto
    [HttpGet("buscar")]
    public async Task<IActionResult> Buscar([FromQuery] string q)
    {
        using var db = new Contexto(); 
        var term = (q ?? string.Empty).Trim();

        if (term.Length < 2)
            return Ok(Array.Empty<object>());

        // Ajustado nombres de columnas (ProductoAD)
        var items = await db.Producto
            .AsNoTracking()
            .Where(p => p.Estado == true &&
                        EF.Functions.Like(p.NombreProducto, $"%{term}%"))
            .OrderBy(p => p.NombreProducto)
            .Take(10)
            .Select(p => new
            {
                id = p.IdProducto,
                nombre = p.NombreProducto,
                precio = p.Precio,
                impuesto = p.PorcentajeImpuesto, 
                stock = p.Cantidad               
            })
            .ToListAsync();

        return Ok(items);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> ObtenerPorId([FromRoute] int id)
    {
        using var db = new Contexto(); 
        var p = await db.Producto
            .AsNoTracking()
            .Where(x => x.IdProducto == id && x.Estado == true)
            .Select(x => new {
                id = x.IdProducto,
                nombre = x.NombreProducto,
                precio = x.Precio,                
                impuesto = x.PorcentajeImpuesto, 
                stock = x.Cantidad               
            })
            .FirstOrDefaultAsync();

        if (p == null) return NotFound();
        return Ok(p);
    }


}
