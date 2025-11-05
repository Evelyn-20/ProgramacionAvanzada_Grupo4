using System.Collections.Generic;
using System.Threading.Tasks;
using CategoriaUI = Pasteleria.Abstracciones.ModeloUI.Categoria;

namespace Pasteleria.Abstracciones.Logica.Categoria
{
    public interface IActualizarCategoria
    {
        int Actualizar(CategoriaUI categoria);
    }
}