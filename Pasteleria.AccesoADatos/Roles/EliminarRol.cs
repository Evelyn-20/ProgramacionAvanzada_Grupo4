using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Auditoria;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Roles
{
    public class EliminarRol : IEliminarRol
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public EliminarRol()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Eliminar(int idRol)
        {
            try
            {
                RolAD rolAEliminar = _contexto.Rol
                    .FirstOrDefault(r => r.IdRol == idRol);

                if (rolAEliminar == null)
                {
                    return 0;
                }

                // Guardar información antes de eliminar para auditoría
                var infoRol = new
                {
                    rolAEliminar.IdRol,
                    rolAEliminar.NombreRol,
                    rolAEliminar.Estado
                };

                _contexto.Rol.Remove(rolAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosEliminados > 0)
                {
                    _auditoria.RegistrarEliminacion("Rol", idRol, infoRol);
                }

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                throw new Exception("No se puede eliminar el rol porque tiene registros relacionados", dbEx);
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}