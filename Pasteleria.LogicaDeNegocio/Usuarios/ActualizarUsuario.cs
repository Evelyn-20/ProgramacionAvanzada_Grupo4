using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class ActualizarUsuario : IActualizarUsuario
    {
        private IActualizarUsuario _actualizarUsuario;

        public ActualizarUsuario()
        {
            _actualizarUsuario = new AccesoADatos.Usuarios.ActualizarUsuario();
        }

        public int Actualizar(Usuario usuario)
        {
            // Validaciones de negocio si son necesarias
            if (string.IsNullOrWhiteSpace(usuario.NombreUsuario))
            {
                throw new System.Exception("El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(usuario.Email))
            {
                throw new System.Exception("El email es obligatorio");
            }

            int resultado = _actualizarUsuario.Actualizar(usuario);
            return resultado;
        }
    }
}