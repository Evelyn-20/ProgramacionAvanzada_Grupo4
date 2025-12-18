using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.LogicaDeNegocio.Roles;
using Pasteleria.LogicaDeNegocio.Usuarios;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    [Authorize] // Requiere autenticación
    public class UsuarioController : BaseController
    {
        private IListarUsuarios _listarUsuarios;
        private ICrearUsuario _crearUsuario;
        private IObtenerUsuario _obtenerUsuarioPorId;
        private IActualizarUsuario _actualizarUsuario;
        private IEliminarUsuario _eliminarUsuario;
        private IListarRoles _listarRoles;

        public UsuarioController()
        {
            try
            {
                _listarUsuarios = new ListarUsuarios();
                _crearUsuario = new CrearUsuario();
                _obtenerUsuarioPorId = new ObtenerUsuario();
                _actualizarUsuario = new ActualizarUsuario();
                _eliminarUsuario = new EliminarUsuario();
                _listarRoles = new ListarRoles();
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        [HttpGet]
        public IActionResult ListadoUsuarios(string buscar)
        {
            // Solo Admin puede gestionar usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                List<Usuario> usuarios = new List<Usuario>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    var usuariosPorNombre = _listarUsuarios.BuscarPorNombre(buscar);
                    var usuariosPorEmail = _listarUsuarios.BuscarPorEmail(buscar);

                    usuarios = usuariosPorNombre.Union(usuariosPorEmail).ToList();
                    ViewBag.Buscar = buscar;
                }
                else
                {
                    usuarios = _listarUsuarios.Obtener();
                }

                var roles = _listarRoles.Obtener();
                var rolesDict = roles.ToDictionary(r => r.IdRol, r => r.NombreRol);
                ViewBag.RolesDict = rolesDict;

                return View(usuarios);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar usuarios: {ex.Message}";
                return View(new List<Usuario>());
            }
        }

        [HttpGet]
        public IActionResult CrearUsuario()
        {
            // Solo Admin puede crear usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            CargarRoles();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(Usuario usuario)
        {
            // Solo Admin puede crear usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                // Validación manual para creación
                if (string.IsNullOrWhiteSpace(usuario.Contrasenna))
                {
                    ModelState.AddModelError("Contrasenna", "La contraseña es obligatoria");
                }

                if (ModelState.IsValid)
                {
                    usuario.Estado = true;

                    int resultado = await _crearUsuario.Guardar(usuario);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Usuario creado exitosamente";
                        return RedirectToAction(nameof(ListadoUsuarios));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo crear el usuario en la base de datos");
                    }
                }

                CargarRoles();
                return View(usuario);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                CargarRoles();
                return View(usuario);
            }
        }

        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            // Solo Admin puede editar usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                var usuario = _obtenerUsuarioPorId.Obtener(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(ListadoUsuarios));
                }

                CargarRoles();
                usuario.Contrasenna = string.Empty;
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el usuario: " + ex.Message;
                return RedirectToAction(nameof(ListadoUsuarios));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarUsuario(Usuario usuario)
        {
            // Solo Admin puede editar usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    int resultado = _actualizarUsuario.Actualizar(usuario);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Usuario actualizado exitosamente";
                        return RedirectToAction(nameof(ListadoUsuarios));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el usuario");
                    }
                }

                CargarRoles();
                return View(usuario);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el usuario: " + ex.Message);
                CargarRoles();
                return View(usuario);
            }
        }

        [HttpGet]
        public IActionResult DetallesUsuario(int id)
        {
            // Solo Admin puede ver detalles de usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                var usuario = _obtenerUsuarioPorId.Obtener(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(ListadoUsuarios));
                }

                var roles = _listarRoles.Obtener();
                var rol = roles.FirstOrDefault(r => r.IdRol == usuario.IdRol);
                ViewBag.NombreRol = rol?.NombreRol ?? "Sin rol";

                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar los detalles del usuario: " + ex.Message;
                return RedirectToAction(nameof(ListadoUsuarios));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarUsuario(int IdUsuario)
        {
            // Solo Admin puede eliminar usuarios
            if (!PuedeGestionarUsuarios())
            {
                return RedirectSinPermiso();
            }

            try
            {
                int resultado = _eliminarUsuario.Eliminar(IdUsuario);

                if (resultado > 0)
                {
                    TempData["Success"] = "Usuario eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el usuario";
                }

                return RedirectToAction(nameof(ListadoUsuarios));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el usuario: " + ex.Message;
                return RedirectToAction(nameof(ListadoUsuarios));
            }
        }

        private void CargarRoles()
        {
            var roles = _listarRoles.Obtener()
                .Where(r => r.Estado)
                .Select(r => new SelectListItem
                {
                    Value = r.IdRol.ToString(),
                    Text = r.NombreRol
                })
                .ToList();

            ViewBag.Roles = roles;
        }
    }
}