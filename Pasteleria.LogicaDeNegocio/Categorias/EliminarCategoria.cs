using Pasteleria.Abstracciones.Logica.Categoria;

namespace Pasteleria.LogicaDeNegocio.Categorias
{
    public class EliminarCategoria : IEliminarCategoria
    {
        private IEliminarCategoria _eliminarCategoria;

        public EliminarCategoria()
        {
            _eliminarCategoria = new AccesoADatos.Categorias.EliminarCategoria();
        }

        public int Eliminar(int idCategoria)
        {
            int resultado = _eliminarCategoria.Eliminar(idCategoria);
            return resultado;
        }
    }
}