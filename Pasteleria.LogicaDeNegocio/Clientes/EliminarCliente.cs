using System;
using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.Logica.Auditoria;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class EliminarCliente : IEliminarCliente
    {
        private IEliminarCliente _eliminarCliente;
        private IObtenerCliente _obtenerCliente;
        private IRegistrarAuditoria _registrarAuditoria;

        public EliminarCliente()
        {
            _eliminarCliente = new AccesoADatos.Clientes.EliminarCliente();
            _obtenerCliente = new AccesoADatos.Clientes.ObtenerCliente();
            _registrarAuditoria = new Auditoria.RegistrarAuditoria();
        }

        public int Eliminar(int idCliente)
        {
            // Validaciones de negocio
            if (idCliente <= 0)
            {
                throw new ArgumentException("El ID del cliente es inválido");
            }

            // Verificar que el cliente existe y obtener sus datos para auditoría
            var clienteExistente = _obtenerCliente.Obtener(idCliente);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente no existe");
            }

            // Guardar valores anteriores para auditoría (sin contraseña)
            var valoresAnteriores = new
            {
                clienteExistente.IdCliente,
                clienteExistente.NombreCliente,
                clienteExistente.Cedula,
                clienteExistente.Correo,
                clienteExistente.Telefono,
                clienteExistente.Direccion,
                clienteExistente.Estado
            };

            try
            {
                int resultado = _eliminarCliente.Eliminar(idCliente);

                if (resultado == 0)
                {
                    throw new Exception("No se pudo eliminar el cliente");
                }

                // Registrar auditoría de eliminación
                _registrarAuditoria.RegistrarEliminacion(
                    tabla: "Cliente",
                    idRegistro: idCliente,
                    valoresAnteriores: valoresAnteriores,
                    usuarioNombre: "Sistema"
                );

                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error al eliminar el cliente: {ex.Message}", ex);
            }
        }
    }
}