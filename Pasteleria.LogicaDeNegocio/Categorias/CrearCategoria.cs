using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.Abstracciones.ModeloUI;
using System.Threading.Tasks;

namespace Pasteleria.LogicaDeNegocio.Categorias
{
    public class CrearCategoria : ICrearCategoria
    {
        private ICrearCategoria _crearCategoria;

        public CrearCategoria()
        {
            _crearCategoria = new AccesoADatos.Categorias.CrearCategoria();
        }

        public async Task<int> Guardar(Categoria laCategoria)
        {
            // Establece el estado como activo por defecto
            laCategoria.Estado = true;

            int cantidadDeResultados = await _crearCategoria.Guardar(laCategoria);
            return cantidadDeResultados;
        }
    }
}