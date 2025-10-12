using System.Collections.Generic;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Models
{
    public class HomeVM
    {
        public IEnumerable<Producto> Productos { get; set; } = new List<Producto>();
        public IEnumerable<Categoria> Categorias { get; set; } = new List<Categoria>();
    }
}
