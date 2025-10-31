using System;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class CrearCliente : ICrearCliente
    {
        private ICrearCliente _crearCliente;

        public CrearCliente()
        {
            _crearCliente = new AccesoADatos.Clientes.CrearCliente();
        }

        public async Task<int> Guardar(Cliente elCliente)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(elCliente.NombreCliente))
            {
                throw new ArgumentException("El nombre del cliente es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(elCliente.Cedula))
            {
                throw new ArgumentException("La cédula es obligatoria");
            }

            // Validar formato de cédula (opcional)
            if (elCliente.Cedula.Length < 9)
            {
                throw new ArgumentException("La cédula debe tener al menos 9 caracteres");
            }

            if (string.IsNullOrWhiteSpace(elCliente.Correo))
            {
                throw new ArgumentException("El correo electrónico es obligatorio");
            }

            // Validar formato de correo
            if (!elCliente.Correo.Contains("@") || !elCliente.Correo.Contains("."))
            {
                throw new ArgumentException("El formato del correo electrónico no es válido");
            }

            if (string.IsNullOrWhiteSpace(elCliente.Telefono))
            {
                throw new ArgumentException("El teléfono es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(elCliente.Contrasenna))
            {
                throw new ArgumentException("La contraseña es obligatoria");
            }

            if (elCliente.Contrasenna.Length < 6)
            {
                throw new ArgumentException("La contraseña debe tener al menos 6 caracteres");
            }

            // Validar complejidad de contraseña (opcional pero recomendado)
            if (!ContieneCaracteresValidos(elCliente.Contrasenna))
            {
                throw new ArgumentException("La contraseña debe contener al menos una letra y un número");
            }

            // Establecer estado como activo por defecto
            elCliente.Estado = true;

            // ENCRIPTAR CONTRASEÑA usando BCrypt
            elCliente.Contrasenna = BCrypt.Net.BCrypt.HashPassword(elCliente.Contrasenna);

            try
            {
                int cantidadDeResultados = await _crearCliente.Guardar(elCliente);

                if (cantidadDeResultados == 0)
                {
                    throw new Exception("No se pudo guardar el cliente en la base de datos");
                }

                return cantidadDeResultados;
            }
            catch (Exception ex)
            {
                // Re-lanzar la excepción para que el controlador la maneje
                throw new Exception($"Error al crear el cliente: {ex.Message}", ex);
            }
        }

        // Método para validar complejidad de contraseña
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
    }
}