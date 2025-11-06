using Pasteleria.Abstracciones.Logica.Auditoria;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Auditoria
{
    public class ListarAuditorias : IListarAuditorias
    {
        private Contexto _contexto;

        public ListarAuditorias()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Auditoria> Obtener()
        {
            try
            {
                // Proyección directa en la consulta SQL para manejar NULL
                var auditorias = _contexto.Auditoria
                    .OrderByDescending(a => a.FechaAccion)
                    .Select(a => new Abstracciones.ModeloUI.Auditoria
                    {
                        IdAuditoria = a.IdAuditoria,
                        Tabla = a.Tabla ?? "",
                        IdRegistro = a.IdRegistro,
                        Accion = a.Accion ?? "",
                        UsuarioId = a.UsuarioId,
                        UsuarioNombre = a.UsuarioNombre ?? "Sistema",
                        ValoresAnteriores = a.ValoresAnteriores ?? "",
                        ValoresNuevos = a.ValoresNuevos ?? "",
                        Descripcion = a.Descripcion ?? "",
                        FechaAccion = a.FechaAccion
                    })
                    .ToList();

                return auditorias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener auditorías: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                throw new Exception("Error al obtener la lista de auditorías", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarPorTabla(string tabla)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(tabla))
                {
                    return new List<Abstracciones.ModeloUI.Auditoria>();
                }

                var auditorias = _contexto.Auditoria
                    .Where(a => a.Tabla != null && a.Tabla.Contains(tabla))
                    .OrderByDescending(a => a.FechaAccion)
                    .Select(a => new Abstracciones.ModeloUI.Auditoria
                    {
                        IdAuditoria = a.IdAuditoria,
                        Tabla = a.Tabla ?? "",
                        IdRegistro = a.IdRegistro,
                        Accion = a.Accion ?? "",
                        UsuarioId = a.UsuarioId,
                        UsuarioNombre = a.UsuarioNombre ?? "Sistema",
                        ValoresAnteriores = a.ValoresAnteriores ?? "",
                        ValoresNuevos = a.ValoresNuevos ?? "",
                        Descripcion = a.Descripcion ?? "",
                        FechaAccion = a.FechaAccion
                    })
                    .ToList();

                return auditorias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar auditorías por tabla: {ex.Message}");
                throw new Exception("Error al buscar auditorías por tabla", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarPorAccion(string accion)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(accion))
                {
                    return new List<Abstracciones.ModeloUI.Auditoria>();
                }

                var auditorias = _contexto.Auditoria
                    .Where(a => a.Accion != null && a.Accion.Contains(accion))
                    .OrderByDescending(a => a.FechaAccion)
                    .Select(a => new Abstracciones.ModeloUI.Auditoria
                    {
                        IdAuditoria = a.IdAuditoria,
                        Tabla = a.Tabla ?? "",
                        IdRegistro = a.IdRegistro,
                        Accion = a.Accion ?? "",
                        UsuarioId = a.UsuarioId,
                        UsuarioNombre = a.UsuarioNombre ?? "Sistema",
                        ValoresAnteriores = a.ValoresAnteriores ?? "",
                        ValoresNuevos = a.ValoresNuevos ?? "",
                        Descripcion = a.Descripcion ?? "",
                        FechaAccion = a.FechaAccion
                    })
                    .ToList();

                return auditorias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar auditorías por acción: {ex.Message}");
                throw new Exception("Error al buscar auditorías por acción", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarPorUsuario(string usuario)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario))
                {
                    return new List<Abstracciones.ModeloUI.Auditoria>();
                }

                var auditorias = _contexto.Auditoria
                    .Where(a => a.UsuarioNombre != null && a.UsuarioNombre.Contains(usuario))
                    .OrderByDescending(a => a.FechaAccion)
                    .Select(a => new Abstracciones.ModeloUI.Auditoria
                    {
                        IdAuditoria = a.IdAuditoria,
                        Tabla = a.Tabla ?? "",
                        IdRegistro = a.IdRegistro,
                        Accion = a.Accion ?? "",
                        UsuarioId = a.UsuarioId,
                        UsuarioNombre = a.UsuarioNombre ?? "Sistema",
                        ValoresAnteriores = a.ValoresAnteriores ?? "",
                        ValoresNuevos = a.ValoresNuevos ?? "",
                        Descripcion = a.Descripcion ?? "",
                        FechaAccion = a.FechaAccion
                    })
                    .ToList();

                return auditorias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar auditorías por usuario: {ex.Message}");
                throw new Exception("Error al buscar auditorías por usuario", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarGeneral(string termino)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(termino))
                {
                    return new List<Abstracciones.ModeloUI.Auditoria>();
                }

                var auditorias = _contexto.Auditoria
                    .Where(a => (a.Tabla != null && a.Tabla.Contains(termino)) ||
                               (a.Accion != null && a.Accion.Contains(termino)) ||
                               (a.UsuarioNombre != null && a.UsuarioNombre.Contains(termino)) ||
                               (a.Descripcion != null && a.Descripcion.Contains(termino)))
                    .OrderByDescending(a => a.FechaAccion)
                    .Select(a => new Abstracciones.ModeloUI.Auditoria
                    {
                        IdAuditoria = a.IdAuditoria,
                        Tabla = a.Tabla ?? "",
                        IdRegistro = a.IdRegistro,
                        Accion = a.Accion ?? "",
                        UsuarioId = a.UsuarioId,
                        UsuarioNombre = a.UsuarioNombre ?? "Sistema",
                        ValoresAnteriores = a.ValoresAnteriores ?? "",
                        ValoresNuevos = a.ValoresNuevos ?? "",
                        Descripcion = a.Descripcion ?? "",
                        FechaAccion = a.FechaAccion
                    })
                    .ToList();

                return auditorias;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar auditorías: {ex.Message}");
                throw new Exception("Error al buscar auditorías", ex);
            }
        }
    }
}