using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using Compilador_MiniCSharp;

namespace MiniC
{
    class AnalizadorSintactico
    {
        Dictionary<int, int> Gramatica = new Dictionary<int, int>();
        Dictionary<int, string> GramaticaLetras = new Dictionary<int, string>();
        Dictionary<string, int[]> Transiciones = new Dictionary<string, int[]>();
        Queue<int> PilaNumeros = new Queue<int>();
        Queue<string> PilaLetras = new Queue<string>();
        void LeerGramatica()
        {
            string[] linea;

            using (var file = new FileStream("Gcaracter.txt", FileMode.Open))
            {
                using (var lector = new StreamReader(file))
                {
                    while (!lector.EndOfStream)
                    {
                        linea = lector.ReadLine().Split(',');
                        Gramatica.Add(Convert.ToInt32(linea[0]), Convert.ToInt32(linea[1]));

                    }

                }
            }
            using (var file = new FileStream("GSLetras.txt", FileMode.Open))
            {
                using (var lector = new StreamReader(file))
                {
                    while (!lector.EndOfStream)
                    {
                        linea = lector.ReadLine().Split(',');
                        GramaticaLetras.Add(Convert.ToInt32(linea[0]), linea[1]);

                    }

                }
            }
        }
        void LeerTransiciones()
        {
            //usar el diccionario transiciones y llenarlo
        }
        void limpiar()
        {
            Transiciones.Clear();
            Gramatica.Clear();
            GramaticaLetras.Clear();
            PilaLetras.Clear();
            PilaNumeros.Clear();
        }

        public void AnalisisSintactico(Stack<string> Tokens)
        {
            LeerGramatica();
            //llamar LeerTransiciones
            //Llamar metodo para rellenar gramatica
            int EstadoActual = 0;
            int CantidadSimbolos = 0;
            string LlaveDiccionario = "";
            /*Estado 1 = ir a
             * Estado 2 = desplazar
             * Estado 3=  reducir
             * Estado 4 =aceptar
            */            
            PilaNumeros.Enqueue(0);
            LlaveDiccionario = PilaNumeros.Peek() + Tokens.Peek();
            var Siguiente = Transiciones[LlaveDiccionario];
            EstadoActual =Siguiente[0];
          


            while(EstadoActual != 5)
            {
                if(EstadoActual == 1)
                {
                    //ir a

                    PilaNumeros.Enqueue(Siguiente[1]);
                    LlaveDiccionario = PilaNumeros.Peek() + Tokens.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {
                        Siguiente = Transiciones[LlaveDiccionario];
                        EstadoActual = Siguiente[0];
                    }
                    else
                    {
                        EstadoActual = 6;
                    }

                }
                else if(EstadoActual ==2)
                {
                    //dessplazar
                   
                    PilaNumeros.Enqueue(Siguiente[1]);
                    PilaLetras.Enqueue(Tokens.Pop());
                   
                    LlaveDiccionario = PilaNumeros.Peek() + Tokens.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {


                        Siguiente = Transiciones[LlaveDiccionario];
                        EstadoActual = Siguiente[0];
                    }
                    else
                    {
                        EstadoActual = 6;
                    }

                }
                else if (EstadoActual == 3)
                {
                    // reducir
                    CantidadSimbolos = Gramatica[Siguiente[1]];
                    if (CantidadSimbolos != 0)
                    {
                        for (int i = 0; i < CantidadSimbolos; i++)
                        {
                            PilaNumeros.Dequeue();
                            PilaLetras.Dequeue();
                        }
                    }

                    PilaLetras.Enqueue(GramaticaLetras[Siguiente[1]]);
                    LlaveDiccionario = PilaNumeros.Peek() + PilaLetras.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {
                        Siguiente = Transiciones[LlaveDiccionario];
                        EstadoActual = Siguiente[0];
                    }
                    else
                    {
                        EstadoActual = 6;
                    }




                }
                else if(EstadoActual == 4)
                {
                    EstadoActual = 5;
                }
                else
                {
                    //error
                }
            }


        }



    }
}
