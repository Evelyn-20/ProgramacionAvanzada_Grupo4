using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;
using QuestPDF.Fluent;
using System;
using System.Collections.Generic;
using System.Linq;
using Pasteleria.Reportes.PDF;
using QuestPDF.Fluent;
using Pasteleria.Reportes.PDF;

namespace Pasteleria.Controllers
{
    [Authorize] // Requiere autenticación
    public class ReportePedidoController : BaseController
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
            // Solo Admin, Ventas, Operaciones y Contador pueden ver reportes
            if (!PuedeVerReportes())
            {
                return RedirectSinPermiso("No tiene permisos para ver reportes");
            }

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
            // Validar permisos
            if (!PuedeVerReportes())
            {
                return RedirectSinPermiso("No tiene permisos para ver reportes");
            }

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
                var pedidos = _listarPedidos.ObtenerPorFecha(FechaInicio, FechaFin.AddDays(1));

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
        public IActionResult ExportarPDF(string FechaInicio, string FechaFin, int? IdCliente, int? IdUsuario, string Estado)
        {
            if (!PuedeVerReportes())
                return RedirectSinPermiso("No tiene permisos");

            DateTime fi = DateTime.Parse(FechaInicio);
            DateTime ff = DateTime.Parse(FechaFin);

            var pedidos = _listarPedidos.ObtenerPorFecha(fi, ff.AddDays(1));

            // Aplicar filtros
            if (!string.IsNullOrWhiteSpace(Estado) && Estado != "Todos")
                pedidos = pedidos.Where(p => p.Estado == Estado).ToList();

            if (IdCliente.HasValue && IdCliente > 0)
                pedidos = pedidos.Where(p => p.IdCliente == IdCliente).ToList();

            if (IdUsuario.HasValue && IdUsuario > 0)
                pedidos = pedidos.Where(p => p.IdUsuario == IdUsuario).ToList();

            var reporte = new List<ReportePedidoDetalle>();

            foreach (var p in pedidos)
            {
                reporte.Add(new ReportePedidoDetalle
                {
                    Pedido = p,
                    Detalles = _obtenerPedido.ObtenerDetalles(p.IdPedido)
                });
            }

            var doc = new ReportePedidosPDF(
                reporte,
                fi.ToString("dd/MM/yyyy"),
                ff.ToString("dd/MM/yyyy")
            );

            var pdf = doc.GeneratePdf();

            return File(
                pdf,
                "application/pdf",
                $"ReportePedidos_{fi:yyyyMMdd}_{ff:yyyyMMdd}.pdf"
            );
        }
    }

    // Clase auxiliar para el reporte
    public class ReportePedidoDetalle
    {
        public Pedido Pedido { get; set; }
        public List<DetallePedido> Detalles { get; set; }
    }
}