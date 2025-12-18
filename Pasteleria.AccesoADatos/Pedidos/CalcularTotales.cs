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
                Subtotal = CalcularSubtotal(items),      // Subtotal NETO (después de descuentos)
                Descuento = CalcularDescuentoTotal(items), // Total de descuentos aplicados
                Impuesto = CalcularImpuestos(items),      // Impuestos sobre subtotal neto
            };

            resumen.Total = CalcularTotal(items);

            return resumen;
        }

        // Calcula el subtotal NETO (precio bruto - descuentos)
        // Este es el monto sobre el cual se calculan los impuestos
        public decimal CalcularSubtotal(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            // El subtotal es la suma de los items.Subtotal
            // que ya tienen el descuento aplicado
            return items.Sum(item => item.Subtotal);
        }

        // Suma total de todos los descuentos aplicados
        public decimal CalcularDescuentoTotal(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            return items.Sum(item => item.Descuento);
        }

        // Calcula impuestos sobre el subtotal NETO (después de descuentos)
        // Fórmula: Subtotal_Neto × (PorcentajeImpuesto / 100)
        public decimal CalcularImpuestos(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            return items.Sum(item =>
            {
                // item.Subtotal ya tiene el descuento aplicado
                return item.Subtotal * (item.PorcentajeImpuesto / 100m);
            });
        }

        // Calcula el total final: Subtotal_Neto + Impuestos
        public decimal CalcularTotal(List<CarritoItem> items)
        {
            if (items == null || !items.Any())
                return 0;

            decimal subtotal = CalcularSubtotal(items);  // Ya con descuento aplicado
            decimal impuesto = CalcularImpuestos(items); // Sobre el subtotal neto

            return subtotal + impuesto;
        }
    }
}