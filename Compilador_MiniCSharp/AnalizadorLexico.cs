using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace Compilador_MiniCSharp
{
    public class AnalizadorLexico
    {
        List<string> ER = new List<string>() //Lista que contiene todas las expresiones regulares a usar para analizar el archivo y la sintaxis de este
        {
            @"^void$|^int$|^double$|^bool$|^string$|^class$|^const$|^interface$|^null$|^this$|^for$|^while$|^foreach$|^if$|^else$|^return$|^break$|^New$|^NewArray$|^Console$|^WriteLine$", //Palabras reservadas, 0
            @"^(true|false)$", //Constantes bool, 1
            @"^\d+((\.)(E\+|e\+)?\d+)?$", //Constantes double incluido notación exponencial, 2
            @"^[0]([x]|[X])((\d([a-f]|\d)*)|([a-f]([a-f]|\d)+))$", //Constantes Enteros incluidos el hexadecimal, 3         
            @"[+]|[-]|[*]|[%]|^<$|^<=$|^>$|^>=$|^=$|^==$|^!=$|&&|\|\||[!]|[;]|[,]|[.]|^\[\s+\]$|^\(\s+\)$|^\{\s+\}$|^\[\]$|^\(\)$|^\{\}$", //Operadores y signos de puntuación, 4
            @"^[A-z|$]([A-z0-9$]){0,30}$", //Identificadores de largo máximo = 30, 5
            "^\".+\"$", //String, 6
            @"^[\/][*](.+|\s)+[*][\/]$", //Comentarios multilinea, 7
            @"^[\/][\/].+$" //Comentarios linea simple, 8
            
        };
        public int AnalizarLexema(string lexema) //Método para analizar cada lexema individualmente
        {
            foreach (var item in ER)
            {
                if (Regex.IsMatch(lexema, item)) //Si encaja el lexema con cualquier expresión regular en la lista devolver el número de la lista
                {
                    return ER.FindIndex(x => x == item); //Retorna el índice de la expresión regular que hizo match con el lexema ingresado.
                }
            }
            return -100; //Valor negativo asignado si no concuerda ninguna expresión regular
        }

        public void LeerArchivo(string path) //Lectura del archivo completo
        {
            StreamReader reader = new StreamReader(path);
            List<string> ArchivoDepurado = new List<string>(); //Archivo que no contendrá tabulaciones 
            List<string> ArchivoTalCual = new List<string>(); //Archivo como existe a nivel lógico, sin ninguna edición
            string Linea = string.Empty; //Variable para ayudar a la lectura del archivo linea a linea

            while ((Linea = reader.ReadLine()) != null) //Leer todas las lineas del archivo 
            {
                ArchivoTalCual.Add(Linea); //Añadir linea nítida
                Linea = Linea.Replace("\t",""); //Se quitan las tabulaciones
                ArchivoDepurado.Add(Linea); //Se añade al archivo depurado
            }
            reader.Close();
        }

    }
}
