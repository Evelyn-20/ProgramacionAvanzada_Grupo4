using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class GestionarEstadosPedido : IGestionarEstadosPedido
    {
        private IGestionarEstadosPedido _gestionarEstados;

        public GestionarEstadosPedido()
        {
            _gestionarEstados = new AccesoADatos.Pedidos.GestionarEstadosPedido();
        }

        public List<EstadoPedido> ObtenerEstados()
        {
            return _gestionarEstados.ObtenerEstados();
        }

        public EstadoPedido ObtenerEstado(int idEstado)
        {
            return _gestionarEstados.ObtenerEstado(idEstado);
        }
    }
}