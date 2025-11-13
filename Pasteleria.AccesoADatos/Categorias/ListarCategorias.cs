using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Categoria;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Categorias
{
    public class ListarCategorias : IListarCategorias
    {
        private Contexto _contexto;

        public ListarCategorias()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Categoria> Obtener()
        {
            try
            {
                List<CategoriaAD> categoriasAD = _contexto.Categoria
                    .AsNoTracking() // Mejora el rendimiento
                    .ToList();
                return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener categorías: {ex.Message}");
                throw;
            }
        }

        public List<Abstracciones.ModeloUI.Categoria> BuscarPorNombre(string nombre)
        {
            try
            {
                List<CategoriaAD> categoriasAD = _contexto.Categoria
                    .AsNoTracking()
                    .Where(c => c.NombreCategoria.Contains(nombre))
                    .ToList();
                return categoriasAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar categorías: {ex.Message}");
                throw;
            }
        }

        public List<Abstracciones.ModeloUI.Categoria> ObtenerActivas()
        {
            try
            {
                // Obtener todas primero y filtrar en memoria para evitar timeout
                List<CategoriaAD> categoriasAD = _contexto.Categoria
                    .AsNoTracking()
                    .ToList();

                // Filtrar las activas en memoria
                return categoriasAD
                    .Where(c => c.Estado)
                    .Select(c => ConvertirObjetoParaUI(c))
                    .ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener categorías activas: {ex.Message}");
                throw;
            }
        }

        private Abstracciones.ModeloUI.Categoria ConvertirObjetoParaUI(CategoriaAD categoriaAD)
        {
            return new Abstracciones.ModeloUI.Categoria
            {
                IdCategoria = categoriaAD.IdCategoria,
                NombreCategoria = categoriaAD.NombreCategoria,
                Imagen = categoriaAD.Imagen,
                Estado = categoriaAD.Estado
            };
        }
    }
}