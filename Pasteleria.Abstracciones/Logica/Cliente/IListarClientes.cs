using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Cliente
{
    public interface IListarClientes
    {
        List<ModeloUI.Cliente> Listar();
        List<ModeloUI.Cliente> BuscarPorNombreOCedula(string filtro);
    }
}