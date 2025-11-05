using System.Collections.Generic;
using System.Threading.Tasks;
using CategoriaUI = Pasteleria.Abstracciones.ModeloUI.Categoria;

namespace Pasteleria.Abstracciones.Logica.Categoria
{
    public interface IListarCategorias
    {
        List<CategoriaUI> Obtener();
        List<CategoriaUI> BuscarPorNombre(string nombre);
        List<CategoriaUI> ObtenerActivas();
    }
}