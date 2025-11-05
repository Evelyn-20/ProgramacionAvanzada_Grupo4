using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.AccesoADatos.Modelos;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Categorias
{
    public class ListarCategorias : IListarCategorias
    {
        private Contexto _contexto;

        public ListarCategorias()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Categoria> Obtener()
        {
            List<CategoriaAD> categoriasAD = _contexto.Categoria.ToList();
            return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
        }

        public List<Abstracciones.ModeloUI.Categoria> BuscarPorNombre(string nombre)
        {
            List<CategoriaAD> categoriasAD = _contexto.Categoria
                .Where(c => c.NombreCategoria.Contains(nombre))
                .ToList();
            return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
        }

        public List<Abstracciones.ModeloUI.Categoria> ObtenerActivas()
        {
            List<CategoriaAD> categoriasAD = _contexto.Categoria
                .Where(c => c.Estado == true)
                .ToList();
            return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
        }

        private Abstracciones.ModeloUI.Categoria ConvertirObjetoParaUI(CategoriaAD categoriaAD)
        {
            return new Abstracciones.ModeloUI.Categoria
            {
                IdCategoria = categoriaAD.IdCategoria,
                NombreCategoria = categoriaAD.NombreCategoria,
                Imagen = categoriaAD.Imagen,
                Estado = categoriaAD.Estado
            };
        }
    }
}