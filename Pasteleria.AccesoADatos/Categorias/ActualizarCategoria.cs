using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Categorias
{
    public class ActualizarCategoria : IActualizarCategoria
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public ActualizarCategoria()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Actualizar(Abstracciones.ModeloUI.Categoria laCategoria)
        {
            try
            {
                // Obtener la categoría existente
                var categoriaExistente = _contexto.Categoria
                    .FirstOrDefault(c => c.IdCategoria == laCategoria.IdCategoria);

                if (categoriaExistente == null)
                {
                    return 0;
                }

                // Guardar valores anteriores para auditoría
                var valoresAnteriores = new
                {
                    categoriaExistente.NombreCategoria,
                    categoriaExistente.Estado
                };

                // Actualizar campos
                categoriaExistente.NombreCategoria = laCategoria.NombreCategoria;
                categoriaExistente.Estado = laCategoria.Estado;

                // Solo actualizar imagen si se proporcionó una nueva
                if (laCategoria.Imagen != null && laCategoria.Imagen.Length > 0)
                {
                    categoriaExistente.Imagen = laCategoria.Imagen;
                }

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosActualizados > 0)
                {
                    _auditoria.RegistrarActualizacion("Categoria", laCategoria.IdCategoria,
                        valoresAnteriores,
                        new
                        {
                            categoriaExistente.NombreCategoria,
                            categoriaExistente.Estado
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