using Pasteleria.Abstracciones.Logica.Rol;

namespace Pasteleria.LogicaDeNegocio.Roles
{
    public class EliminarRol : IEliminarRol
    {
        private IEliminarRol _eliminarRol;

        public EliminarRol()
        {
            _eliminarRol = new AccesoADatos.Roles.EliminarRol();
        }

        public int Eliminar(int idRol)
        {
            int resultado = _eliminarRol.Eliminar(idRol);
            return resultado;
        }
    }
}