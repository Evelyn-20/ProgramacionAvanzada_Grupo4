using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Roles
{
    public class ListarRoles : IListarRoles
    {
        private Contexto _contexto;

        public ListarRoles()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Rol> Obtener()
        {
            try
            {
                List<RolAD> rolesAD = _contexto.Rol
                    .OrderBy(r => r.NombreRol)
                    .ToList();
                return rolesAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener roles: {ex.Message}");
                throw new Exception("Error al obtener la lista de roles", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Rol> BuscarPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return new List<Abstracciones.ModeloUI.Rol>();
                }

                List<RolAD> rolesAD = _contexto.Rol
                    .Where(r => r.NombreRol.Contains(nombre))
                    .OrderBy(r => r.NombreRol)
                    .ToList();
                return rolesAD.Select(r => ConvertirObjetoParaUI(r)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar roles por nombre: {ex.Message}");
                throw new Exception("Error al buscar roles por nombre", ex);
            }
        }

        private Abstracciones.ModeloUI.Rol ConvertirObjetoParaUI(RolAD rolAD)
        {
            return new Abstracciones.ModeloUI.Rol
            {
                IdRol = rolAD.IdRol,
                NombreRol = rolAD.NombreRol,
                Estado = rolAD.Estado
            };
        }
    }
}