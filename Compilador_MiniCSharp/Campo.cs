using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    class Campo
    {
        public string Estado { get; set; }
        public string Simbolo { get; set; }
        public string Identificador { get; set; } //Para saber donde se encuentra la operación

        public Campo(string estado, string simbolo)
        {
            Estado = estado;
            Simbolo = simbolo;
            Identificador = estado + "_" + simbolo;
        }
    }
}
