using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Cliente
    {
        public int IdCliente { get; set; }
        [DisplayName("Nombre del cliente")]
        public string NombreCliente { get; set; }
        [DisplayName("Cédula")]
        public string Cedula { get; set; }
        [DisplayName("Correo electrónico")]
        public string Correo { get; set; }
        [DisplayName("Teléfono")]
        public string Telefono { get; set; }
        [DisplayName("Dirección")]
        public string Direccion { get; set; }
        [DisplayName("Contraseña")]
        public string Contrasenna { get; set; }
        public bool Estado { get; set; }
    }
} 