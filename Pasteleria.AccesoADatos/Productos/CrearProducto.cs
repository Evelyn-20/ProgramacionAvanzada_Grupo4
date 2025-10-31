using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Productos
{
    public class CrearProducto : ICrearProducto
    {
        private Contexto _contexto;

        public CrearProducto()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Producto elProducto)
        {
            ProductoAD elProductoAGuardar = ConvertirObjetoParaAD(elProducto);

            _contexto.Producto.Add(elProductoAGuardar);

            int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();
            return cantidadDeDatosAgregados;
        }

        private ProductoAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Producto producto)
        {
            return new ProductoAD
            {
                IdCategoria = producto.IdCategoria,
                NombreProducto = producto.NombreProducto,
                DescripcionProducto = producto.DescripcionProducto,
                Cantidad = producto.Cantidad,
                Precio = producto.Precio,
                PorcentajeImpuesto = producto.PorcentajeImpuesto,
                Imagen = producto.Imagen ?? new byte[0],
                Estado = producto.Estado
            };
        }
    }
}