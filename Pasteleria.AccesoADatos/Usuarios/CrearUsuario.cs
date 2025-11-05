using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Usuarios
{
    public class CrearUsuario : ICrearUsuario
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public CrearUsuario()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Usuario elUsuario)
        {
            try
            {
                // Validar que no exista un usuario con el mismo email
                bool emailExiste = await _contexto.Usuario.AnyAsync(u => u.Email == elUsuario.Email);
                if (emailExiste)
                {
                    throw new Exception("Ya existe un usuario registrado con ese email");
                }

                // Validar que no exista un usuario con el mismo nombre
                bool nombreExiste = await _contexto.Usuario.AnyAsync(u => u.NombreUsuario == elUsuario.NombreUsuario);
                if (nombreExiste)
                {
                    throw new Exception("Ya existe un usuario registrado con ese nombre de usuario");
                }

                // Validar que el rol exista
                bool rolExiste = await _contexto.Rol.AnyAsync(r => r.IdRol == elUsuario.IdRol);
                if (!rolExiste)
                {
                    throw new Exception("El rol seleccionado no existe");
                }

                UsuarioAD elUsuarioAGuardar = ConvertirObjetoParaAD(elUsuario);
                _contexto.Usuario.Add(elUsuarioAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                // Registrar en auditoría
                if (cantidadDeDatosAgregados > 0)
                {
                    _auditoria.RegistrarCreacion("Usuario", elUsuarioAGuardar.IdUsuario, new
                    {
                        elUsuarioAGuardar.IdUsuario,
                        elUsuarioAGuardar.NombreUsuario,
                        elUsuarioAGuardar.Email,
                        elUsuarioAGuardar.IdRol,
                        elUsuarioAGuardar.Estado
                    });
                }

                System.Diagnostics.Debug.WriteLine($"Usuario guardado exitosamente. ID: {elUsuarioAGuardar.IdUsuario}");
                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar usuario: {ex.Message}");
                throw;
            }
        }

        private UsuarioAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Usuario usuario)
        {
            return new UsuarioAD
            {
                NombreUsuario = usuario.NombreUsuario?.Trim(),
                Email = usuario.Email?.Trim(),
                Contrasenna = usuario.Contrasenna,
                IdRol = usuario.IdRol,
                Estado = usuario.Estado
            };
        }
    }
}