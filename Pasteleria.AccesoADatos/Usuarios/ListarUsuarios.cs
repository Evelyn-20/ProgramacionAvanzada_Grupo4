using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Usuarios
{
    public class ListarUsuarios : IListarUsuarios
    {
        private Contexto _contexto;

        public ListarUsuarios()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Usuario> Obtener()
        {
            try
            {
                List<UsuarioAD> usuariosAD = _contexto.Usuario
                    .OrderBy(u => u.NombreUsuario)
                    .ToList();
                return usuariosAD.Select(u => ConvertirObjetoParaUI(u)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener usuarios: {ex.Message}");
                throw new Exception("Error al obtener la lista de usuarios", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Usuario> BuscarPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return new List<Abstracciones.ModeloUI.Usuario>();
                }

                List<UsuarioAD> usuariosAD = _contexto.Usuario
                    .Where(u => u.NombreUsuario.Contains(nombre))
                    .OrderBy(u => u.NombreUsuario)
                    .ToList();
                return usuariosAD.Select(u => ConvertirObjetoParaUI(u)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar usuarios por nombre: {ex.Message}");
                throw new Exception("Error al buscar usuarios por nombre", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Usuario> BuscarPorEmail(string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                {
                    return new List<Abstracciones.ModeloUI.Usuario>();
                }

                List<UsuarioAD> usuariosAD = _contexto.Usuario
                    .Where(u => u.Email.Contains(email))
                    .OrderBy(u => u.NombreUsuario)
                    .ToList();
                return usuariosAD.Select(u => ConvertirObjetoParaUI(u)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar usuarios por email: {ex.Message}");
                throw new Exception("Error al buscar usuarios por email", ex);
            }
        }

        private Abstracciones.ModeloUI.Usuario ConvertirObjetoParaUI(UsuarioAD usuarioAD)
        {
            return new Abstracciones.ModeloUI.Usuario
            {
                IdUsuario = usuarioAD.IdUsuario,
                NombreUsuario = usuarioAD.NombreUsuario,
                Email = usuarioAD.Email,
                Contrasenna = usuarioAD.Contrasenna,
                IdRol = usuarioAD.IdRol,
                Estado = usuarioAD.Estado
            };
        }
    }
}