using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Usuario
{
    public interface IAutenticarUsuario
    {
        (ModeloUI.Usuario usuario, string nombreRol) Autenticar(string email, string contrasenna);
    }
}
