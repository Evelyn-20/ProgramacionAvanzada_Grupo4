using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace Pasteleria.Controllers
{
    public class BaseController : Controller
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            // Guardar información del usuario en ViewBag para las vistas
            ViewBag.UsuarioAutenticado = User.Identity?.IsAuthenticated ?? false;
            ViewBag.TipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            ViewBag.NombreUsuario = User.Identity?.Name;
            ViewBag.UsuarioId = ObtenerUsuarioId();
        }

        // Verificación General

        protected bool VerificarSesionActiva()
        {
            var nombreUsuario = User.Identity?.Name;
            if (string.IsNullOrEmpty(nombreUsuario))
            {
                TempData["Error"] = "Debe iniciar sesión para acceder a esta sección";
                return false;
            }
            return true;
        }

        protected bool TieneRol(params string[] rolesPermitidos)
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (string.IsNullOrEmpty(tipoUsuario))
            {
                TempData["Error"] = "Debe iniciar sesión para acceder a esta sección";
                return false;
            }

            foreach (var rol in rolesPermitidos)
            {
                if (tipoUsuario.Equals(rol, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            TempData["Error"] = "No tienes permisos para acceder a esta sección";
            return false;
        }

        // Permisos por funcionalidad
        protected bool PuedeVerCatalogo()
        {
            return TieneRol("Administrador", "Ventas", "Operaciones", "Supervisor",
                           "Vendedor", "Operador", "Contador", "Cliente");
        }

        protected bool PuedeGestionarInventario()
        {
            return TieneRol("Administrador", "Operaciones", "Supervisor", "Operador");
        }

        protected bool PuedeEliminarProductos()
        {
            return TieneRol("Administrador");
        }

        protected bool PuedeGestionarPedidos()
        {
            return TieneRol("Administrador", "Ventas", "Operaciones", "Supervisor", "Vendedor", "Operador");
        }

        protected bool PuedeCrearPedidos()
        {
            return TieneRol("Administrador", "Ventas", "Supervisor", "Vendedor");
        }

        protected bool PuedeGestionarClientes()
        {
            return TieneRol("Administrador", "Ventas", "Supervisor", "Vendedor");
        }

        protected bool PuedeGestionarUsuarios()
        {
            return TieneRol("Administrador");
        }

        protected bool PuedeVerReportes()
        {
            return TieneRol("Administrador", "Ventas", "Operaciones", "Supervisor", "Contador");
        }

        protected bool PuedeGestionarCategorias()
        {
            return TieneRol("Administrador", "Operaciones", "Supervisor", "Operador");
        }

        protected bool PuedeVerAuditorias()
        {
            return TieneRol("Administrador");
        }

        // Verificar Rol
        protected bool EsAdministrador()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Administrador";
        }

        protected bool EsVentas()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Ventas";
        }

        protected bool EsOperaciones()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Operaciones";
        }

        protected bool EsSupervisor()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Supervisor";
        }

        protected bool EsVendedor()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Vendedor";
        }

        protected bool EsContador()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Contador";
        }

        protected bool EsOperador()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Operador";
        }

        protected bool EsCliente()
        {
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;
            return tipoUsuario == "Cliente";
        }

        // Obtener información del Usuario
        protected int? ObtenerUsuarioId()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (int.TryParse(userId, out int id))
            {
                return id;
            }
            return null;
        }

        protected string ObtenerTipoUsuario()
        {
            return User.FindFirst("TipoUsuario")?.Value;
        }

        protected string ObtenerNombreUsuario()
        {
            return User.Identity?.Name;
        }

        protected IActionResult RedirectSinPermiso(string mensaje = null)
        {
            TempData["Error"] = mensaje ?? "No tienes permisos para acceder a esta sección";
            return RedirectToAction("Index", "Home");
        }
    }
}