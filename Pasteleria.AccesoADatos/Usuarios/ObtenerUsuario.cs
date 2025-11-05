using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Usuarios
{
    public class ObtenerUsuario : IObtenerUsuario
    {
        private Contexto _contexto;

        public ObtenerUsuario()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Usuario Obtener(int idUsuario)
        {
            try
            {
                UsuarioAD usuarioAD = _contexto.Usuario
                    .FirstOrDefault(u => u.IdUsuario == idUsuario);

                if (usuarioAD == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Usuario con ID {idUsuario} no encontrado");
                    return null;
                }

                return ConvertirObjetoParaUI(usuarioAD);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener usuario: {ex.Message}");
                throw new Exception($"Error al obtener el usuario con ID {idUsuario}", ex);
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