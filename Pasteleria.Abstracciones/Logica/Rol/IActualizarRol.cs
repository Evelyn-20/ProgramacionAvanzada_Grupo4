using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Rol
{
    public interface IActualizarRol
    {
        int Actualizar(ModeloUI.Rol rol);
    }
}