using System;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class CrearUsuario : ICrearUsuario
    {
        private ICrearUsuario _crearUsuario;

        public CrearUsuario()
        {
            _crearUsuario = new AccesoADatos.Usuarios.CrearUsuario();
        }

        public async Task<int> Guardar(Usuario elUsuario)
        {
            // Validaciones de negocio
            if (string.IsNullOrWhiteSpace(elUsuario.NombreUsuario))
            {
                throw new Exception("El nombre de usuario es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(elUsuario.Email))
            {
                throw new Exception("El email es obligatorio");
            }

            if (string.IsNullOrWhiteSpace(elUsuario.Contrasenna))
            {
                throw new Exception("La contraseña es obligatoria");
            }

            if (elUsuario.Contrasenna.Length < 6)
            {
                throw new Exception("La contraseña debe tener al menos 6 caracteres");
            }

            if (elUsuario.IdRol <= 0)
            {
                throw new Exception("Debe seleccionar un rol válido");
            }

            // Establece el estado como activo por defecto
            elUsuario.Estado = true;

            int cantidadDeResultados = await _crearUsuario.Guardar(elUsuario);
            return cantidadDeResultados;
        }
    }
}