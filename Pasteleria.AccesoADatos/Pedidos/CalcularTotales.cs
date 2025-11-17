using System;
using System.Collections.Generic;
using System.Linq;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class CalcularTotales : ICalcularTotales
    {
        public ResumenCompra CalcularResumen(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
            {
                return new ResumenCompra
                {
                    Items = new List<CarritoItem>(),
                    Subtotal = 0,
                    Descuento = 0,
                    Impuesto = 0,
                    Total = 0
                };
            }

            var resumen = new ResumenCompra
            {
                Items = items,
                Subtotal = CalcularSubtotal(items),
                Descuento = CalcularDescuentoTotal(items),
                Impuesto = CalcularImpuestos(items),
            };

            resumen.Total = CalcularTotal(items);

            return resumen;
        }

        public decimal CalcularSubtotal(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            return items.Sum(item => item.Precio * item.Cantidad);
        }

        public decimal CalcularDescuentoTotal(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            return items.Sum(item => item.Descuento);
        }

        public decimal CalcularImpuestos(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            // Calcular impuesto usando el porcentaje de cada producto
            return items.Sum(item =>
            {
                decimal subtotalItem = (item.Precio * item.Cantidad) - item.Descuento;
                return subtotalItem * (item.PorcentajeImpuesto / 100m);
            });
        }

        public decimal CalcularTotal(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            decimal subtotal = CalcularSubtotal(items);
            decimal descuento = CalcularDescuentoTotal(items);
            decimal impuesto = CalcularImpuestos(items);

            return subtotal - descuento + impuesto;
        }
    }
}