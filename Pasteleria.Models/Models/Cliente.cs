using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Models.Models
{
    [Table("Cliente")]
    public class Cliente
    {
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("NombreCliente")]
        public string NombreCliente { get; set; }

        [Column("Cedula")]
        public string Cedula { get; set; }

        [Column("Correo")]
        public string Correo { get; set; }

        [Column("Telefono")]
        public string Telefono { get; set; }

        [Column("Direccion")]
        public string Direccion { get; set; }

        [Column("Contrasenna")]
        public string Contrasenna { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}