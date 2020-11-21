using System;
using System.Collections.Generic;
using System.Text;

namespace MiniC
{
    class Tipos
    {
        Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
        Dictionary<string, Intermedio> Metodos = new Dictionary<string, Intermedio>();
        public Tipos(Dictionary<string, Variable> _Variables, Dictionary<string, Intermedio> _Metodos)
        {
            Variables = _Variables;
            Metodos = _Metodos;
        }
    }
}
