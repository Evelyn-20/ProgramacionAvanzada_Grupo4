using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Models.Models
{
    [Table("Categoria")]
    public class Categoria
    {
        [Column("IdCategoria")]
        public int IdCategoria { get; set; }

        [Column("NombreCategoria")]
        public string NombreCategoria { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}