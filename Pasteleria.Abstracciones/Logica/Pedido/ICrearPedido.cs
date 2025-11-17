using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;
using DetallePedidoUI = Pasteleria.Abstracciones.ModeloUI.DetallePedido;
using EstadoPedidoUI = Pasteleria.Abstracciones.ModeloUI.EstadoPedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Pedido
{
    public interface ICrearPedido
    {
        Task<int> Guardar(PedidoUI pedido, List<DetallePedidoUI> detalles);
    }
}