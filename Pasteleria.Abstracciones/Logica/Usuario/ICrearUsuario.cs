using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Usuario
{
    public interface ICrearUsuario
    {
        Task<int> Guardar(ModeloUI.Usuario elUsuario);
    }
}