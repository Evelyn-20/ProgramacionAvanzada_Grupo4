using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.AccesoADatos.Modelos;
using System.Linq;

namespace Pasteleria.AccesoADatos.Categorias
{
    public class ObtenerCategoria : IObtenerCategoria
    {
        private Contexto _contexto;

        public ObtenerCategoria()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Categoria Obtener(int idCategoria)
        {
            CategoriaAD categoriaAD = _contexto.Categoria
                .FirstOrDefault(c => c.IdCategoria == idCategoria);

            if (categoriaAD == null)
            {
                return null;
            }

            return ConvertirObjetoParaUI(categoriaAD);
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