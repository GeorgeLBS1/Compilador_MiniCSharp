using System;

namespace Compilador_MiniCSharp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Ingrese la ruta del archivo");
            string archivo = Console.ReadLine();

            archivo = archivo.Trim('"');
            AnalizadorLexico analizador = new AnalizadorLexico();
            analizador.LeerArchivo(archivo);
            
            //int i = analizador.AnalizarLexema("Hola");
            //Console.WriteLine(Convert.ToString(i));
        }
    }
}
