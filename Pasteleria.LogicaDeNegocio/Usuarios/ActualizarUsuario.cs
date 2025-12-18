using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class ActualizarUsuario : IActualizarUsuario
    {
        private IActualizarUsuario _actualizarUsuario;
        private IObtenerUsuario _obtenerUsuario;

        public ActualizarUsuario()
        {
            _actualizarUsuario = new AccesoADatos.Usuarios.ActualizarUsuario();
            _obtenerUsuario = new AccesoADatos.Usuarios.ObtenerUsuario();
        }

        public int Actualizar(Usuario usuario)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                throw new System.Exception("El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                throw new System.Exception("El email es obligatorio");
            }

            // Obtener usuario existente para manejar la contraseña
            var usuarioExistente = _obtenerUsuario.Obtener(usuario.IdUsuario);
            if (usuarioExistente == null)
            {
                throw new System.Exception("El usuario no existe en la base de datos");
            }

            // Manejo de contraseña
            if (string.IsNullOrWhiteSpace(usuario.Contrasenna))
            {
                // Si no se proporciona contraseña, mantener la existente
                usuario.Contrasenna = usuarioExistente.Contrasenna;
            }
            else
            {
                // Si se proporciona contraseña, validar y encriptar
                if (usuario.Contrasenna.Length < 6)
                {
                    throw new System.Exception("La contraseña debe tener al menos 6 caracteres");
                }

                // Aquí puedes agregar más validaciones si lo deseas
                usuario.Contrasenna = BCrypt.Net.BCrypt.HashPassword(usuario.Contrasenna);
            }

            int resultado = _actualizarUsuario.Actualizar(usuario);
            return resultado;
        }
    }
}