using Pasteleria.Abstracciones.Logica.Auditoria;
using Pasteleria.Abstracciones.ModeloUI;
using System.Collections.Generic;

namespace Pasteleria.LogicaDeNegocio.Auditoria
{
    public class ListarAuditorias : IListarAuditorias
    {
        private IListarAuditorias _listarAuditorias;

        public ListarAuditorias()
        {
            _listarAuditorias = new AccesoADatos.Auditoria.ListarAuditorias();
        }

        public List<Abstracciones.ModeloUI.Auditoria> Obtener()
        {
            return _listarAuditorias.Obtener();
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarPorTabla(string tabla)
        {
            return _listarAuditorias.BuscarPorTabla(tabla);
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarPorAccion(string accion)
        {
            return _listarAuditorias.BuscarPorAccion(accion);
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarPorUsuario(string usuario)
        {
            return _listarAuditorias.BuscarPorUsuario(usuario);
        }

        public List<Abstracciones.ModeloUI.Auditoria> BuscarGeneral(string termino)
        {
            return _listarAuditorias.BuscarGeneral(termino);
        }
    }
}