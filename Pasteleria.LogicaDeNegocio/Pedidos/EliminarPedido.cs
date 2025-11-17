using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class EliminarPedido : IEliminarPedido
    {
        private IEliminarPedido _eliminarPedido;

        public EliminarPedido()
        {
            _eliminarPedido = new AccesoADatos.Pedidos.EliminarPedido();
        }

        public int Eliminar(int idPedido)
        {
            if (idPedido <= 0)
                throw new ArgumentException("El ID del pedido debe ser mayor a cero");

            return _eliminarPedido.Eliminar(idPedido);
        }
    }
}