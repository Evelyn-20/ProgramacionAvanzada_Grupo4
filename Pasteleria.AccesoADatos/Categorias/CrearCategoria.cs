using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using System;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Categorias
{
    public class CrearCategoria : ICrearCategoria
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public CrearCategoria()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Categoria laCategoria)
        {
            CategoriaAD laCategoriaAGuardar = ConvertirObjetoParaAD(laCategoria);

            _contexto.Categoria.Add(laCategoriaAGuardar);

            int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

            // Registrar en auditoría
            if (cantidadDeDatosAgregados > 0)
            {
                _auditoria.RegistrarCreacion("Categoria", laCategoriaAGuardar.IdCategoria, new
                {
                    laCategoriaAGuardar.IdCategoria,
                    laCategoriaAGuardar.NombreCategoria,
                    laCategoriaAGuardar.Estado
                });
            }

            return cantidadDeDatosAgregados;
        }

        private CategoriaAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Categoria categoria)
        {
            return new CategoriaAD
            {
                NombreCategoria = categoria.NombreCategoria,
                Imagen = categoria.Imagen ?? new byte[0],
                Estado = categoria.Estado
            };
        }
    }
}