using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class ObtenerPedido : IObtenerPedido
    {
        private IObtenerPedido _obtenerPedido;

        public ObtenerPedido()
        {
            _obtenerPedido = new AccesoADatos.Pedidos.ObtenerPedido();
        }

        public Pedido Obtener(int idPedido)
        {
            return _obtenerPedido.Obtener(idPedido);
        }

        public List<DetallePedido> ObtenerDetalles(int idPedido)
        {
            return _obtenerPedido.ObtenerDetalles(idPedido);
        }
    }
}