using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Auditoria")]
    public class AuditoriaAD
    {
        [Key]
        public int IdAuditoria { get; set; }

        [Required]
        [StringLength(100)]
        public string Tabla { get; set; }

        [Required]
        public int IdRegistro { get; set; }

        [Required]
        [StringLength(50)]
        public string Accion { get; set; }

        public int? UsuarioId { get; set; }

        [StringLength(200)]
        public string UsuarioNombre { get; set; }

        public string ValoresAnteriores { get; set; }

        public string ValoresNuevos { get; set; }

        [StringLength(500)]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaAccion { get; set; }
    }
}