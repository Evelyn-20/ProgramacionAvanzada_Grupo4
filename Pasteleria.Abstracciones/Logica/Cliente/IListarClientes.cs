using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClienteUI = Pasteleria.Abstracciones.ModeloUI.Cliente;

namespace Pasteleria.Abstracciones.Logica.Cliente
{
    public interface IListarClientes
    {
        List<ClienteUI> Obtener();
        List<ClienteUI> BuscarPorNombre(string nombre);
        List<ClienteUI> BuscarPorCedula(string cedula);
    }
}