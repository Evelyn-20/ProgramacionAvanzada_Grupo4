using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class ListarPedidos : IListarPedidos
    {
        private IListarPedidos _listarPedidos;

        public ListarPedidos()
        {
            _listarPedidos = new AccesoADatos.Pedidos.ListarPedidos();
        }

        public List<Pedido> Obtener()
        {
            return _listarPedidos.Obtener();
        }

        public List<Pedido> ObtenerPorCliente(int idCliente)
        {
            return _listarPedidos.ObtenerPorCliente(idCliente);
        }

        public List<Pedido> ObtenerPorEstado(int idEstado)
        {
            return _listarPedidos.ObtenerPorEstado(idEstado);
        }

        public List<Pedido> ObtenerPorFecha(DateTime fechaInicio, DateTime fechaFin)
        {
            return _listarPedidos.ObtenerPorFecha(fechaInicio, fechaFin);
        }
    }
}