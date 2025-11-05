using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RolUI = Pasteleria.Abstracciones.ModeloUI.Rol;

namespace Pasteleria.Abstracciones.Logica.Rol
{
    public interface IListarRoles
    {
        List<RolUI> Obtener();
        List<RolUI> BuscarPorNombre(string nombre);
    }
}