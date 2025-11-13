using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Roles
{
    public class ObtenerRol : IObtenerRol
    {
        private Contexto _contexto;

        public ObtenerRol()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Rol Obtener(int idRol)
        {
            try
            {
                RolAD rolAD = _contexto.Rol
                    .FirstOrDefault(r => r.IdRol == idRol);

                if (rolAD == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Rol con ID {idRol} no encontrado");
                    return null;
                }

                return ConvertirObjetoParaUI(rolAD);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener rol: {ex.Message}");
                throw new Exception($"Error al obtener el rol con ID {idRol}", ex);
            }
        }

        private Abstracciones.ModeloUI.Rol ConvertirObjetoParaUI(RolAD rolAD)
        {
            return new Abstracciones.ModeloUI.Rol
            {
                IdRol = rolAD.IdRol,
                NombreRol = rolAD.NombreRol,
                Estado = rolAD.Estado
            };
        }
    }
}