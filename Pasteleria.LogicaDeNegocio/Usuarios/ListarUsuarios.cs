using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class ListarUsuarios : IListarUsuarios
    {
        private IListarUsuarios _listarUsuarios;

        public ListarUsuarios()
        {
            _listarUsuarios = new AccesoADatos.Usuarios.ListarUsuarios();
        }

        public List<Usuario> Obtener()
        {
            return _listarUsuarios.Obtener();
        }

        public List<Usuario> BuscarPorNombre(string nombre)
        {
            return _listarUsuarios.BuscarPorNombre(nombre);
        }

        public List<Usuario> BuscarPorEmail(string email)
        {
            return _listarUsuarios.BuscarPorEmail(email);
        }
    }
}