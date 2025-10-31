using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.AccesoADatos.Modelos;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class ListarClientes : IListarClientes
    {
        private Contexto _contexto;

        public ListarClientes()
        {
            _contexto = new Contexto();
        }

        public List<Abstracciones.ModeloUI.Cliente> Obtener()
        {
            try
            {
                List<ClienteAD> clientesAD = _contexto.Cliente
                    .OrderBy(c => c.NombreCliente)
                    .ToList();
                return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al obtener clientes: {ex.Message}");
                throw new Exception("Error al obtener la lista de clientes", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Cliente> BuscarPorNombre(string nombre)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    return new List<Abstracciones.ModeloUI.Cliente>();
                }

                List<ClienteAD> clientesAD = _contexto.Cliente 
                    .Where(c => c.NombreCliente.Contains(nombre) ||
                                c.Correo.Contains(nombre) ||
                                c.Telefono.Contains(nombre))
                    .OrderBy(c => c.NombreCliente)
                    .ToList();
                return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar clientes por nombre: {ex.Message}");
                throw new Exception("Error al buscar clientes por nombre", ex);
            }
        }

        public List<Abstracciones.ModeloUI.Cliente> BuscarPorCedula(string cedula)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(cedula))
                {
                    return new List<Abstracciones.ModeloUI.Cliente>();
                }

                List<ClienteAD> clientesAD = _contexto.Cliente
                    .Where(c => c.Cedula.Contains(cedula))
                    .OrderBy(c => c.NombreCliente)
                    .ToList();
                return clientesAD.Select(c => ConvertirObjetoParaUI(c)).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al buscar clientes por cédula: {ex.Message}");
                throw new Exception("Error al buscar clientes por cédula", ex);
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