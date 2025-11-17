using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class ActualizarPedido : IActualizarPedido
    {
        private IActualizarPedido _actualizarPedido;

        public ActualizarPedido()
        {
            _actualizarPedido = new AccesoADatos.Pedidos.ActualizarPedido();
        }

        public int Actualizar(Pedido pedido)
        {
            if (pedido == null)
                throw new ArgumentException("El pedido no puede ser nulo");

            return _actualizarPedido.Actualizar(pedido);
        }

        public int ActualizarEstado(int idPedido, int idEstado)
        {
            if (idPedido <= 0)
                throw new ArgumentException("El ID del pedido debe ser mayor a cero");

            if (idEstado <= 0)
                throw new ArgumentException("El ID del estado debe ser mayor a cero");

            return _actualizarPedido.ActualizarEstado(idPedido, idEstado);
        }
    }
}