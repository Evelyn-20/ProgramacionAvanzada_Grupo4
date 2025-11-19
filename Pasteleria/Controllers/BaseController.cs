using Microsoft.AspNetCore.Mvc;

namespace Pasteleria.Controllers
{
    public class BaseController : Controller
    {
        protected bool VerificarPermisosAdministrador()
        {
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (tipoUsuario != "Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return false;
            }

            return true;
        }

        protected bool VerificarSesionActiva()
        {
            var nombreUsuario = HttpContext.Session.GetString("UsuarioNombre") ??
                               HttpContext.Session.GetString("ClienteNombre");

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                TempData["Error"] = "Debe iniciar sesión para acceder a esta sección";
                return false;
            }

            return true;
        }

        protected bool TieneRol(params string[] rolesPermitidos)
        {
            var tipoUsuario = HttpContext.Session.GetString("TipoUsuario");

            if (tipoUsuario != "Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return false;
            }

            var rolActual = HttpContext.Session.GetString("UsuarioRol");

            foreach (var rol in rolesPermitidos)
            {
                if (rolActual == rol)
                {
                    return true;
                }
            }

            TempData["Error"] = "Tu rol no tiene permisos para realizar esta acción";
            return false;
        }

        protected bool PuedeGestionarInventario()
        {
            return TieneRol("Admin", "Operaciones");
        }

        protected bool PuedeGestionarPedidos()
        {
            return TieneRol("Admin", "Ventas");
        }

        protected bool PuedeEliminarProductos()
        {
            return TieneRol("Admin");
        }

        protected bool PuedeGestionarUsuarios()
        {
            return TieneRol("Admin");
        }

        protected bool PuedeVerCatalogo()
        {
            return TieneRol("Admin", "Ventas", "Operaciones");
        }
    }
}
