using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Pedido;
using Rotativa.AspNetCore;
using Pasteleria.Abstracciones.ModeloUI;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Linq;
using System.Collections.Generic;

namespace Pasteleria.Controllers
{
    public class ReportePedidoController : Controller
    {
        private readonly IListarPedidos _listarPedidos;
        private readonly IObtenerPedido _obtenerPedido;

        public ReportePedidoController(
            IListarPedidos listarPedidos,
            IObtenerPedido obtenerPedido)
        {
            _listarPedidos = listarPedidos;
            _obtenerPedido = obtenerPedido;
        }

        private void CargarCombos()
        {
            var servicioClientes = new Pasteleria.LogicaDeNegocio.Clientes.ListarClientes();
            var servicioUsuarios = new Pasteleria.LogicaDeNegocio.Usuarios.ListarUsuarios();

            ViewBag.Clientes = servicioClientes.Obtener();
            ViewBag.Usuarios = servicioUsuarios.Obtener();

            // Cargar estados
            var gestionarEstados = new Pasteleria.AccesoADatos.Pedidos.GestionarEstadosPedido();
            ViewBag.Estados = gestionarEstados.ObtenerEstados();
        }

        // GET: /ReportePedido/GenerarReporte
        public IActionResult GenerarReporte()
        {
            CargarCombos();

            // Si hay TempData con los filtros anteriores, mantenerlos
            if (TempData["FechaInicio"] != null)
            {
                ViewBag.FechaInicio = TempData["FechaInicio"];
                ViewBag.FechaFin = TempData["FechaFin"];
                ViewBag.IdCliente = TempData["IdCliente"];
                ViewBag.IdUsuario = TempData["IdUsuario"];
                ViewBag.Estado = TempData["Estado"];
            }

            return View();
        }

        // POST: /ReportePedido/GenerarReporteResult
        [HttpPost]
        public IActionResult GenerarReporteResult(DateTime FechaInicio, DateTime FechaFin, int? IdCliente, int? IdUsuario, string Estado)
        {
            try
            {
                // Validar fechas
                if (FechaInicio > FechaFin)
                {
                    TempData["Error"] = "La fecha de inicio no puede ser mayor a la fecha fin";

                    // Guardar valores en TempData para mantenerlos
                    TempData["FechaInicio"] = FechaInicio.ToString("yyyy-MM-dd");
                    TempData["FechaFin"] = FechaFin.ToString("yyyy-MM-dd");
                    TempData["IdCliente"] = IdCliente;
                    TempData["IdUsuario"] = IdUsuario;
                    TempData["Estado"] = Estado;

                    return RedirectToAction("GenerarReporte");
                }

                // Obtener todos los pedidos en el rango de fechas
                var pedidos = _listarPedidos.ObtenerPorFecha(FechaInicio, FechaFin);

                // Filtrar por estado si se especifica
                if (!string.IsNullOrWhiteSpace(Estado) && Estado != "Todos")
                {
                    pedidos = pedidos.Where(p => p.Estado == Estado).ToList();
                }

                // Filtrar por cliente si se especifica
                if (IdCliente.HasValue && IdCliente.Value > 0)
                {
                    pedidos = pedidos.Where(p => p.IdCliente == IdCliente.Value).ToList();
                }

                // Filtrar por usuario si se especifica
                if (IdUsuario.HasValue && IdUsuario.Value > 0)
                {
                    pedidos = pedidos.Where(p => p.IdUsuario == IdUsuario.Value).ToList();
                }

                // Crear modelo para la vista con detalles de productos
                var reporteCompleto = new List<ReportePedidoDetalle>();

                foreach (var pedido in pedidos)
                {
                    var detalles = _obtenerPedido.ObtenerDetalles(pedido.IdPedido);

                    reporteCompleto.Add(new ReportePedidoDetalle
                    {
                        Pedido = pedido,
                        Detalles = detalles
                    });
                }

                // Guardar los filtros en ViewBag para la vista
                ViewBag.FechaInicio = FechaInicio.ToString("yyyy-MM-dd");
                ViewBag.FechaFin = FechaFin.ToString("yyyy-MM-dd");
                ViewBag.IdCliente = IdCliente;
                ViewBag.IdUsuario = IdUsuario;
                ViewBag.Estado = Estado ?? "Todos";

                // Estadísticas del reporte
                ViewBag.TotalPedidos = reporteCompleto.Count;
                ViewBag.TotalGeneral = reporteCompleto.Sum(r => r.Pedido.Total);
                ViewBag.TotalProductos = reporteCompleto.Sum(r => r.Detalles.Sum(d => d.Cantidad));

                // Recargar combos para mostrarlos en la vista
                CargarCombos();

                return View(reporteCompleto);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al generar reporte: {ex.Message}";
                return RedirectToAction("GenerarReporte");
            }
        }

        // GET: /ReportePedido/ExportarPDF
        public IActionResult ExportarPDF(string FechaInicio, string FechaFin, int? IdCliente, int? IdUsuario, string Estado)
        {
            try
            {
                // Validar que se proporcionen fechas
                if (string.IsNullOrWhiteSpace(FechaInicio) || string.IsNullOrWhiteSpace(FechaFin))
                {
                    TempData["Error"] = "Debe especificar un rango de fechas";
                    return RedirectToAction("GenerarReporte");
                }

                // Parsear las fechas con formato específico
                DateTime fechaInicioDate;
                DateTime fechaFinDate;

                // Intentar parsear con formato yyyy-MM-dd primero (que viene del formulario)
                if (!DateTime.TryParseExact(FechaInicio, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out fechaInicioDate))
                {
                    // Si falla, intentar parseo normal
                    if (!DateTime.TryParse(FechaInicio, out fechaInicioDate))
                    {
                        TempData["Error"] = "La fecha de inicio no es válida";
                        return RedirectToAction("GenerarReporte");
                    }
                }

                if (!DateTime.TryParseExact(FechaFin, "yyyy-MM-dd",
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.None,
                    out fechaFinDate))
                {
                    // Si falla, intentar parseo normal
                    if (!DateTime.TryParse(FechaFin, out fechaFinDate))
                    {
                        TempData["Error"] = "La fecha fin no es válida";
                        return RedirectToAction("GenerarReporte");
                    }
                }

                // Obtener todos los pedidos en el rango de fechas
                var pedidos = _listarPedidos.ObtenerPorFecha(fechaInicioDate, fechaFinDate);

                // Aplicar filtros
                if (!string.IsNullOrWhiteSpace(Estado) && Estado != "Todos")
                {
                    pedidos = pedidos.Where(p => p.Estado == Estado).ToList();
                }

                if (IdCliente.HasValue && IdCliente.Value > 0)
                {
                    pedidos = pedidos.Where(p => p.IdCliente == IdCliente.Value).ToList();
                }

                if (IdUsuario.HasValue && IdUsuario.Value > 0)
                {
                    pedidos = pedidos.Where(p => p.IdUsuario == IdUsuario.Value).ToList();
                }

                // Crear modelo para el PDF con detalles
                var reporteCompleto = new List<ReportePedidoDetalle>();

                foreach (var pedido in pedidos)
                {
                    var detalles = _obtenerPedido.ObtenerDetalles(pedido.IdPedido);

                    reporteCompleto.Add(new ReportePedidoDetalle
                    {
                        Pedido = pedido,
                        Detalles = detalles
                    });
                }

                // Preparar ViewBag con estadísticas para el PDF
                ViewBag.FechaInicio = fechaInicioDate.ToString("dd/MM/yyyy");
                ViewBag.FechaFin = fechaFinDate.ToString("dd/MM/yyyy");
                ViewBag.FechaGeneracion = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
                ViewBag.TotalPedidos = reporteCompleto.Count;
                ViewBag.TotalGeneral = reporteCompleto.Sum(r => r.Pedido.Total);
                ViewBag.TotalProductos = reporteCompleto.Sum(r => r.Detalles.Sum(d => d.Cantidad));

                // Filtros aplicados (para mostrar en la info del PDF)
                ViewBag.FiltroEstado = Estado ?? "Todos";

                // Obtener nombres de cliente y usuario si están filtrados
                if (IdCliente.HasValue && IdCliente.Value > 0)
                {
                    var servicioClientes = new Pasteleria.LogicaDeNegocio.Clientes.ListarClientes();
                    var cliente = servicioClientes.Obtener().FirstOrDefault(c => c.IdCliente == IdCliente.Value);
                    ViewBag.FiltroCliente = cliente?.NombreCliente ?? "";
                }
                else
                {
                    ViewBag.FiltroCliente = "";
                }

                if (IdUsuario.HasValue && IdUsuario.Value > 0)
                {
                    var servicioUsuarios = new Pasteleria.LogicaDeNegocio.Usuarios.ListarUsuarios();
                    var usuario = servicioUsuarios.Obtener().FirstOrDefault(u => u.IdUsuario == IdUsuario.Value);
                    ViewBag.FiltroUsuario = usuario?.NombreUsuario ?? "";
                }
                else
                {
                    ViewBag.FiltroUsuario = "";
                }

                // Generar PDF
                return new ViewAsPdf("ReportePDF", reporteCompleto)
                {
                    FileName = $"ReportePedidos_{fechaInicioDate:yyyyMMdd}_{fechaFinDate:yyyyMMdd}.pdf",
                    PageSize = Rotativa.AspNetCore.Options.Size.A4,
                    PageOrientation = Rotativa.AspNetCore.Options.Orientation.Portrait,
                    PageMargins = new Rotativa.AspNetCore.Options.Margins(10, 10, 10, 10),
                    CustomSwitches = "--disable-smart-shrinking --enable-local-file-access"
                };
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al exportar PDF: {ex.Message}";
                return RedirectToAction("GenerarReporte");
            }
        }
    }

    // Clase auxiliar para el reporte
    public class ReportePedidoDetalle
    {
        public Pedido Pedido { get; set; }
        public List<DetallePedido> Detalles { get; set; }
    }
}