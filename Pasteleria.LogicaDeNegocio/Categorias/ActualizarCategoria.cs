using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Categorias
{
    public class ActualizarCategoria : IActualizarCategoria
    {
        private IActualizarCategoria _actualizarCategoria;

        public ActualizarCategoria()
        {
            _actualizarCategoria = new AccesoADatos.Categorias.ActualizarCategoria();
        }

        public int Actualizar(Categoria categoria)
        {
            int resultado = _actualizarCategoria.Actualizar(categoria);
            return resultado;
        }
    }
}