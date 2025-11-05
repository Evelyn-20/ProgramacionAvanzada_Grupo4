using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Auditoria;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Usuarios
{
    public class EliminarUsuario : IEliminarUsuario
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public EliminarUsuario()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Eliminar(int idUsuario)
        {
            try
            {
                UsuarioAD usuarioAEliminar = _contexto.Usuario
                    .FirstOrDefault(u => u.IdUsuario == idUsuario);

                if (usuarioAEliminar == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Usuario con ID {idUsuario} no encontrado");
                    return 0;
                }

                // Guardar información antes de eliminar para auditoría
                var infoUsuario = new
                {
                    usuarioAEliminar.IdUsuario,
                    usuarioAEliminar.NombreUsuario,
                    usuarioAEliminar.Email,
                    usuarioAEliminar.IdRol,
                    usuarioAEliminar.Estado
                };

                _contexto.Usuario.Remove(usuarioAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosEliminados > 0)
                {
                    _auditoria.RegistrarEliminacion("Usuario", idUsuario, infoUsuario);
                }

                System.Diagnostics.Debug.WriteLine($"Usuario eliminado exitosamente. ID: {idUsuario}");

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error de base de datos al eliminar usuario: {dbEx.Message}");
                throw new Exception("No se puede eliminar el usuario porque tiene registros relacionados", dbEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar usuario: {ex.Message}");
                throw;
            }
        }
    }
}