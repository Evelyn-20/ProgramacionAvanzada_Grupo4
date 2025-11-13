using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pasteleria.Abstracciones.ModeloUI;

namespace Pasteleria.Abstracciones.Logica.Auditoria
{
    public interface IListarAuditorias
    {
        List<Abstracciones.ModeloUI.Auditoria> Obtener();
        List<Abstracciones.ModeloUI.Auditoria> BuscarPorTabla(string tabla);
        List<Abstracciones.ModeloUI.Auditoria> BuscarPorAccion(string accion);
        List<Abstracciones.ModeloUI.Auditoria> BuscarPorUsuario(string usuario);
        List<Abstracciones.ModeloUI.Auditoria> BuscarGeneral(string termino);
    }
}