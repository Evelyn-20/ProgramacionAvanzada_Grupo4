using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.Logica.Auditoria
{
    public interface IRegistrarAuditoria
    {
        void Registrar(string tabla, int idRegistro, string accion,
                      object valoresAnteriores = null, object valoresNuevos = null,
                      string descripcion = null, string usuarioNombre = "Sistema");

        void RegistrarCreacion(string tabla, int idRegistro, object valoresNuevos,
                              string usuarioNombre = "Sistema");

        void RegistrarActualizacion(string tabla, int idRegistro,
                                    object valoresAnteriores, object valoresNuevos,
                                    string usuarioNombre = "Sistema");

        void RegistrarEliminacion(string tabla, int idRegistro, object valoresAnteriores,
                                 string usuarioNombre = "Sistema");
    }
}