using System.Collections.Generic;
using System.Linq;
using Pasteleria.Abstracciones.Logica.Pedido;
using Pasteleria.AccesoADatos.Modelos;
using EstadoPedidoUI = Pasteleria.Abstracciones.ModeloUI.EstadoPedido;

namespace Pasteleria.AccesoADatos.Pedidos
{
    public class GestionarEstadosPedido : IGestionarEstadosPedido
    {
        private Contexto _contexto;

        public GestionarEstadosPedido()
        {
            _contexto = new Contexto();
        }

        public List<EstadoPedidoUI> ObtenerEstados()
        {
            var estadosAD = _contexto.EstadoPedido
                .Where(e => e.Estado)
                .OrderBy(e => e.NombreEstado)
                .ToList();

            return estadosAD.Select(e => new EstadoPedidoUI
            {
                IdEstadoPedido = e.IdEstadoPedido,
                NombreEstado = e.NombreEstado,
                Descripcion = e.Descripcion,
                Estado = e.Estado
            }).ToList();
        }

        public EstadoPedidoUI ObtenerEstado(int idEstado)
        {
            var estadoAD = _contexto.EstadoPedido
                .FirstOrDefault(e => e.IdEstadoPedido == idEstado);

            if (estadoAD == null)
                return null;

            return new EstadoPedidoUI
            {
                IdEstadoPedido = estadoAD.IdEstadoPedido,
                NombreEstado = estadoAD.NombreEstado,
                Descripcion = estadoAD.Descripcion,
                Estado = estadoAD.Estado
            };
        }
    }
}