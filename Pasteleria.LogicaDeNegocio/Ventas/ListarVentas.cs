using System;
using System.Collections.Generic;
using Pasteleria.Abstracciones.Logica.Venta;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Ventas
{
    public class ListarVentas : IListarVentas
    {
        private readonly IListarVentas _listarVentas;

        public ListarVentas()
        {
            _listarVentas = new AccesoADatos.Ventas.ListarVentas();
        }

        public List<Venta> Obtener()
        {
            return _listarVentas.Obtener();
        }

        public List<Venta> ObtenerPorFecha(DateTime inicio, DateTime fin)
        {
            return _listarVentas.ObtenerPorFecha(inicio, fin);
        }

        public List<Venta> ObtenerPorCliente(int idCliente)
        {
            return _listarVentas.ObtenerPorCliente(idCliente);
        }

        public List<Venta> ObtenerPorUsuario(int idUsuario)
        {
            return _listarVentas.ObtenerPorUsuario(idUsuario);
        }
    }
}
