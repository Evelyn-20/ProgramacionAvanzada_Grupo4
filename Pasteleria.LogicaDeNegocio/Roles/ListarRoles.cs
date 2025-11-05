using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace Pasteleria.LogicaDeNegocio.Roles
{
    public class ListarRoles : IListarRoles
    {
        private IListarRoles _listarRoles;

        public ListarRoles()
        {
            _listarRoles = new AccesoADatos.Roles.ListarRoles();
        }

        public List<Rol> Obtener()
        {
            return _listarRoles.Obtener();
        }

        public List<Rol> BuscarPorNombre(string nombre)
        {
            return _listarRoles.BuscarPorNombre(nombre);
        }
    }
}