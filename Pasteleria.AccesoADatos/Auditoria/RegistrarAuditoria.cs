using Pasteleria.Abstracciones.Logica.Auditoria;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Text.Json;

namespace Pasteleria.AccesoADatos.Auditoria
{
    public class RegistrarAuditoria : IRegistrarAuditoria
    {
        private Contexto _contexto;

        public RegistrarAuditoria()
        {
            _contexto = new Contexto();
        }

        public void Registrar(string tabla, int idRegistro, string accion,
                             object valoresAnteriores = null, object valoresNuevos = null,
                             string descripcion = null, string usuarioNombre = "Sistema")
        {
            try
            {
                var auditoria = new AuditoriaAD
                {
                    Tabla = tabla,
                    IdRegistro = idRegistro,
                    Accion = accion,
                    UsuarioNombre = usuarioNombre ?? "Sistema",
                    ValoresAnteriores = valoresAnteriores != null ? JsonSerializer.Serialize(valoresAnteriores) : null,
                    ValoresNuevos = valoresNuevos != null ? JsonSerializer.Serialize(valoresNuevos) : null,
                    Descripcion = descripcion,
                    FechaAccion = DateTime.Now
                };

                _contexto.Auditoria.Add(auditoria);
                _contexto.SaveChanges();
            }
            catch (Exception ex)
            {
                // Log del error pero no detener la operación principal
                System.Diagnostics.Debug.WriteLine($"Error al registrar auditoría: {ex.Message}");
            }
        }

        public void RegistrarCreacion(string tabla, int idRegistro, object valoresNuevos,
                                      string usuarioNombre = "Sistema")
        {
            Registrar(tabla, idRegistro, "CREAR", null, valoresNuevos,
                     $"Se creó un nuevo registro en {tabla}", usuarioNombre);
        }

        public void RegistrarActualizacion(string tabla, int idRegistro,
                                          object valoresAnteriores, object valoresNuevos,
                                          string usuarioNombre = "Sistema")
        {
            Registrar(tabla, idRegistro, "ACTUALIZAR", valoresAnteriores, valoresNuevos,
                     $"Se actualizó el registro en {tabla}", usuarioNombre);
        }

        public void RegistrarEliminacion(string tabla, int idRegistro, object valoresAnteriores,
                                        string usuarioNombre = "Sistema")
        {
            Registrar(tabla, idRegistro, "ELIMINAR", valoresAnteriores, null,
                     $"Se eliminó el registro de {tabla}", usuarioNombre);
        }
    }
}