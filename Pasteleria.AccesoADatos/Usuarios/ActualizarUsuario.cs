using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Usuarios
{
    public class ActualizarUsuario : IActualizarUsuario
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public ActualizarUsuario()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Actualizar(Abstracciones.ModeloUI.Usuario elUsuario)
        {
            try
            {
                var usuarioExistente = _contexto.Usuario
                    .FirstOrDefault(u => u.IdUsuario == elUsuario.IdUsuario);

                if (usuarioExistente == null)
                {
                    return 0;
                }

                // Validar que el email no esté duplicado (excepto para el mismo usuario)
                bool emailDuplicado = _contexto.Usuario
                    .Any(u => u.Email == elUsuario.Email && u.IdUsuario != elUsuario.IdUsuario);
                if (emailDuplicado)
                {
                    throw new Exception("Ya existe otro usuario con ese email");
                }

                // Validar que el nombre de usuario no esté duplicado
                bool nombreDuplicado = _contexto.Usuario
                    .Any(u => u.NombreUsuario == elUsuario.NombreUsuario && u.IdUsuario != elUsuario.IdUsuario);
                if (nombreDuplicado)
                {
                    throw new Exception("Ya existe otro usuario con ese nombre de usuario");
                }

                // Validar que el rol exista
                bool rolExiste = _contexto.Rol.Any(r => r.IdRol == elUsuario.IdRol);
                if (!rolExiste)
                {
                    throw new Exception("El rol seleccionado no existe");
                }

                // Guardar valores anteriores para auditoría
                var valoresAnteriores = new
                {
                    usuarioExistente.NombreUsuario,
                    usuarioExistente.Email,
                    usuarioExistente.IdRol,
                    usuarioExistente.Estado
                };

                // Actualizar los campos
                usuarioExistente.NombreUsuario = elUsuario.NombreUsuario?.Trim();
                usuarioExistente.Email = elUsuario.Email?.Trim();
                usuarioExistente.IdRol = elUsuario.IdRol;
                usuarioExistente.Estado = elUsuario.Estado;

                // Solo actualizar la contraseña si se proporciona una nueva
                if (!string.IsNullOrWhiteSpace(elUsuario.Contrasenna))
                {
                    usuarioExistente.Contrasenna = elUsuario.Contrasenna;
                }

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosActualizados > 0)
                {
                    _auditoria.RegistrarActualizacion("Usuario", elUsuario.IdUsuario,
                        valoresAnteriores,
                        new
                        {
                            usuarioExistente.NombreUsuario,
                            usuarioExistente.Email,
                            usuarioExistente.IdRol,
                            usuarioExistente.Estado
                        }
                    );
                }

                System.Diagnostics.Debug.WriteLine($"Usuario actualizado exitosamente. ID: {elUsuario.IdUsuario}");

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar usuario: {ex.Message}");
                throw;
            }
        }
    }
}