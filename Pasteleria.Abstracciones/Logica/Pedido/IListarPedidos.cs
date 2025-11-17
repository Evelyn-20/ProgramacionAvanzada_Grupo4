using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;
using DetallePedidoUI = Pasteleria.Abstracciones.ModeloUI.DetallePedido;
using EstadoPedidoUI = Pasteleria.Abstracciones.ModeloUI.EstadoPedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Pedido
{
    public interface IListarPedidos
    {
        List<PedidoUI> Obtener();
        List<PedidoUI> ObtenerPorCliente(int idCliente);
        List<PedidoUI> ObtenerPorEstado(int idEstado);
        List<PedidoUI> ObtenerPorFecha(DateTime fechaInicio, DateTime fechaFin);
    }
}