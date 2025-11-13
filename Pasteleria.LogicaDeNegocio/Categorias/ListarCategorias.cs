using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace Pasteleria.LogicaDeNegocio.Categorias
{
    public class ListarCategorias : IListarCategorias
    {
        private IListarCategorias _listarCategorias;

        public ListarCategorias()
        {
            _listarCategorias = new AccesoADatos.Categorias.ListarCategorias();
        }

        public List<Categoria> Obtener()
        {
            return _listarCategorias.Obtener();
        }

        public List<Categoria> BuscarPorNombre(string nombre)
        {
            return _listarCategorias.BuscarPorNombre(nombre);
        }

        public List<Categoria> ObtenerActivas()
        {
            return _listarCategorias.ObtenerActivas();
        }
    }
}