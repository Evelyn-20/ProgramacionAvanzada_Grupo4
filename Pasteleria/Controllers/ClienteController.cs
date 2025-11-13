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
                throw;
            }
        }

        // GET: Cliente/ListadoClientes
        public IActionResult ListadoClientes(string buscar)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                List<Cliente> clientes = new List<Cliente>();

                if (!string.IsNullOrWhiteSpace(buscar))
                {
                    // Buscar por nombre, cédula o correo
                    var clientesPorNombre = _listarCliente.BuscarPorNombre(buscar);
                    var clientesPorCedula = _listarCliente.BuscarPorCedula(buscar);
                    var clientesPorCorreo = _listarCliente.BuscarPorCorreo(buscar);

                    // Combinar resultados y eliminar duplicados
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

        // GET: Cliente/CrearCliente
        [HttpGet]
        public IActionResult CrearCliente()
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            return View();
        }

        // POST: Cliente/CrearCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCliente(Cliente cliente)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                System.Diagnostics.Debug.WriteLine($"Nombre: {cliente?.NombreCliente}");
                System.Diagnostics.Debug.WriteLine($"Cédula: {cliente?.Cedula}");
                System.Diagnostics.Debug.WriteLine($"Correo: {cliente?.Correo}");

                if (ModelState.IsValid)
                {
                    // Verificar si el correo ya existe
                    var clienteExistentePorCorreo = _listarCliente.BuscarPorCorreo(cliente.Correo);
                    if (clienteExistentePorCorreo != null && clienteExistentePorCorreo.Count > 0)
                    {
                        ModelState.AddModelError("Correo", "El correo electrónico ya está registrado");
                        return View(cliente);
                    }

                    // Verificar si la cédula ya existe
                    var clienteExistentePorCedula = _listarCliente.BuscarPorCedula(cliente.Cedula);
                    if (clienteExistentePorCedula != null && clienteExistentePorCedula.Count > 0)
                    {
                        ModelState.AddModelError("Cedula", "La cédula ya está registrada");
                        return View(cliente);
                    }

                    // Establecer estado como activo
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

                return View(cliente);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error: {ex.Message}");
                return View(cliente);
            }
        }

        // GET: Cliente/EditarCliente/5
        [HttpGet]
        public IActionResult EditarCliente(int id)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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

        // POST: Cliente/EditarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditarCliente(Cliente cliente)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

            try
            {
                System.Diagnostics.Debug.WriteLine($"ID: {cliente?.IdCliente}");

                if (ModelState.IsValid)
                {
                    // Verificar si el correo ya existe en otro cliente
                    var clienteExistentePorCorreo = _listarCliente.BuscarPorCorreo(cliente.Correo);
                    if (clienteExistentePorCorreo != null &&
                        clienteExistentePorCorreo.Any(c => c.IdCliente != cliente.IdCliente))
                    {
                        ModelState.AddModelError("Correo", "El correo electrónico ya está registrado por otro cliente");
                        return View(cliente);
                    }

                    // Verificar si la cédula ya existe en otro cliente
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

        // POST: Cliente/EliminarCliente
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EliminarCliente(int IdCliente)
        {
            if (!VerificarPermisosAdministrador())
                return RedirectToAction("Index", "Home");

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