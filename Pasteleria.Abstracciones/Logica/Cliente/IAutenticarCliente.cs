using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Cliente
{
    public interface IAutenticarCliente
    {
        ModeloUI.Cliente Autenticar(string correo, string contrasenna);
    }
}
