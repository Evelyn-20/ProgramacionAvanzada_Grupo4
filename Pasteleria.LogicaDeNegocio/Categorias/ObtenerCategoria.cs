using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Categorias
{
    public class ObtenerCategoria : IObtenerCategoria
    {
        private IObtenerCategoria _obtenerCategoria;

        public ObtenerCategoria()
        {
            _obtenerCategoria = new AccesoADatos.Categorias.ObtenerCategoria();
        }

        public Categoria Obtener(int id)
        {
            Categoria laCategoria = _obtenerCategoria.Obtener(id);
            return laCategoria;
        }
    }
}