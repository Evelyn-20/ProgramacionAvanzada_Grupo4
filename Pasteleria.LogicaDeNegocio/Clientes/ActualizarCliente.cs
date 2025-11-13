using System;
using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class ActualizarCliente : IActualizarCliente
    {
        private IActualizarCliente _actualizarCliente;
        private IObtenerCliente _obtenerCliente;

        public ActualizarCliente()
        {
            _actualizarCliente = new AccesoADatos.Clientes.ActualizarCliente();
            _obtenerCliente = new AccesoADatos.Clientes.ObtenerCliente();
        }

        public int Actualizar(Cliente cliente)
        {
            // Validaciones de negocio
            if (cliente.IdCliente <= 0)
            {
                throw new ArgumentException("El ID del cliente es inválido");
            }

            if (string.IsNullOrWhiteSpace(cliente.NombreCliente))
            {
                throw new ArgumentException("El nombre del cliente es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(cliente.Cedula))
            {
                throw new ArgumentException("La cédula es obligatoria");
            }

            if (cliente.Cedula.Length < 9)
            {
                throw new ArgumentException("La cédula debe tener al menos 9 caracteres");
            }

            if (string.IsNullOrWhiteSpace(cliente.Correo))
            {
                throw new ArgumentException("El correo electrónico es obligatorio");
            }

            if (!cliente.Correo.Contains("@") || !cliente.Correo.Contains("."))
            {
                throw new ArgumentException("El formato del correo electrónico no es válido");
            }

            if (string.IsNullOrWhiteSpace(cliente.Telefono))
            {
                throw new ArgumentException("El teléfono es obligatorio");
            }

            // Obtener cliente existente para manejar la contraseña
            var clienteExistente = _obtenerCliente.Obtener(cliente.IdCliente);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente no existe en la base de datos");
            }

            // Manejo de contraseña
            if (string.IsNullOrWhiteSpace(cliente.Contrasenna))
            {
                // Si no se proporciona contraseña, mantener la existente
                cliente.Contrasenna = clienteExistente.Contrasenna;
            }
            else
            {
                // Si se proporciona contraseña, validar y encriptar
                if (cliente.Contrasenna.Length < 6)
                {
                    throw new ArgumentException("La contraseña debe tener al menos 6 caracteres");
                }

                if (!ContieneCaracteresValidos(cliente.Contrasenna))
                {
                    throw new ArgumentException("La contraseña debe contener al menos una letra y un número");
                }

                cliente.Contrasenna = BCrypt.Net.BCrypt.HashPassword(cliente.Contrasenna);
            }

            try
            {
                int resultado = _actualizarCliente.Actualizar(cliente);

                if (resultado == 0)
                {
                    throw new Exception("No se pudo actualizar el cliente en la base de datos");
                }

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al actualizar el cliente: {ex.Message}", ex);
            }
        }

        private bool ContieneCaracteresValidos(string password)
        {
            bool tieneLetra = false;
            bool tieneNumero = false;

            foreach (char c in password)
            {
                if (char.IsLetter(c)) tieneLetra = true;
                if (char.IsDigit(c)) tieneNumero = true;
            }

            return tieneLetra && tieneNumero;
        }

        public static bool VerificarContrasena(string contrasenaIngresada, string hashAlmacenado)
        {
            try
            {
                return BCrypt.Net.BCrypt.Verify(contrasenaIngresada, hashAlmacenado);
            }
            catch
            {
                return false;
            }
        }
    }
}