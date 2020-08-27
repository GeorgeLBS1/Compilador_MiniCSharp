using System;
using System.Collections.Generic;
using System.Text;

namespace Compilador_MiniCSharp
{
    public class Token
    {
        public string Palabra { get; set; }
        public int CInicio { get; set; }
        public int CFinal { get; set; }
        public int Linea { get; set; }
        public int Tipo_token { get; set; }

        public Token(string p, int ini, int fin, int lin, int tip)
        {
            Palabra = p;
            CInicio = ini + 1;
            CFinal = fin + 1;
            Linea = lin;
            Tipo_token = tip;
        }
    }
}
