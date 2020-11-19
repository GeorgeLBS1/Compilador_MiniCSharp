using MiniC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices.ComTypes;
using System.Threading;
namespace Compilador_MiniCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
           
     

            Console.BackgroundColor = ConsoleColor.DarkBlue;
            Console.WriteLine(@"  __  __ _       _  _____  _  _      _____                      _ _           _            ");
            Thread.Sleep(250);
            Console.WriteLine(@" |  \/  (_)     (_)/ ____|| || |_   / ____|                    (_) |         | |           ");
            Thread.Sleep(250);
            Console.WriteLine(@" | \  / |_ _ __  _| |   |_  __  _| | |     ___  _ __ ___  _ __  _| | __ _  __| | ___  _ __ ");
            Thread.Sleep(250);
            Console.WriteLine(@" | |\/| | | '_ \| | |    _| || |_  | |    / _ \| '_ ` _ \| '_ \| | |/ _` |/ _` |/ _ \| '__|");
            Thread.Sleep(250);
            Console.WriteLine(@" | |  | | | | | | | |___|_  __  _| | |___| (_) | | | | | | |_) | | | (_| | (_| | (_) | |   ");
            Thread.Sleep(250);
            Console.WriteLine(@" |_|  |_|_|_| |_|_|\_____||_||_|    \_____\___/|_| |_| |_| .__/|_|_|\__,_|\__,_|\___/|_|   ");
            Thread.Sleep(250);
            Console.WriteLine(@"                                                         | |                               ");
            Thread.Sleep(250);
            Console.WriteLine(@"                                                         |_|                               ");
            Thread.Sleep(250);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Bienvenid@");
            Console.WriteLine("Ingrese la ruta del archivo o arrastre el archivo a la consola");
            Console.WriteLine("");
            string archivo = Console.ReadLine();

            archivo = archivo.Trim('"');
            if (archivo != "")
            {
                AnalizadorLexico analizador = new AnalizadorLexico();
                analizador.LeerArchivo(archivo);
                if (analizador.Errores == false)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Archivo generado con éxito, 0 errores");
                    Console.ForegroundColor = ConsoleColor.White;
                    Queue<Token> ColaTokens = new Queue<Token>();
                    Queue<Token> ColaTokensSemantica = new Queue<Token>();
                    Stack<Token> Cola = new Stack<Token>();
                    List<Token> lista = analizador.ListaDeTokens;

                    foreach (var item in analizador.ListaDeTokens)
                    {
                        ColaTokensSemantica.Enqueue(item);
                    }
                    foreach (var item in analizador.ListaDeTokens)
                    {
                       

                        if (item.Tipo_token == 5)
                        {
                            Token temp = new Token("ident", item.CInicio, item.CFinal, item.Linea, item.Tipo_token);
                            ColaTokens.Enqueue(temp);
                        }
                        else if (item.Tipo_token == 3)
                        {
                            Token temp = new Token("intConstant", item.CInicio, item.CFinal, item.Linea, item.Tipo_token);
                            ColaTokens.Enqueue(temp);
                        }
                        else if (item.Tipo_token == 1)
                        {
                            Token temp = new Token("boolConstant", item.CInicio, item.CFinal, item.Linea, item.Tipo_token);
                            ColaTokens.Enqueue(temp);
                        }
                         else  if (item.Tipo_token == 2)
                        {
                            Token temp = new Token("doubleConstant", item.CInicio, item.CFinal, item.Linea, item.Tipo_token);
                            ColaTokens.Enqueue(temp);
                        }
                        else if (item.Tipo_token == 6)
                        {
                            Token temp = new Token("stringConstant", item.CInicio, item.CFinal, item.Linea, item.Tipo_token);
                            ColaTokens.Enqueue(temp);

                        }
                        else
                        {
                            Token temp = new Token(item.Palabra, item.CInicio, item.CFinal, item.Linea, item.Tipo_token);
                            ColaTokens.Enqueue(temp);
                        }
                        
                       

                     

                    }
                    Token dolar = new Token("$", 0, 0, 0, 0);
                    ColaTokens.Enqueue(dolar);

                    //foreach(var item in ColaTokens)
                    //{
                    //    Cola.Push(item);
                    //}
                    
                    
                    AnalizadorSintactico modelo = new AnalizadorSintactico();
                    modelo.AnalisisSintactico(ColaTokens);
                    AnalizadorSemantico nuevo = new AnalizadorSemantico();
                     nuevo.Analizador(ColaTokensSemantica);
                    
                }
                else
                {
                    Console.WriteLine($"Cantidad de errores: {analizador.CantidadErrores}");
                    Console.WriteLine("Archivo generado");
                }
                Console.ReadKey();
            }
            else
            {
                Console.WriteLine("Ruta no válida");
            }
            
        }
    }
}
