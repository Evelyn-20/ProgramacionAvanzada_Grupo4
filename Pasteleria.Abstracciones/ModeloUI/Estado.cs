using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pasteleria.Abstracciones.ModeloUI
{
    public static class Estado
    {
        public static bool activo = true;
        public static bool inactivo = false;
    }

    public enum Estados
    {
        Activo = 1,
        Inactivo = 2
    }
}
