using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Clientes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.Controllers
{
    [Authorize] // Requiere autenticación
    public class ClienteController : BaseController
    {
        private IListarClientes _listarCliente;
        private ICrearCliente _crearCliente;
        private IObtenerCliente _obtenerClientePorId;
        private IActualizarCliente _actualizarCliente;
        private IEliminarCliente _eliminarCliente;

        public ClienteController()
        {
            try
            {
                _listarCliente = new ListarClientes();
                _crearCliente = new CrearCliente();
                _obtenerClientePorId = new ObtenerCliente();
                _actualizarCliente = new ActualizarCliente();
                _eliminarCliente = new EliminarCliente();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR EN CONSTRUCTOR: {ex.Message}");
                throw;
            }
        }

        [HttpGet]
        public IActionResult ListadoClientes(string buscar)
        {
            if (!PuedeGestionarClientes())
            {
                return RedirectSinPermiso();
            }

            try
            {
                List<Cliente> clientes = new List<Cliente>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    var clientesPorNombre = _listarCliente.BuscarPorNombre(buscar);
                    var clientesPorCedula = _listarCliente.BuscarPorCedula(buscar);
                    var clientesPorCorreo = _listarCliente.BuscarPorCorreo(buscar);

                    clientes = clientesPorNombre
                        .Union(clientesPorCedula)
                        .Union(clientesPorCorreo)
                        .ToList();

                    ViewBag.Buscar = buscar;
                }
                else
                {
                    clientes = _listarCliente.Obtener();
                }

                return View(clientes);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al cargar clientes: {ex.Message}";
                return View(new List<Cliente>());
            }
        }

        [HttpGet]
        public IActionResult CrearCliente()
        {
            // Admin y Ventas pueden crear clientes
            if (!PuedeGestionarClientes())
            {
                return RedirectSinPermiso();
            }

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCliente(Cliente cliente)
        {
            // Admin y Ventas pueden crear clientes
            if (!PuedeGestionarClientes())
            {
                return RedirectSinPermiso();
            }

            try
            {
                // Validación manual para creación
                if (string.IsNullOrWhiteSpace(cliente.Contrasenna))
                {
                    ModelState.AddModelError("Contrasenna", "La contraseña es obligatoria");
                }

                if (ModelState.IsValid)
                {
                    var clienteExistentePorCorreo = _listarCliente.BuscarPorCorreo(cliente.Correo);
                    if (clienteExistentePorCorreo != null && clienteExistentePorCorreo.Count > 0)
                    {
                        ModelState.AddModelError("Correo", "El correo electrónico ya está registrado");
                        return View(cliente);
                    }

                    var clienteExistentePorCedula = _listarCliente.BuscarPorCedula(cliente.Cedula);
                    if (clienteExistentePorCedula != null && clienteExistentePorCedula.Count > 0)
                    {
                        ModelState.AddModelError("Cedula", "La cédula ya está registrada");
                        return View(cliente);
                    }

                    cliente.Estado = true;

                    int resultado = await _crearCliente.Guardar(cliente);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Cliente creado exitosamente";
                        return RedirectToAction(nameof(ListadoClientes));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo crear el cliente en la base de datos");
                    }
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(cliente);
            }
        }

        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
            // Admin y Ventas pueden editar clientes
            if (!PuedeGestionarClientes())
            {
                return RedirectSinPermiso();
            }

            try
            {
                var cliente = _obtenerClientePorId.Obtener(id);

                if (cliente == null)
                {
                    TempData["Error"] = "Cliente no encontrado";
                    return RedirectToAction(nameof(ListadoClientes));
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al cargar el cliente: " + ex.Message;
                return RedirectToAction(nameof(ListadoClientes));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(Cliente cliente)
        {
            // Admin y Ventas pueden editar clientes
            if (!PuedeGestionarClientes())
            {
                return RedirectSinPermiso();
            }

            try
            {
                if (ModelState.IsValid)
                {
                    var clienteExistentePorCorreo = _listarCliente.BuscarPorCorreo(cliente.Correo);
                    if (clienteExistentePorCorreo != null &&
                        clienteExistentePorCorreo.Any(c => c.IdCliente != cliente.IdCliente))
                    {
                        ModelState.AddModelError("Correo", "El correo electrónico ya está registrado por otro cliente");
                        return View(cliente);
                    }

                    var clienteExistentePorCedula = _listarCliente.BuscarPorCedula(cliente.Cedula);
                    if (clienteExistentePorCedula != null &&
                        clienteExistentePorCedula.Any(c => c.IdCliente != cliente.IdCliente))
                    {
                        ModelState.AddModelError("Cedula", "La cédula ya está registrada por otro cliente");
                        return View(cliente);
                    }

                    int resultado = _actualizarCliente.Actualizar(cliente);

                    if (resultado > 0)
                    {
                        TempData["Success"] = "Cliente actualizado exitosamente";
                        return RedirectToAction(nameof(ListadoClientes));
                    }
                    else
                    {
                        ModelState.AddModelError("", "No se pudo actualizar el cliente");
                    }
                }

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "Error al actualizar el cliente: " + ex.Message);
                return View(cliente);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCliente(int IdCliente)
        {
            // Solo Admin puede eliminar clientes
            if (!EsAdministrador())
            {
                TempData["Error"] = "Solo administradores pueden eliminar clientes";
                return RedirectToAction(nameof(ListadoClientes));
            }

            try
            {
                int resultado = _eliminarCliente.Eliminar(IdCliente);

                if (resultado > 0)
                {
                    TempData["Success"] = "Cliente eliminado exitosamente";
                }
                else
                {
                    TempData["Error"] = "No se pudo eliminar el cliente";
                }

                return RedirectToAction(nameof(ListadoClientes));
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Error al eliminar el cliente: " + ex.Message;
                return RedirectToAction(nameof(ListadoClientes));
            }
        }
    }
}