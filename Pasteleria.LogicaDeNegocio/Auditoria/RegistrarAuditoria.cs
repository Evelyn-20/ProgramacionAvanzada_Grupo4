using Pasteleria.Abstracciones.Logica.Auditoria;

namespace Pasteleria.LogicaDeNegocio.Auditoria
{
    public class RegistrarAuditoria : IRegistrarAuditoria
    {
        private IRegistrarAuditoria _registrarAuditoria;

        public RegistrarAuditoria()
        {
            _registrarAuditoria = new AccesoADatos.Auditoria.RegistrarAuditoria();
        }

        public void Registrar(string tabla, int idRegistro, string accion,
                             object valoresAnteriores = null, object valoresNuevos = null,
                             string descripcion = null, string usuarioNombre = "Sistema")
        {
            _registrarAuditoria.Registrar(tabla, idRegistro, accion, valoresAnteriores,
                                         valoresNuevos, descripcion, usuarioNombre);
        }

        public void RegistrarCreacion(string tabla, int idRegistro, object valoresNuevos,
                                      string usuarioNombre = "Sistema")
        {
            _registrarAuditoria.RegistrarCreacion(tabla, idRegistro, valoresNuevos,
                                                 usuarioNombre);
        }

        public void RegistrarActualizacion(string tabla, int idRegistro,
                                          object valoresAnteriores, object valoresNuevos,
                                          string usuarioNombre = "Sistema")
        {
            _registrarAuditoria.RegistrarActualizacion(tabla, idRegistro, valoresAnteriores,
                                                      valoresNuevos, usuarioNombre);
        }

        public void RegistrarEliminacion(string tabla, int idRegistro, object valoresAnteriores,
                                        string usuarioNombre = "Sistema")
        {
            _registrarAuditoria.RegistrarEliminacion(tabla, idRegistro, valoresAnteriores,
                                                    usuarioNombre);
        }
    }
}