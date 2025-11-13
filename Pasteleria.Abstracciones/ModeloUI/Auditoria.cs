using System;
using System.ComponentModel;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Auditoria
    {
        public int IdAuditoria { get; set; }

        [DisplayName("Tabla")]
        public string Tabla { get; set; }

        [DisplayName("ID del Registro")]
        public int IdRegistro { get; set; }

        [DisplayName("Acción")]
        public string Accion { get; set; }

        [DisplayName("ID de Usuario")]
        public int? UsuarioId { get; set; }

        [DisplayName("Usuario")]
        public string UsuarioNombre { get; set; }

        [DisplayName("Valores Anteriores")]
        public string ValoresAnteriores { get; set; }

        [DisplayName("Valores Nuevos")]
        public string ValoresNuevos { get; set; }

        [DisplayName("Descripción")]
        public string Descripcion { get; set; }

        [DisplayName("Fecha de Acción")]
        public DateTime FechaAccion { get; set; }

        // Propiedades calculadas
        [DisplayName("Fecha")]
        public string FechaFormateada => FechaAccion.ToString("dd/MM/yyyy HH:mm:ss");

        [DisplayName("Acción")]
        public string AccionFormateada
        {
            get
            {
                return Accion switch
                {
                    "CREAR" => "Creación",
                    "ACTUALIZAR" => "Actualización",
                    "ELIMINAR" => "Eliminación",
                    _ => Accion
                };
            }
        }
    }
}