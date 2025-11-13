using Pasteleria.Abstracciones.Logica.Rol;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Roles
{
    public class ActualizarRol : IActualizarRol
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public ActualizarRol()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Actualizar(Abstracciones.ModeloUI.Rol elRol)
        {
            try
            {
                var rolExistente = _contexto.Rol
                    .FirstOrDefault(r => r.IdRol == elRol.IdRol);

                if (rolExistente == null)
                {
                    return 0;
                }

                // Validar que el nombre no esté duplicado (excepto para el mismo rol)
                bool nombreDuplicado = _contexto.Rol
                    .Any(r => r.NombreRol == elRol.NombreRol && r.IdRol != elRol.IdRol);
                if (nombreDuplicado)
                {
                    throw new Exception("Ya existe otro rol con ese nombre");
                }

                // Guardar valores anteriores para auditoría
                var valoresAnteriores = new
                {
                    rolExistente.NombreRol,
                    rolExistente.Estado
                };

                // Actualizar los campos
                rolExistente.NombreRol = elRol.NombreRol?.Trim();
                rolExistente.Estado = elRol.Estado;

                int cantidadDeDatosActualizados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosActualizados > 0)
                {
                    _auditoria.RegistrarActualizacion("Rol", elRol.IdRol,
                        valoresAnteriores,
                        new
                        {
                            rolExistente.NombreRol,
                            rolExistente.Estado
                        }
                    );
                }

                System.Diagnostics.Debug.WriteLine($"Rol actualizado exitosamente. ID: {elRol.IdRol}");

                return cantidadDeDatosActualizados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al actualizar rol: {ex.Message}");
                throw;
            }
        }
    }
}