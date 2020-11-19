using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    class Variable
    {
        public string Valor { get; set; }
        public string TipoDato { get; set; }

        public int CInicio { get; set; }
        public int CFinal { get; set; }
        public int Linea { get; set; }

        public Variable(string Val, string tp,  int ini, int fin, int lin)
        {
            
            Valor = Val;
            TipoDato = tp;
          
            CInicio = ini + 1;
            CFinal = fin + 1;
            Linea = lin;

        }
    }
}
