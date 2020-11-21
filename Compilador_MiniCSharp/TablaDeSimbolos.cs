using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    public class TablaDeSimbolos
    {
        public string Identificador { get; set; }
        public string Valor { get; set; }
        public TablaDeSimbolos(string id, string valor)
        {
            Identificador = id;
            Valor = valor;
        }
    }
}
