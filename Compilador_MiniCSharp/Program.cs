using System;

namespace Compilador_MiniCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese la ruta del archivo");
            Console.WriteLine("");
            string archivo = Console.ReadLine();

            archivo = archivo.Trim('"');
            AnalizadorLexico analizador = new AnalizadorLexico();
            analizador.LeerArchivo(archivo);
            Console.ReadKey();
        }
    }
}
