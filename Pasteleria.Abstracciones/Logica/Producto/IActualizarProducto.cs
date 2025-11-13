using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProductoUI = Pasteleria.Abstracciones.ModeloUI.Producto;

namespace Pasteleria.Abstracciones.Logica.Producto
{
    public interface IActualizarProducto
    {
        int Actualizar(ProductoUI producto);
    }
}