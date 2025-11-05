using Microsoft.EntityFrameworkCore;
using Pasteleria.Abstracciones.Logica.Cliente;
using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.AccesoADatos.Modelos;
using Pasteleria.AccesoADatos.Auditoria;
using System;
using System.Linq;

namespace Pasteleria.AccesoADatos.Clientes
{
    public class EliminarCliente : IEliminarCliente
    {
        private Contexto _contexto;
        private RegistrarAuditoria _auditoria;

        public EliminarCliente()
        {
            _contexto = new Contexto();
            _auditoria = new RegistrarAuditoria();
        }

        public int Eliminar(int idCliente)
        {
            try
            {
                ClienteAD clienteAEliminar = _contexto.Cliente
                    .FirstOrDefault(c => c.IdCliente == idCliente);

                if (clienteAEliminar == null)
                {
                    System.Diagnostics.Debug.WriteLine($"Cliente con ID {idCliente} no encontrado");
                    return 0;
                }

                // Guardar información antes de eliminar para auditoría (sin contraseña)
                var infoCliente = new
                {
                    clienteAEliminar.IdCliente,
                    clienteAEliminar.NombreCliente,
                    clienteAEliminar.Cedula,
                    clienteAEliminar.Correo,
                    clienteAEliminar.Telefono,
                    clienteAEliminar.Direccion,
                    clienteAEliminar.Estado
                };

                _contexto.Cliente.Remove(clienteAEliminar);
                int cantidadDeDatosEliminados = _contexto.SaveChanges();

                // Registrar en auditoría
                if (cantidadDeDatosEliminados > 0)
                {
                    _auditoria.RegistrarEliminacion("Cliente", idCliente, infoCliente);
                }

                System.Diagnostics.Debug.WriteLine($"Cliente eliminado exitosamente. ID: {idCliente}");

                return cantidadDeDatosEliminados;
            }
            catch (DbUpdateException dbEx)
            {
                System.Diagnostics.Debug.WriteLine($"Error de base de datos al eliminar cliente: {dbEx.Message}");
                throw new Exception("No se puede eliminar el cliente porque tiene registros relacionados", dbEx);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error al eliminar cliente: {ex.Message}");
                throw;
            }
        }
    }
}