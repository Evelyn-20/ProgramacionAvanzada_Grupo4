using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.LogicaDeNegocio.Productos;

namespace Pasteleria.LogicaDeNegocio.Productos
{
    public class CrearProducto : ICrearProducto
    {
        private ICrearProducto _crearProducto;

        public CrearProducto()
        {
            _crearProducto = new AccesoADatos.Productos.CrearProducto();
        }

        public async Task<int> Guardar(Producto elProducto)
        {
            // Establece el estado como activo por defecto
            elProducto.Estado = true;

            int cantidadDeResultados = await _crearProducto.Guardar(elProducto);
            return cantidadDeResultados;
        }
    }
}