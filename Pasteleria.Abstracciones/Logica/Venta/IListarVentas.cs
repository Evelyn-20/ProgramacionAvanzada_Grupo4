using System;
using System.Collections.Generic;
using Pasteleria.Abstracciones.ModeloUI;
using VentaUI = Pasteleria.Abstracciones.ModeloUI.Venta;

namespace Pasteleria.Abstracciones.Logica.Venta
{
    public interface IListarVentas
    {
        List<VentaUI> Obtener();
        List<VentaUI> ObtenerPorFecha(DateTime inicio, DateTime fin);
        List<VentaUI> ObtenerPorCliente(int idCliente);
        List<VentaUI> ObtenerPorUsuario(int idUsuario);
    }
}
