using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public static class EstadoPedido
    {
        public static string pendiente = "Pendiente";
        public static string enProceso = "En Proceso";
        public static string completado = "Completado";
        public static string cancelado = "Cancelado";
        public static string entregado = "Entregado";
    }

    public enum EstadosDePedido
    {
        Pendiente = 1,
        EnProceso = 2,
        Completado = 3,
        Cancelado = 4,
        Entregado = 5
    } 
}