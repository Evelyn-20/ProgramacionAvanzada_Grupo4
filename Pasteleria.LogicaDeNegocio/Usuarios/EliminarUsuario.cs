using Pasteleria.Abstracciones.Logica.Usuario;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class EliminarUsuario : IEliminarUsuario
    {
        private IEliminarUsuario _eliminarUsuario;

        public EliminarUsuario()
        {
            _eliminarUsuario = new AccesoADatos.Usuarios.EliminarUsuario();
        }

        public int Eliminar(int idUsuario)
        {
            if (idUsuario <= 0)
            {
                throw new System.Exception("El ID del usuario no es válido");
            }

            int resultado = _eliminarUsuario.Eliminar(idUsuario);
            return resultado;
        }
    }
}