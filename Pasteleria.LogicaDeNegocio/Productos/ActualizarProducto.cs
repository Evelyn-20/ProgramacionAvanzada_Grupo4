using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Productos
{
    public class ActualizarProducto : IActualizarProducto
    {
        private IActualizarProducto _actualizarProducto;

        public ActualizarProducto()
        {
            _actualizarProducto = new AccesoADatos.Productos.ActualizarProducto();
        }

        public int Actualizar(Producto producto)
        {
            int resultado = _actualizarProducto.Actualizar(producto);
            return resultado;
        }
    }
}