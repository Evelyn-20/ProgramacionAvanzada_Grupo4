using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class ObtenerCliente : IObtenerCliente
    {
        private Contexto _contexto;

        public ObtenerCliente()
        {
            _contexto = new Contexto();
        }

        public Abstracciones.ModeloUI.Cliente Obtener(int idCliente)
        {
            try
            {
                ClienteAD clienteAD = _contexto.Cliente
                    .FirstOrDefault(c => c.IdCliente == idCliente);

                if (clienteAD == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Cliente con ID {idCliente} no encontrado");
                    return null;
                }

                return ConvertirObjetoParaUI(clienteAD);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener cliente: {ex.Message}");
                throw new Exception($"Error al obtener el cliente con ID {idCliente}", ex);
            }
        }

        private Abstracciones.ModeloUI.Cliente ConvertirObjetoParaUI(ClienteAD clienteAD)
        {
            return new Abstracciones.ModeloUI.Cliente
            {
                IdCliente = clienteAD.IdCliente,
                NombreCliente = clienteAD.NombreCliente,
                Cedula = clienteAD.Cedula,
                Correo = clienteAD.Correo,
                Telefono = clienteAD.Telefono,
                Direccion = clienteAD.Direccion,
                Contrasenna = clienteAD.Contrasenna,
                Estado = clienteAD.Estado
            };
        }
    }
}