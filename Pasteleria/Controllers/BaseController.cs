using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Pasteleria.Controllers
{
    public class BaseController : Controller
    {
        protected bool VerificarPermisosAdministrador()
        {
            // Leer desde Claims en lugar de Session
            var tipoUsuario = User.FindFirst("TipoUsuario")?.Value;

            if (tipoUsuario != "Administrador")
            {
                TempData["Error"] = "No tienes permisos para acceder a esta sección";
                return false;
            }

            return true;
        }

        protected bool VerificarSesionActiva()
        {
            // Leer desde Claims en lugar de Session
            var nombreUsuario = User.Identity?.Name;

            if (string.IsNullOrEmpty(nombreUsuario))
            {
                TempData["Error"] = "Debe iniciar sesión para acceder a esta sección";
                return false;
            }

            return true;
        }
    }
}