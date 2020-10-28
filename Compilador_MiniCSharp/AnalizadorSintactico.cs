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
        Stack<int> PilaNumeros = new Stack<int>();
        Stack<string> PilaLetras = new Stack<string>();
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

        public void AnalisisSintactico(Queue<Token> Tokens)
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
          
            PilaNumeros.Push(0);
            LlaveDiccionario = PilaNumeros.Peek() + "_" + Tokens.Peek().Palabra;
            var Siguiente = Transiciones[LlaveDiccionario];
            EstadoActual = Siguiente[0].Substring(0,1);



            while (EstadoActual != "ACC")
            {
                if (EstadoActual == "1" || EstadoActual == "2" || EstadoActual == "3" || EstadoActual == "4" || EstadoActual == "5"
                    || EstadoActual == "6" || EstadoActual == "7" || EstadoActual == "8" || EstadoActual == "9")  
                {
                    //ir a

                    PilaNumeros.Push(Convert.ToInt32(Siguiente[0]));
                    LlaveDiccionario = PilaNumeros.Peek() + "_" + Tokens.Peek().Palabra;
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {
                        Siguiente = Transiciones[LlaveDiccionario];
                        if (Siguiente.Length == 1)
                        {
                            if (Siguiente[0] != "")
                            {


                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            else
                            {
                                EstadoActual = "Error";
                            }
                        }
                        else
                        {
                            if(Tokens.Peek().Palabra != "else")
                            {
                                Siguiente[0] = Siguiente[1];
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            else
                            {
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            //conflicto
                        }
                    }
                    else
                    {
                        EstadoActual = "Error";

                    }

                }
                else if (EstadoActual == "d")
                {
                    //dessplazar

                    PilaNumeros.Push(Convert.ToInt32(Siguiente[0].Substring(1,Siguiente[0].Length-1)));
                    PilaLetras.Push(Tokens.Dequeue().Palabra);

                    LlaveDiccionario = PilaNumeros.Peek() + "_" + Tokens.Peek().Palabra;
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {


                        Siguiente = Transiciones[LlaveDiccionario];

                        if (Siguiente.Length == 1)
                        {
                            if (Siguiente[0] != "")
                            {
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            else
                            {
                                EstadoActual = "Error";
                            }
                        }
                        else
                        {


                            //conflicto
                            if (Tokens.Peek().Palabra != "else")
                            {
                                Siguiente[0] = Siguiente[1];
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            else
                            {
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                        }
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
                            PilaNumeros.Pop();
                            PilaLetras.Pop();
                        }
                    }

                    PilaLetras.Push(GramaticaLetras[Convert.ToInt32(Siguiente[0].Substring(1, Siguiente[0].Length - 1))]);
                    LlaveDiccionario = PilaNumeros.Peek() + "_" + PilaLetras.Peek();
                    if (Transiciones.ContainsKey(LlaveDiccionario))
                    {
                        Siguiente = Transiciones[LlaveDiccionario];
                        if (Siguiente.Length == 1)
                        {
                            if (Siguiente[0] != "")
                            {
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            else
                            {
                                EstadoActual = "Error";
                            }
                        }
                        else
                        {
                            //conflicto
                            if (Tokens.Peek().Palabra != "else")
                            {
                                Siguiente[0] = Siguiente[1];
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                            else
                            {
                                EstadoActual = Siguiente[0].Substring(0, 1);
                            }
                        }
                    }
                    else
                    {
                        EstadoActual = "Error";
                    }
                }
                else if (EstadoActual == "a")
                {
                   
                    EstadoActual = "ACC";
                    Console.WriteLine("El analisis sintactico se completo con exito");
                }
                else
                {
                    Console.WriteLine(LlaveDiccionario);
                    Console.WriteLine("Error en linea"+ Tokens.Peek().Linea +"Error en token: " + Tokens.Peek().Palabra);

                    return;
                }
            }

            limpiar();
            return;
        }



    }
}
