using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Productos
{
    public class CrearProducto : ICrearProducto
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public CrearProducto()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Producto elProducto)
        {
            // Establecer fechas de creación y actualización
            var fechaActual = DateTime.Now;
            elProducto.FechaCreacion = fechaActual;
            elProducto.FechaActualizacion = fechaActual;

            ProductoAD elProductoAGuardar = ConvertirObjetoParaAD(elProducto);

            _contexto.Producto.Add(elProductoAGuardar);

            int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

            // Registrar en auditoría
            if (cantidadDeDatosAgregados > 0)
            {
                _auditoria.RegistrarCreacion("Producto", elProductoAGuardar.IdProducto, new
                {
                    elProductoAGuardar.IdProducto,
                    elProductoAGuardar.NombreProducto,
                    elProductoAGuardar.IdCategoria,
                    elProductoAGuardar.Precio,
                    elProductoAGuardar.Cantidad,
                    elProductoAGuardar.Estado
                });
            }

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
                Estado = producto.Estado,
                FechaCreacion = producto.FechaCreacion,
                FechaActualizacion = producto.FechaActualizacion
            };
        }
    }
}