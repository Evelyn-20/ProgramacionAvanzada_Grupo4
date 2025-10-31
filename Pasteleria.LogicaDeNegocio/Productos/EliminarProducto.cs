using Pasteleria.Abstracciones.Logica.Producto;

namespace Pasteleria.LogicaDeNegocio.Productos
{
    public class EliminarProducto : IEliminarProducto
    {
        private IEliminarProducto _eliminarProducto;

        public EliminarProducto()
        {
            _eliminarProducto = new AccesoADatos.Productos.EliminarProducto();
        }

        public int Eliminar(int idProducto)
        {
            int resultado = _eliminarProducto.Eliminar(idProducto);
            return resultado;
        }
    }
}