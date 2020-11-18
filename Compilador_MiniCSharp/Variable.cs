using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    class Variable
    {
        public string Valor { get; set; }
        public int TipoDato { get; set; }

        public int CInicio { get; set; }
        public int CFinal { get; set; }
        public int Linea { get; set; }

        public Variable(string Val, int tp,  int ini, int fin, int lin, int tip)
        {
            
            Valor = Val;
            TipoDato = tp;
          
            CInicio = ini + 1;
            CFinal = fin + 1;
            Linea = lin;

        }
    }
}
