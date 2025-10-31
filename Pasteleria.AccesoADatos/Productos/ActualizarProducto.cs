using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;

namespace Pasteleria.AccesoADatos.Productos
{
    public class ActualizarProducto : IActualizarProducto
    {
        private Contexto _contexto;

        public ActualizarProducto()
        {
            _contexto = new Contexto();
        }

        public int Actualizar(Abstracciones.ModeloUI.Producto elProducto)
        {
            ProductoAD elProductoActualizar = ConvertirObjetoParaAD(elProducto);

            _contexto.Producto.Update(elProductoActualizar);

            int cantidadDeDatosActualizados = _contexto.SaveChanges();
            return cantidadDeDatosActualizados;
        }

        private ProductoAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Producto producto)
        {
            return new ProductoAD
            {
                IdProducto = producto.IdProducto,
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