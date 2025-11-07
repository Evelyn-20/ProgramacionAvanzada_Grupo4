using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Clientes;
using Pasteleria.LogicaDeNegocio.Usuarios;
using System;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    public class AccountController : Controller
    {
        private readonly ICrearCliente _crearCliente;
        private readonly IListarClientes _listarClientes;
        private readonly IAutenticarCliente _autenticarCliente;
        private readonly IAutenticarUsuario _autenticarUsuario;

        public AccountController()
        {
            _crearCliente = new CrearCliente();
            _listarClientes = new ListarClientes();
            _autenticarCliente = new AutenticarCliente();
            _autenticarUsuario = new AutenticarUsuario();
        }

        // GET: Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public IActionResult Login(string email, string password)
        {
            try
            {
                if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
                {
                    TempData["Error"] = "Por favor complete todos los campos";
                    return View();
                }

                // Primero intentar como Usuario (administrador con rol)
                var (usuario, nombreRol) = _autenticarUsuario.Autenticar(email, password);

                if (usuario != null)
                {
                    // Login como Usuario (admin)
                    HttpContext.Session.SetInt32("UsuarioId", usuario.IdUsuario);
                    HttpContext.Session.SetString("UsuarioNombre", usuario.NombreUsuario);
                    HttpContext.Session.SetString("UsuarioEmail", usuario.Email);
                    HttpContext.Session.SetString("UsuarioRol", nombreRol);
                    HttpContext.Session.SetString("TipoUsuario", "Administrador");

                    TempData["Success"] = $"Bienvenido {usuario.NombreUsuario} ({nombreRol})";
                    return RedirectToAction("Index", "Home");
                }

                // Si no es Usuario, intentar como Cliente
                var cliente = _autenticarCliente.Autenticar(email, password);

                if (cliente != null)
                {
                    // Login como Cliente
                    HttpContext.Session.SetInt32("ClienteId", cliente.IdCliente);
                    HttpContext.Session.SetString("ClienteNombre", cliente.NombreCliente);
                    HttpContext.Session.SetString("ClienteEmail", cliente.Correo);
                    HttpContext.Session.SetString("TipoUsuario", "Cliente");

                    TempData["Success"] = "Inicio de sesión exitoso. ¡Bienvenido!";
                    return RedirectToAction("Index", "Home");
                }

                TempData["Error"] = "Correo o contraseña incorrectos";
                return View();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN LOGIN: {ex.Message}");
                TempData["Error"] = "Error al iniciar sesión. Por favor intente nuevamente.";
                return View();
            }
        }

        // GET: Account/Registro
        public IActionResult Registro()
        {
            return View();
        }

        // POST: Account/Registro
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Registro(Cliente cliente)
        {
            try
            {
                System.Diagnostics.Debug.WriteLine($"Intentando registrar: {cliente?.NombreCliente}");
                System.Diagnostics.Debug.WriteLine($"Email: {cliente?.Correo}");

                if (ModelState.IsValid)
                {
                    // Verificar si el correo ya existe
                    var clienteExistente = _listarClientes.BuscarPorCorreo(cliente.Correo);
                    if (clienteExistente != null && clienteExistente.Count > 0)
                    {
                        TempData["Error"] = "El correo electrónico ya está registrado";
                        return View(cliente);
                    }

                    // Verificar si la cédula ya existe
                    var clientePorCedula = _listarClientes.BuscarPorCedula(cliente.Cedula);
                    if (clientePorCedula != null && clientePorCedula.Count > 0)
                    {
                        TempData["Error"] = "La cédula ya está registrada";
                        return View(cliente);
                    }

                    // Establecer estado como activo
                    cliente.Estado = true;

                    int resultado = await _crearCliente.Guardar(cliente);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "¡Registro exitoso! Ya puedes iniciar sesión con tu cuenta";
                        return RedirectToAction("Login");
                    }
                    else
                    {
                        TempData["Error"] = "No se pudo completar el registro. Por favor intente nuevamente.";
                    }
                }
                else
                {
                    // Log de errores de validación
                    foreach (var key in ModelState.Keys)
                    {
                        var errors = ModelState[key].Errors;
                        foreach (var error in errors)
                        {
                            System.Diagnostics.Debug.WriteLine($"Error en {key}: {error.ErrorMessage}");
                        }
                    }
                    TempData["Error"] = "Por favor complete correctamente todos los campos requeridos";
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN REGISTRO: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"StackTrace: {ex.StackTrace}");
                TempData["Error"] = "Error al procesar el registro: " + ex.Message;
                return View(cliente);
            }
        }

        // GET: Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            TempData["Success"] = "Has cerrado sesión exitosamente";
            return RedirectToAction("Index", "Home");
        }
    }
}