using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    class Variable
    {
         string Valor { get; set; }
        string TipoDato { get; set; }

         int CInicio { get; set; }
        int CFinal { get; set; }
         int Linea { get; set; }
         int Constante { get; set; }

        public Variable(string Val, string tp,  int ini, int fin, int lin, int _Consante)
        {
            
            Valor = Val;
            TipoDato = tp;
          
            CInicio = ini + 1;
            CFinal = fin + 1;
            Linea = lin;
            Constante = _Consante;
        }
    }
}
