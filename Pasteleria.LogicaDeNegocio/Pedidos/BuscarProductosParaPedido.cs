using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class BuscarProductosParaPedido : IBuscarProductosParaPedido
    {
        private IBuscarProductosParaPedido _buscarProductos;

        public BuscarProductosParaPedido()
        {
            _buscarProductos = new AccesoADatos.Pedidos.BuscarProductosParaPedido();
        }

        public List<Producto> BuscarPorTermino(string termino)
        {
            return _buscarProductos.BuscarPorTermino(termino);
        }

        public List<Producto> BuscarActivos()
        {
            return _buscarProductos.BuscarActivos();
        }

        public Producto ObtenerPorId(int idProducto)
        {
            return _buscarProductos.ObtenerPorId(idProducto);
        }
    }
}