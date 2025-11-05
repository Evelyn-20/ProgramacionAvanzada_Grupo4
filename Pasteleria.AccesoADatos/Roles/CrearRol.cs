using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Roles
{
    public class CrearRol : ICrearRol
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public CrearRol()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Rol elRol)
        {
            try
            {
                // Validar que no exista un rol con el mismo nombre
                bool nombreExiste = await _contexto.Rol.AnyAsync(r => r.NombreRol == elRol.NombreRol);
                if (nombreExiste)
                {
                    throw new Exception("Ya existe un rol registrado con ese nombre");
                }

                RolAD elRolAGuardar = ConvertirObjetoParaAD(elRol);
                _contexto.Rol.Add(elRolAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                // Registrar en auditoría
                if (cantidadDeDatosAgregados > 0)
                {
                    _auditoria.RegistrarCreacion("Rol", elRolAGuardar.IdRol, new
                    {
                        elRolAGuardar.IdRol,
                        elRolAGuardar.NombreRol,
                        elRolAGuardar.Estado
                    });
                }

                System.Diagnostics.Debug.WriteLine($"Rol guardado exitosamente. ID: {elRolAGuardar.IdRol}");
                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar rol: {ex.Message}");
                throw;
            }
        }

        private RolAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Rol rol)
        {
            return new RolAD
            {
                NombreRol = rol.NombreRol?.Trim(),
                Estado = rol.Estado
            };
        }
    }
}