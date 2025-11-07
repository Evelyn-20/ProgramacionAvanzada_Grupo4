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
    }
}
