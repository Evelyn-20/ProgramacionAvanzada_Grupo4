using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Usuarios;
using Pasteleria.LogicaDeNegocio.Roles;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    public class UsuarioController : Controller
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

        // GET: Usuario/ListadoUsuarios
        public IActionResult ListadoUsuarios(string buscar)
        {
            try
            {
                List<Usuario> usuarios = new List<Usuario>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    // Buscar por nombre o email
                    var usuariosPorNombre = _listarUsuarios.BuscarPorNombre(buscar);
                    var usuariosPorEmail = _listarUsuarios.BuscarPorEmail(buscar);

                    // Combinar resultados y eliminar duplicados
                    usuarios = usuariosPorNombre.Union(usuariosPorEmail).ToList();
                    ViewBag.Buscar = buscar;
                }
                else
                {
                    usuarios = _listarUsuarios.Obtener();
                }

                // Crear diccionario de roles para mostrar en la vista
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

        // GET: Usuario/CrearUsuario
        [HttpGet]
        public IActionResult CrearUsuario()
        {
            CargarRoles();
            return View();
        }

        // POST: Usuario/CrearUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearUsuario(Usuario usuario)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Nombre: {usuario?.NombreUsuario}");
                System.Diagnostics.Debug.WriteLine($"Email: {usuario?.Email}");
                System.Diagnostics.Debug.WriteLine($"IdRol: {usuario?.IdRol}");

                if (ModelState.IsValid)
                {
                    // Establecer estado como activo
                    usuario.Estado = true;

                    int resultado = await _crearUsuario.Guardar(usuario);

                    if (resultado > 0)
                    {
                        TempData["Exito"] = "Usuario creado exitosamente";
                        return RedirectToAction(nameof(ListadoUsuarios));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo crear el usuario en la base de datos");
                    }
                }
                else
                {
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"  {key}: {error.ErrorMessage}");
                        }
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

        // GET: Usuario/EditarUsuario/5
        [HttpGet]
        public IActionResult EditarUsuario(int id)
        {
            try
            {
                var usuario = _obtenerUsuarioPorId.Obtener(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(ListadoUsuarios));
                }

                CargarRoles();
                // Limpiar la contraseña para que no se muestre en el formulario
                usuario.Contrasenna = string.Empty;
                return View(usuario);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el usuario: " + ex.Message;
                return RedirectToAction(nameof(ListadoUsuarios));
            }
        }

        // POST: Usuario/EditarUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarUsuario(Usuario usuario)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"ID: {usuario?.IdUsuario}");

                if (ModelState.IsValid)
                {
                    int resultado = _actualizarUsuario.Actualizar(usuario);

                    if (resultado > 0)
                    {
                        TempData["Exito"] = "Usuario actualizado exitosamente";
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

        // GET: Usuario/DetallesUsuario/5
        [HttpGet]
        public IActionResult DetallesUsuario(int id)
        {
            try
            {
                var usuario = _obtenerUsuarioPorId.Obtener(id);

                if (usuario == null)
                {
                    TempData["Error"] = "Usuario no encontrado";
                    return RedirectToAction(nameof(ListadoUsuarios));
                }

                // Obtener el nombre del rol
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

        // POST: Usuario/EliminarUsuario
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarUsuario(int IdUsuario)
        {
            try
            {
                int resultado = _eliminarUsuario.Eliminar(IdUsuario);

                if (resultado > 0)
                {
                    TempData["Exito"] = "Usuario eliminado exitosamente";
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

        // Método auxiliar para cargar los roles en el ViewBag
        private void CargarRoles()
        {
            var roles = _listarRoles.Obtener()
                .Where(r => r.Estado) // Solo roles activos
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