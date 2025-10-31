using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class ActualizarCliente : IActualizarCliente
    {
        private Contexto _contexto;

        public ActualizarCliente()
        {
            _contexto = new Contexto();
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
                    throw new Exception("El cliente no existe");
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

                System.Diagnostics.Debug.WriteLine($"Cliente actualizado exitosamente. ID: {elCliente.IdCliente}");

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar cliente: {ex.Message}");
                throw new Exception("Error al actualizar el cliente en la base de datos", ex);
            }
        }
    }
}