using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class CrearCliente : ICrearCliente
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public CrearCliente()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Cliente elCliente)
        {
            try
            {
                // Validar que no exista un cliente con la misma cédula
                bool cedulaExiste = await _contexto.Cliente.AnyAsync(c => c.Cedula == elCliente.Cedula);
                if (cedulaExiste)
                {
                    throw new Exception("Ya existe un cliente registrado con esa cédula");
                }

                // Validar que no exista un cliente con el mismo correo
                bool correoExiste = await _contexto.Cliente.AnyAsync(c => c.Correo == elCliente.Correo);
                if (correoExiste)
                {
                    throw new Exception("Ya existe un cliente registrado con ese correo electrónico");
                }

                ClienteAD elClienteAGuardar = ConvertirObjetoParaAD(elCliente);
                _contexto.Cliente.Add(elClienteAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                // Registrar en auditoría
                if (cantidadDeDatosAgregados > 0)
                {
                    _auditoria.RegistrarCreacion("Cliente", elClienteAGuardar.IdCliente, new
                    {
                        elClienteAGuardar.IdCliente,
                        elClienteAGuardar.NombreCliente,
                        elClienteAGuardar.Cedula,
                        elClienteAGuardar.Correo,
                        elClienteAGuardar.Telefono,
                        elClienteAGuardar.Direccion,
                        elClienteAGuardar.Estado
                        // No incluir contraseña por seguridad
                    });
                }

                System.Diagnostics.Debug.WriteLine($"Cliente guardado exitosamente. ID: {elClienteAGuardar.IdCliente}");
                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar cliente: {ex.Message}");
                throw;
            }
        }

        private ClienteAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Cliente cliente)
        {
            return new ClienteAD
            {
                NombreCliente = cliente.NombreCliente?.Trim(),
                Cedula = cliente.Cedula?.Trim(),
                Correo = cliente.Correo?.Trim().ToLower(),
                Telefono = cliente.Telefono?.Trim(),
                Direccion = cliente.Direccion?.Trim(),
                Contrasenna = cliente.Contrasenna,
                Estado = cliente.Estado
            };
        }
    }
}