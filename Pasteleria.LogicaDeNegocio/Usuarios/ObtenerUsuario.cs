using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class ObtenerUsuario : IObtenerUsuario
    {
        private IObtenerUsuario _obtenerUsuario;

        public ObtenerUsuario()
        {
            _obtenerUsuario = new AccesoADatos.Usuarios.ObtenerUsuario();
        }

        public Usuario Obtener(int id)
        {
            if (id <= 0)
            {
                throw new System.Exception("El ID del usuario no es válido");
            }

            Usuario elUsuario = _obtenerUsuario.Obtener(id);
            return elUsuario;
        }
    }
}