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
        Dictionary<string, string[]> Transiciones = new Dictionary<string, string[]>();
        Queue<int> PilaNumeros = new Queue<int>();
        Queue<string> PilaLetras = new Queue<string>();
        void LeerGramatica()
        {
            string[] linea;
            string path = Environment.CurrentDirectory;
            var xx = path.Split(@"bin");
            string ruta = xx[0] + "Gcaracter.txt";
            using (var file = new FileStream(ruta, FileMode.Open))
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
            string path2 = Environment.CurrentDirectory;
            var xx2 = path2.Split(@"bin");
            string ruta2 = xx2[0] + "GSLetras.txt";
            using (var file = new FileStream(ruta2, FileMode.Open))
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

            LecturaGramatica modelo = new LecturaGramatica();
            string path = Environment.CurrentDirectory;
            var xx = path.Split(@"bin");
            string ruta = xx[0] + "Gramatica.csv";
            Transiciones = modelo.Leer(ruta);

            string EstadoActual = "";
            int CantidadSimbolos = 0;
            string LlaveDiccionario = "";
            /*Estado 1 = ir a
             * Estado 2 = desplazar
             * Estado 3=  reducir
             * Estado 4 =aceptar
            */
            PilaNumeros.Enqueue(0);
            LlaveDiccionario = PilaNumeros.Peek() + "_" + Tokens.Peek();
            var Siguiente = Transiciones[LlaveDiccionario];
            EstadoActual = Siguiente[0].Substring(0,1);



            while (EstadoActual != "ACC")
            {
                if (EstadoActual == "1" || EstadoActual == "2" || EstadoActual == "3" || EstadoActual == "4" || EstadoActual == "5"
                    || EstadoActual == "6" || EstadoActual == "7" || EstadoActual == "8" || EstadoActual == "9")  
                {
                    //ir a

                    PilaNumeros.Enqueue(Convert.ToInt32(Siguiente[0]));
                    LlaveDiccionario = PilaNumeros.Peek() + "_" + Tokens.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {
                        Siguiente = Transiciones[LlaveDiccionario];
                        EstadoActual =  Siguiente[0].Substring(0,1);
                    }
                    else
                    {
                        EstadoActual = "Error";
                    }

                }
                else if (EstadoActual == "d")
                {
                    //dessplazar

                    PilaNumeros.Enqueue(Convert.ToInt32(Siguiente[0].Substring(1,Siguiente[0].Length-1)));
                    PilaLetras.Enqueue(Tokens.Pop());

                    LlaveDiccionario = PilaNumeros.Peek() + "_" + Tokens.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {


                        Siguiente = Transiciones[LlaveDiccionario];
                        EstadoActual = Siguiente[0];
                    }
                    else
                    {
                        EstadoActual = "Error";
                    }

                }
                else if (EstadoActual == "r")
                {
                    // reducir
                    CantidadSimbolos = Gramatica[Convert.ToInt32(Siguiente[0].Substring(1, Siguiente[0].Length - 1))];
                    if (CantidadSimbolos != 0)
                    {
                        for (int i = 0; i < CantidadSimbolos; i++)
                        {
                            PilaNumeros.Dequeue();
                            PilaLetras.Dequeue();
                        }
                    }

                    PilaLetras.Enqueue(GramaticaLetras[Convert.ToInt32(Siguiente[0].Substring(1, Siguiente[0].Length - 1))]);
                    LlaveDiccionario = PilaNumeros.Peek() + "_" + PilaLetras.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {
                        Siguiente = Transiciones[LlaveDiccionario];
                        EstadoActual = Siguiente[0].Substring(0,1);
                    }
                    else
                    {
                        EstadoActual = "Error";
                    }
                }
                else if (EstadoActual == "a")
                {
                    EstadoActual = "ACC";
                }
                else
                {
                    Console.WriteLine("Error en token: " + Tokens.Peek());
                    return;
                }
            }


        }



    }
}
