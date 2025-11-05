using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class ActualizarCliente : IActualizarCliente
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public ActualizarCliente()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Actualizar(Abstracciones.ModeloUI.Cliente elCliente)
        {
            try
            {
                // Verificar que el cliente existe
                var clienteExistente = _contexto.Cliente
                    .FirstOrDefault(c => c.IdCliente == elCliente.IdCliente);

                if (clienteExistente == null)
                {
                    return 0;
                }

                // Validar que la cédula no esté duplicada (excepto para el mismo cliente)
                bool cedulaDuplicada = _contexto.Cliente
                    .Any(c => c.Cedula == elCliente.Cedula && c.IdCliente != elCliente.IdCliente);
                if (cedulaDuplicada)
                {
                    throw new Exception("Ya existe otro cliente con esa cédula");
                }

                // Validar que el correo no esté duplicado (excepto para el mismo cliente)
                bool correoDuplicado = _contexto.Cliente
                    .Any(c => c.Correo == elCliente.Correo && c.IdCliente != elCliente.IdCliente);
                if (correoDuplicado)
                {
                    throw new Exception("Ya existe otro cliente con ese correo electrónico");
                }

                // Guardar valores anteriores para auditoría (sin contraseña por seguridad)
                var valoresAnteriores = new
                {
                    clienteExistente.NombreCliente,
                    clienteExistente.Cedula,
                    clienteExistente.Correo,
                    clienteExistente.Telefono,
                    clienteExistente.Direccion,
                    clienteExistente.Estado
                };

                // Actualizar los valores del cliente existente
                clienteExistente.NombreCliente = elCliente.NombreCliente?.Trim();
                clienteExistente.Cedula = elCliente.Cedula?.Trim();
                clienteExistente.Correo = elCliente.Correo?.Trim().ToLower();
                clienteExistente.Telefono = elCliente.Telefono?.Trim();
                clienteExistente.Direccion = elCliente.Direccion?.Trim();
                clienteExistente.Contrasenna = elCliente.Contrasenna;
                clienteExistente.Estado = elCliente.Estado;

                _contexto.Cliente.Update(clienteExistente);
                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosActualizados > 0)
                {
                    _auditoria.RegistrarActualizacion("Cliente", elCliente.IdCliente,
                        valoresAnteriores,
                        new
                        {
                            clienteExistente.NombreCliente,
                            clienteExistente.Cedula,
                            clienteExistente.Correo,
                            clienteExistente.Telefono,
                            clienteExistente.Direccion,
                            clienteExistente.Estado
                        }
                    );
                }

                System.Diagnostics.Debug.WriteLine($"Cliente actualizado exitosamente. ID: {elCliente.IdCliente}");

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar cliente: {ex.Message}");
                throw;
            }
        }
    }
}