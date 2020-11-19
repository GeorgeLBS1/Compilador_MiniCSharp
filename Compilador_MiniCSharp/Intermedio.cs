using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    class Intermedio
    {
        string TipoFuncion;
        Dictionary<string, Metodo> variables = new Dictionary<string, Metodo>();

        public Intermedio(string _tp, Dictionary<string, Metodo> _variables)
        {
            TipoFuncion = _tp;
            variables = _variables;

        }
    }
}
