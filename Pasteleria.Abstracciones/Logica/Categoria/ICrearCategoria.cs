using System.Collections.Generic;
using System.Threading.Tasks;
using CategoriaUI = Pasteleria.Abstracciones.ModeloUI.Categoria;

namespace Pasteleria.Abstracciones.Logica.Categoria
{
    public interface ICrearCategoria
    {
        Task<int> Guardar(CategoriaUI laCategoria);
    }
}