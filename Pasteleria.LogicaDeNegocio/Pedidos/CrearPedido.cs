using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Pedidos
{
    public class CrearPedido : ICrearPedido
    {
        private ICrearPedido _crearPedido;

        public CrearPedido()
        {
            _crearPedido = new AccesoADatos.Pedidos.CrearPedido();
        }

        public async Task<int> Guardar(Pedido pedido, List<DetallePedido> detalles)
        {
            // Validaciones de negocio
            if (pedido == null)
                throw new ArgumentException("El pedido no puede ser nulo");

            if (detalles == null || detalles.Count == 0)
                throw new ArgumentException("El pedido debe tener al menos un producto");

            if (pedido.Total <= 0)
                throw new ArgumentException("El total del pedido debe ser mayor a cero");

            return await _crearPedido.Guardar(pedido, detalles);
        }
    }
}
