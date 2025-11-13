using Pasteleria.Abstracciones.Logica.Producto;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Productos
{
    public class ActualizarProducto : IActualizarProducto
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public ActualizarProducto()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Actualizar(Abstracciones.ModeloUI.Producto elProducto)
        {
            try
            {
                // Obtener el producto existente
                var productoExistente = _contexto.Producto
                    .FirstOrDefault(p => p.IdProducto == elProducto.IdProducto);

                if (productoExistente == null)
                {
                    return 0;
                }

                // Guardar valores anteriores para auditoría
                var valoresAnteriores = new
                {
                    productoExistente.NombreProducto,
                    productoExistente.Precio,
                    productoExistente.Cantidad,
                    productoExistente.Estado,
                    productoExistente.PorcentajeImpuesto,
                    productoExistente.IdCategoria
                };

                // Actualizar SOLO los campos modificables
                productoExistente.NombreProducto = elProducto.NombreProducto;
                productoExistente.IdCategoria = elProducto.IdCategoria;
                productoExistente.DescripcionProducto = elProducto.DescripcionProducto;
                productoExistente.Cantidad = elProducto.Cantidad;
                productoExistente.Precio = elProducto.Precio;
                productoExistente.PorcentajeImpuesto = elProducto.PorcentajeImpuesto;
                productoExistente.Estado = elProducto.Estado;
                productoExistente.FechaActualizacion = DateTime.Now;

                // Solo actualizar imagen si se proporcionó una nueva
                if (elProducto.Imagen != null && elProducto.Imagen.Length > 0)
                {
                    productoExistente.Imagen = elProducto.Imagen;
                }

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosActualizados > 0)
                {
                    _auditoria.RegistrarActualizacion("Producto", elProducto.IdProducto,
                        valoresAnteriores,
                        new
                        {
                            productoExistente.NombreProducto,
                            productoExistente.Precio,
                            productoExistente.Cantidad,
                            productoExistente.Estado,
                            productoExistente.PorcentajeImpuesto,
                            productoExistente.IdCategoria
                        }
                    );
                }

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}