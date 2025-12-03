using Microsoft.AspNetCore.Mvc;
using Pasteleria.LogicaDeNegocio.Ventas; 
using Rotativa.AspNetCore;
using Pasteleria.Abstracciones.ModeloUI;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Pasteleria.Controllers
{
    public class VentaController : Controller
    {
        private readonly ListarVentas _servicio;

        public VentaController()
        {
            _servicio = new ListarVentas();
        }



        private void CargarCombos()
        {
            var servicioClientes = new Pasteleria.LogicaDeNegocio.Clientes.ListarClientes();
            var servicioUsuarios = new Pasteleria.LogicaDeNegocio.Usuarios.ListarUsuarios();

            ViewBag.Clientes = servicioClientes.Obtener();
            ViewBag.Usuarios = servicioUsuarios.Obtener();
        }




        public IActionResult GenerarPDF()
        {
            CargarCombos();
            return View();
        }


        [HttpPost]
        public IActionResult GenerarPDFResult(DateTime FechaInicio, DateTime FechaFin, int? IdCliente, int? IdUsuario)
        {
            var ventas = _servicio.Obtener();

            ventas = ventas
                .Where(v => v.FechaVenta >= FechaInicio && v.FechaVenta <= FechaFin)
                .ToList();

            if (IdCliente.HasValue)
                ventas = ventas.Where(v => v.IdCliente == IdCliente.Value).ToList();

            if (IdUsuario.HasValue)
                ventas = ventas.Where(v => v.IdUsuario == IdUsuario.Value).ToList();

            // Necesario para rellenar los combos nuevamente
            CargarCombos();

            return View(ventas);
        }

        public IActionResult ExportarPDF()
        {
            var ventas = _servicio.Obtener(); // tus datos filtrados

            return new ViewAsPdf("GenerarPDFResult", ventas)
            {
                FileName = "ReporteVentas.pdf",
                PageSize = Rotativa.AspNetCore.Options.Size.A4,
                PageOrientation = Rotativa.AspNetCore.Options.Orientation.Landscape,
                CustomSwitches = "--disable-smart-shrinking",
                // Especificar el layout PDF
                ViewData = new ViewDataDictionary<List<Venta>>(ViewData, ventas)
                {
                    ["Layout"] = "_LayoutPDF"
                }
            };
        }




    }
}
