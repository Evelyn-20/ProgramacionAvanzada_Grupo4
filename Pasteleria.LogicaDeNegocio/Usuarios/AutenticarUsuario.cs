using Pasteleria.Abstracciones.Logica.Usuario;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pasteleria.LogicaDeNegocio.Usuarios
{
    public class AutenticarUsuario : IAutenticarUsuario
    {
        public (Usuario usuario, string nombreRol) Autenticar(string email, string contrasenna)
        {
            using (var context = new Contexto())
            {
                var usuarioAD = context.Usuario
                    .Include(u => u.Rol)
                    .FirstOrDefault(u => u.Email == email && u.Contrasenna == contrasenna && u.Estado == true);

                if (usuarioAD == null)
                    return (null, null);

                var usuario = new Usuario
                {
                    IdUsuario = usuarioAD.IdUsuario,
                    NombreUsuario = usuarioAD.NombreUsuario,
                    Email = usuarioAD.Email,
                    IdRol = usuarioAD.IdRol,
                    Estado = usuarioAD.Estado
                };

                return (usuario, usuarioAD.Rol.NombreRol);
            }
        }
    }
}
