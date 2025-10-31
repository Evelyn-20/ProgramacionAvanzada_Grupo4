using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class CrearCliente : ICrearCliente
    {
        private Contexto _contexto;

        public CrearCliente()
        {
            _contexto = new Contexto();
        }

        public async Task<int> Guardar(Abstracciones.ModeloUI.Cliente elCliente)
        {
            try
            {
                // Validar que no exista un cliente con la misma cédula
                bool cedulaExiste = await _contexto.Cliente.AnyAsync(c => c.Cedula == elCliente.Cedula);
                if (cedulaExiste)
                {
                    throw new Exception("Ya existe un cliente registrado con esa cédula");
                }

                // Validar que no exista un cliente con el mismo correo
                bool correoExiste = await _contexto.Cliente.AnyAsync(c => c.Correo == elCliente.Correo);
                if (correoExiste)
                {
                    throw new Exception("Ya existe un cliente registrado con ese correo electrónico");
                }

                ClienteAD elClienteAGuardar = ConvertirObjetoParaAD(elCliente);
                _contexto.Cliente.Add(elClienteAGuardar);
                int cantidadDeDatosAgregados = await _contexto.SaveChangesAsync();

                System.Diagnostics.Debug.WriteLine($"Cliente guardado exitosamente. ID: {elClienteAGuardar.IdCliente}");
                return cantidadDeDatosAgregados;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al guardar cliente: {ex.Message}");
                throw new Exception("Error al guardar el cliente en la base de datos", ex);
            }
        }

        private ClienteAD ConvertirObjetoParaAD(Abstracciones.ModeloUI.Cliente cliente)
        {
            return new ClienteAD
            {
                NombreCliente = cliente.NombreCliente?.Trim(),
                Cedula = cliente.Cedula?.Trim(),
                Correo = cliente.Correo?.Trim().ToLower(),
                Telefono = cliente.Telefono?.Trim(),
                Direccion = cliente.Direccion?.Trim(),
                Contrasenna = cliente.Contrasenna,
                Estado = cliente.Estado
            };
        }
    }
}