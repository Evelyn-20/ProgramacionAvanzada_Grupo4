using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class ListarClientes : IListarClientes
    {
        private IListarClientes _listarClientes;

        public ListarClientes()
        {
            _listarClientes = new AccesoADatos.Clientes.ListarClientes();
        }

        public List<Cliente> Obtener()
        {
            return _listarClientes.Obtener();
        }

        public List<Cliente> BuscarPorNombre(string nombre)
        {
            return _listarClientes.BuscarPorNombre(nombre);
        }

        public List<Cliente> BuscarPorCedula(string cedula)
        {
            return _listarClientes.BuscarPorCedula(cedula);
        }
    }
}