using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Productos
{
    public class ObtenerProducto : IObtenerProducto
    {
        private IObtenerProducto _obtenerProducto;

        public ObtenerProducto()
        {
            _obtenerProducto = new AccesoADatos.Productos.ObtenerProducto();
        }

        public Producto Obtener(int id)
        {
            Producto elProducto = _obtenerProducto.Obtener(id);
            return elProducto;
        }
    }
}