namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Venta
    {
        public int IdVenta { get; set; }
        public int IdPedido { get; set; }
        public int IdCliente { get; set; }
        public int IdUsuario { get; set; }
        public DateTime FechaVenta { get; set; }

        public decimal Subtotal { get; set; }
        public decimal Impuesto { get; set; }
        public decimal Total { get; set; }
        public string MetodoPago { get; set; }

        // Datos para UI
        public string NombreCliente { get; set; }
        public string NombreUsuario { get; set; }
    }
}
