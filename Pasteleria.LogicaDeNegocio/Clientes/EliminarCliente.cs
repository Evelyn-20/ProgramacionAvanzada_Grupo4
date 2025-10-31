using System;
using Pasteleria.Abstracciones.Logica.Cliente;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class EliminarCliente : IEliminarCliente
    {
        private IEliminarCliente _eliminarCliente;
        private IObtenerCliente _obtenerCliente;

        public EliminarCliente()
        {
            _eliminarCliente = new AccesoADatos.Clientes.EliminarCliente();
            _obtenerCliente = new AccesoADatos.Clientes.ObtenerCliente();
        }

        public int Eliminar(int idCliente)
        {
            // Validaciones de negocio
            if (idCliente <= 0)
            {
                throw new ArgumentException("El ID del cliente es inválido");
            }

            // Verificar que el cliente existe
            var clienteExistente = _obtenerCliente.Obtener(idCliente);
            if (clienteExistente == null)
            {
                throw new Exception("El cliente no existe");
            }

            // Aquí podrías agregar validaciones adicionales
            // Por ejemplo: verificar que el cliente no tenga pedidos pendientes
            // if (clienteExistente.TienePedidosPendientes)
            // {
            //     throw new Exception("No se puede eliminar un cliente con pedidos pendientes");
            // }

            int resultado = _eliminarCliente.Eliminar(idCliente);

            if (resultado == 0)
            {
                throw new Exception("No se pudo eliminar el cliente");
            }

            return resultado;
        }
    }
}