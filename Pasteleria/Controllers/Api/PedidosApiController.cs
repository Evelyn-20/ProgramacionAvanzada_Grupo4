using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


[ApiController]
[Route("api/pedidos")]
public class PedidosApiController : ControllerBase
{
    public record LineaReq(int ProductoId, decimal Cantidad, decimal? DescuentoPorc, decimal? DescuentoFijo);
    public record CalcularReq(List<LineaReq> Lineas);

    // POST /api/pedidos/calcular
    [HttpPost("calcular")]
    public async Task<IActionResult> Calcular([FromBody] CalcularReq req)
    {
        if (req?.Lineas == null || req.Lineas.Count == 0)
            return Ok(new { subtotal = 0m, impuestos = 0m, total = 0m, lineas = Array.Empty<object>() });

        using var db = new Contexto(); 

        var ids = req.Lineas.Select(l => l.ProductoId).Distinct().ToList();

        // Cargamos productos en memoria: Precio, PorcentajeImpuesto (%), Cantidad (stock)
        var productos = await db.Producto
            .Where(p => ids.Contains(p.IdProducto))
            .ToDictionaryAsync(p => p.IdProducto);

        decimal subtotal = 0m, impuestos = 0m;
        var lineasOut = new List<object>();

        foreach (var l in req.Lineas)
        {
            if (!productos.TryGetValue(l.ProductoId, out var p)) continue;

            var qty = Math.Max(0m, l.Cantidad);
            var precioUnit = p.Precio; 

            // Bruto sin descuentos
            var bruto = precioUnit * qty;

            // Descuento %
            var dPct = Math.Clamp(l.DescuentoPorc ?? 0m, 0m, 100m);
            var montoPct = decimal.Round(bruto * (dPct / 100m), 2);

            // Descuento fijo
            var dFijo = Math.Max(0m, l.DescuentoFijo ?? 0m);

            // Neto sin impuesto (no negativo)
            var netoSinImp = Math.Max(0m, bruto - montoPct - dFijo);

            // Impuesto por línea: PorcentajeImpuesto es porcentaje (ej. 13)
            var impLinea = decimal.Round(netoSinImp * (p.PorcentajeImpuesto / 100m), 2);
            var totalLinea = netoSinImp + impLinea;

            subtotal += netoSinImp;
            impuestos += impLinea;

            lineasOut.Add(new
            {
                productoId = p.IdProducto,
                nombre = p.NombreProducto,
                cantidad = qty,
                precioUnit,
                descuentoAplicado = new { porcentaje = dPct, fijo = dFijo },
                impuestoPorc = p.PorcentajeImpuesto,
                neto = netoSinImp,
                impuesto = impLinea,
                totalLinea
            });
        }

        var total = subtotal + impuestos;

        return Ok(new
        {
            subtotal = decimal.Round(subtotal, 2),
            impuestos = decimal.Round(impuestos, 2),
            total = decimal.Round(total, 2),
            lineas = lineasOut
        });
    }
}
