using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UsuarioUI = Pasteleria.Abstracciones.ModeloUI.Usuario;

namespace Pasteleria.Abstracciones.Logica.Usuario
{
    public interface IListarUsuarios
    {
        List<UsuarioUI> Obtener();
        List<UsuarioUI> BuscarPorNombre(string nombre);
        List<UsuarioUI> BuscarPorEmail(string email);
    }
}