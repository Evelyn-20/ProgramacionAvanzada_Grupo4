using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pasteleria.AccesoADatos.Modelos
{
    [Table("Auditoria")]
    public class AuditoriaAD
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdAuditoria { get; set; }

        [MaxLength(100)]
        public string Tabla { get; set; }

        public int IdRegistro { get; set; }

        [MaxLength(50)]
        public string Accion { get; set; }

        public int? UsuarioId { get; set; }

        [MaxLength(100)]
        public string UsuarioNombre { get; set; }

        public string ValoresAnteriores { get; set; }

        public string ValoresNuevos { get; set; }

        [MaxLength(500)]
        public string Descripcion { get; set; }

        public DateTime FechaAccion { get; set; }
    }
}