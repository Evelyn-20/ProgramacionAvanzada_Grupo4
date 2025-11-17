using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PedidoUI = Pasteleria.Abstracciones.ModeloUI.Pedido;
using DetallePedidoUI = Pasteleria.Abstracciones.ModeloUI.DetallePedido;
using EstadoPedidoUI = Pasteleria.Abstracciones.ModeloUI.EstadoPedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Pedido
{
    public interface IBuscarProductosParaPedido
    {
        List<Abstracciones.ModeloUI.Producto> BuscarPorTermino(string termino);
        List<Abstracciones.ModeloUI.Producto> BuscarActivos();
        Abstracciones.ModeloUI.Producto ObtenerPorId(int idProducto);
    }
}