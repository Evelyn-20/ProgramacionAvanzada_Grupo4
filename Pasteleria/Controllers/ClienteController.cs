using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pasteleria.Models;
using Pasteleria.Abstracciones.ModeloUI;
using X.PagedList;

namespace Pasteleria.Controllers
{
    public class ClienteController : Controller
    {
        // Inyección del contexto de base de datos
        private readonly PasteleriaDbContext _db;
        public ClienteController(PasteleriaDbContext db) => _db = db;

        // LISTADO con filtros y paginación
        public async Task<IActionResult> ListadoClientes([FromQuery] ClienteFiltroVM f)
        {
            // Query base de clientes sin rastreo de cambios (mejor rendimiento para lectura)
            var q = _db.Clientes.AsNoTracking();

            // Aplicar filtro de búsqueda si existe
            if (!string.IsNullOrWhiteSpace(f.Buscar))
            {
                var filtro = f.Buscar.ToLower().Trim();
                q = q.Where(c => c.NombreCliente.ToLower().Contains(filtro) ||
                                 c.Cedula.Contains(filtro) ||
                                 c.Correo.ToLower().Contains(filtro) ||
                                 c.Telefono.Contains(filtro));
            }

            // Ordenar alfabéticamente por nombre
            q = q.OrderBy(c => c.NombreCliente);

            // Configurar paginación (10 clientes por página)
            var page = f.Pagina <= 0 ? 1 : f.Pagina;
            var paged = await q.ToPagedListAsync(page, 10);

            // Pasar el filtro a la vista para mantener los valores
            ViewBag.Filtro = f;
            return View("ListadoClientes", paged);
        }

        // GET: formulario Crear
        public IActionResult CrearCliente()
        {
            var vm = new ClienteFormVM();
            return View("CrearCliente", vm);
        }

        // POST: crear cliente
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> CrearCliente(ClienteFormVM vm)
        {
            // Validar que el modelo sea válido
            if (!ModelState.IsValid)
                return View("CrearCliente", vm);

            // Validar que la contraseña sea obligatoria al crear
            if (string.IsNullOrWhiteSpace(vm.Contrasenna))
            {
                ModelState.AddModelError(nameof(vm.Contrasenna), "La contraseña es obligatoria al crear un cliente");
                return View("CrearCliente", vm);
            }

            // Verificar si la cédula ya existe
            if (await _db.Clientes.AnyAsync(c => c.Cedula == vm.Cedula))
            {
                ModelState.AddModelError(nameof(vm.Cedula), "Ya existe un cliente con esta cédula");
                return View("CrearCliente", vm);
            }

            // Crear el nuevo cliente
            var c = new Cliente
            {
                NombreCliente = vm.NombreCliente.Trim(),
                Cedula = vm.Cedula.Trim(),
                Correo = vm.Correo.Trim(),
                Telefono = vm.Telefono.Trim(),
                Direccion = vm.Direccion.Trim(),
                Contrasenna = vm.Contrasenna, // TODO: Implementar hash en producción
                Estado = true // Nuevo cliente siempre activo
            };

            _db.Add(c);
            await _db.SaveChangesAsync();
            TempData["ok"] = "Cliente creado exitosamente";
            return RedirectToAction(nameof(ListadoClientes));
        }

        // GET: formulario Editar
        public async Task<IActionResult> EditarCliente(int id)
        {
            // Buscar el cliente por ID
            var c = await _db.Clientes.FindAsync(id);
            if (c == null) return RedirectToAction(nameof(ListadoClientes));

            // Mapear a ViewModel para el formulario
            var vm = new ClienteFormVM
            {
                IdCliente = c.IdCliente,
                NombreCliente = c.NombreCliente,
                Cedula = c.Cedula,
                Correo = c.Correo,
                Telefono = c.Telefono,
                Direccion = c.Direccion,
                Estado = c.Estado
                // No pasamos la contraseña por seguridad
            };
            return View("EditarCliente", vm);
        }

        // POST: guardar edición
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EditarCliente(ClienteFormVM vm)
        {
            // Remover validación de contraseña (es opcional al editar)
            ModelState.Remove(nameof(vm.Contrasenna));

            if (!ModelState.IsValid) return View("EditarCliente", vm);

            // Buscar el cliente a actualizar
            var c = await _db.Clientes.FindAsync(vm.IdCliente);
            if (c == null) return RedirectToAction(nameof(ListadoClientes));

            // Verificar si la cédula ya existe en otro cliente
            if (await _db.Clientes.AnyAsync(x => x.Cedula == vm.Cedula && x.IdCliente != vm.IdCliente))
            {
                ModelState.AddModelError(nameof(vm.Cedula), "Ya existe otro cliente con esta cédula");
                return View("EditarCliente", vm);
            }

            // Actualizar los datos del cliente
            c.NombreCliente = vm.NombreCliente.Trim();
            c.Cedula = vm.Cedula.Trim();
            c.Correo = vm.Correo.Trim();
            c.Telefono = vm.Telefono.Trim();
            c.Direccion = vm.Direccion.Trim();
            c.Estado = vm.Estado;

            // Solo actualizar contraseña si se proporciona una nueva
            if (!string.IsNullOrWhiteSpace(vm.Contrasenna))
            {
                c.Contrasenna = vm.Contrasenna; // TODO: Hashear contraseña
            }

            await _db.SaveChangesAsync();
            TempData["ok"] = "Cliente actualizado exitosamente";
            return RedirectToAction(nameof(ListadoClientes));
        }

        // POST: eliminar cliente
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> EliminarCliente(int IdCliente)
        {
            var c = await _db.Clientes.FindAsync(IdCliente);
            if (c != null)
            {
                _db.Clientes.Remove(c);
                await _db.SaveChangesAsync();
                TempData["ok"] = "Cliente eliminado exitosamente";
            }
            return RedirectToAction(nameof(ListadoClientes));
        }
    }
}