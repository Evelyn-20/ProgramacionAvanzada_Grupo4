using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductoUI = Pasteleria.Abstracciones.ModeloUI.Producto; // para que no confunda con carpeta

namespace Pasteleria.Abstracciones.Logica.Producto
{
    public interface IObtenerProducto
    {
        ProductoUI Obtener(int id);
    }
}