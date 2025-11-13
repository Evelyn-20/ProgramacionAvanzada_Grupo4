using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class ObtenerCliente : IObtenerCliente
    {
        private IObtenerCliente _obtenerCliente;

        public ObtenerCliente()
        {
            _obtenerCliente = new AccesoADatos.Clientes.ObtenerCliente();
        }

        public Cliente Obtener(int id)
        {
            Cliente elCliente = _obtenerCliente.Obtener(id);
            return elCliente;
        }
    }
}