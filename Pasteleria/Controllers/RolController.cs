using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    [Authorize]
    public class RolController : BaseController
    {
        private IListarRoles _listarRol;
        private ICrearRol _crearRol;
        private IObtenerRol _obtenerRolPorId;
        private IActualizarRol _actualizarRol;
        private IEliminarRol _eliminarRol;

        public RolController()
        {
            try
            {
                _listarRol = new ListarRoles();
                _crearRol = new CrearRol();
                _obtenerRolPorId = new ObtenerRol();
                _actualizarRol = new ActualizarRol();
                _eliminarRol = new EliminarRol();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult ListadoRoles(string buscar)
        {
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                List<Rol> roles = new List<Rol>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    roles = _listarRol.BuscarPorNombre(buscar);
                    ViewBag.Buscar = buscar;
                }
                else
                {
                    roles = _listarRol.Obtener();
                }

                return View(roles);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar roles: {ex.Message}";
                return View(new List<Rol>());
            }
        }

        [HttpGet]
        public IActionResult CrearRol()
        {
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            if (!EsAdministrador())
            {
                return RedirectSinPermiso("Solo los administradores pueden crear roles");
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearRol(Rol rol)
        {
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            if (!EsAdministrador())
            {
                return RedirectSinPermiso("Solo los administradores pueden crear roles");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    rol.Estado = true;

                    int resultado = await _crearRol.Guardar(rol);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Rol creado exitosamente";
                        return RedirectToAction(nameof(ListadoRoles));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo crear el rol en la base de datos");
                    }
                }

                return View(rol);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(rol);
            }
        }

        [HttpGet]
        public IActionResult EditarRol(int id)
        {
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            if (!EsAdministrador())
            {
                return RedirectSinPermiso("Solo los administradores pueden editar roles");
            }

            try
            {
                var rol = _obtenerRolPorId.Obtener(id);

                if (rol == null)
                {
                    TempData["Error"] = "Rol no encontrado";
                    return RedirectToAction(nameof(ListadoRoles));
                }

                return View(rol);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el rol: " + ex.Message;
                return RedirectToAction(nameof(ListadoRoles));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarRol(Rol rol)
        {
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            if (!EsAdministrador())
            {
                return RedirectSinPermiso("Solo los administradores pueden editar roles");
            }

            try
            {
                if (ModelState.IsValid)
                {
                    int resultado = _actualizarRol.Actualizar(rol);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Rol actualizado exitosamente";
                        return RedirectToAction(nameof(ListadoRoles));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el rol");
                    }
                }

                return View(rol);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el rol: " + ex.Message);
                return View(rol);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarRol(int IdRol)
        {
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                int resultado = _eliminarRol.Eliminar(IdRol);

                if (resultado > 0)
                {
                    TempData["Success"] = "Rol eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el rol";
                }

                return RedirectToAction(nameof(ListadoRoles));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el rol: " + ex.Message;
                return RedirectToAction(nameof(ListadoRoles));
            }
        }
    }
}