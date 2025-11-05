using System;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Roles
{
    public class CrearRol : ICrearRol
    {
        private ICrearRol _crearRol;

        public CrearRol()
        {
            _crearRol = new AccesoADatos.Roles.CrearRol();
        }

        public async Task<int> Guardar(Rol elRol)
        {
            // Establece el estado como activo por defecto
            elRol.Estado = true;

            int cantidadDeResultados = await _crearRol.Guardar(elRol);
            return cantidadDeResultados;
        }
    }
}