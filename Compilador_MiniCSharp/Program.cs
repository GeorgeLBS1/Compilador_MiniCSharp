using MiniC;
using System;
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
            //obtener la ruta
            string path = Environment.CurrentDirectory;
            var xx = path.Split(@"bin");
            string ruta = xx[0]+"Gramatica.csv";

            LecturaGramatica lg = new LecturaGramatica();
            lg.Leer(ruta);

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
