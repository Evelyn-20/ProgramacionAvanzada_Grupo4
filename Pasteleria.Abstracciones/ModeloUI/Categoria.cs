using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public class Categoria
    {
        public int IdCategoria { get; set; }
        [DisplayName("Nombre de la categoría")]
        public string NombreCategoria { get; set; }
        public bool Estado { get; set; }
    }
} 