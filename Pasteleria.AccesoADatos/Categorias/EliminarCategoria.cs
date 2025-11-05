using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using System.Linq;

namespace Pasteleria.AccesoADatos.Categorias
{
    public class EliminarCategoria : IEliminarCategoria
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public EliminarCategoria()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Eliminar(int idCategoria)
        {
            CategoriaAD categoriaAEliminar = _contexto.Categoria
                .FirstOrDefault(c => c.IdCategoria == idCategoria);

            if (categoriaAEliminar != null)
            {
                // Guardar información antes de eliminar para auditoría
                var infoCategoria = new
                {
                    categoriaAEliminar.IdCategoria,
                    categoriaAEliminar.NombreCategoria,
                    categoriaAEliminar.Estado
                };

                _contexto.Categoria.Remove(categoriaAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosEliminados > 0)
                {
                    _auditoria.RegistrarEliminacion("Categoria", idCategoria, infoCategoria);
                }

                return cantidadDeDatosEliminados;
            }

            return 0;
        }
    }
}