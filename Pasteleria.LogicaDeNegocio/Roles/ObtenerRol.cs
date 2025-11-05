using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Roles
{
    public class ObtenerRol : IObtenerRol
    {
        private IObtenerRol _obtenerRol;

        public ObtenerRol()
        {
            _obtenerRol = new AccesoADatos.Roles.ObtenerRol();
        }

        public Rol Obtener(int id)
        {
            Rol elRol = _obtenerRol.Obtener(id);
            return elRol;
        }
    }
}