using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Roles
{
    public class ActualizarRol : IActualizarRol
    {
        private IActualizarRol _actualizarRol;

        public ActualizarRol()
        {
            _actualizarRol = new AccesoADatos.Roles.ActualizarRol();
        }

        public int Actualizar(Rol rol)
        {
            int resultado = _actualizarRol.Actualizar(rol);
            return resultado;
        }
    }
}