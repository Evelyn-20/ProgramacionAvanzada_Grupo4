using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Pasteleria.LogicaDeNegocio.Clientes
{
    public class AutenticarCliente : IAutenticarCliente
    {
        public Cliente Autenticar(string correo, string contrasenna)
        {
            using (var context = new Contexto())
            {
                var clienteAD = context.Cliente
                    .FirstOrDefault(c => c.Correo == correo && c.Contrasenna == contrasenna && c.Estado == true);

                if (clienteAD == null)
                    return null;

                return new Cliente
                {
                    IdCliente = clienteAD.IdCliente,
                    NombreCliente = clienteAD.NombreCliente,
                    Cedula = clienteAD.Cedula,
                    Correo = clienteAD.Correo,
                    Telefono = clienteAD.Telefono,
                    Direccion = clienteAD.Direccion,
                    Estado = clienteAD.Estado
                };
            }
        }
    }
}
