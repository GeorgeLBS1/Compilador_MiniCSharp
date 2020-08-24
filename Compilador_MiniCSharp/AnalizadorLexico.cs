using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel.Design;
using System.Linq;

namespace Compilador_MiniCSharp
{
    public class AnalizadorLexico
    {
        public static List<string> Comentarios = new List<string>(); //Se guardan los comentarios
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

            string Linea = string.Empty; //Variable para ayudar a la lectura del archivo linea a linea
            bool CierreComentario; //verifica si existe cierre de comentario
            int ContadorLineas = 0;

            while ((Linea = reader.ReadLine()) != null) //Leer todas las lineas del archivo 
            {
                CierreComentario = false;
                if (!Linea.Contains("/*")) //No trae comentario
                {
                    if (Linea.Equals(""))
                    {
                        ContadorLineas++;
                    }
                    else
                    {
                        Linea = Linea.Replace("\t", " "); //cambiar tabulaciones por espacio
                        Analisis(Linea, ContadorLineas);   //enviar al analizador
                    }

                }
                else
                {
                    if (!Linea.Contains("*/"))
                    {
                        Linea = Linea.Replace("\t", " ");
                        var Divisior = Linea.Split("/*", StringSplitOptions.RemoveEmptyEntries);
                        bool comprobar = false;
                        if (Divisior.Length > 1)
                        {
                            comprobar = true;
                        }
                        if (comprobar == true)
                        {
                            Analisis(Divisior[0], ContadorLineas);
                            Comentarios.Add("/*" + Divisior[1]);
                            Linea = reader.ReadLine();
                            ContadorLineas++;

                        }

                        while (!Linea.Contains("*/"))
                        {

                            Comentarios.Add(Linea);
                            Linea = reader.ReadLine();
                            ContadorLineas++;

                            if (reader.EndOfStream && Linea == null)
                            {
                                CierreComentario = true;
                                break;

                            }


                        }

                    }
                    if (CierreComentario == false)
                    {
                        Linea = Linea.Replace("\t", " ");
                        var DividirComentario = Linea.Split("*/", StringSplitOptions.RemoveEmptyEntries);
                        if (DividirComentario.Length == 0)
                        {
                            Comentarios.Add("*/");
                        }
                        if (DividirComentario.Length == 1)
                        {
                            if (DividirComentario[0].Contains("/*"))
                            {
                                var Divisor = DividirComentario[0].Split("/*", StringSplitOptions.RemoveEmptyEntries);
                                Analisis(Divisor[0], ContadorLineas);
                                Comentarios.Add("/*" + Divisor[1] + "*/");
                            }
                            else
                            {


                                Comentarios.Add(DividirComentario[0] + "*/");
                            }
                        }

                        if (DividirComentario.Length > 1)
                        {

                            if (DividirComentario[0].Contains("/*"))
                            {
                                var Divisor = DividirComentario[0].Split("/*", StringSplitOptions.RemoveEmptyEntries);
                                if (Divisor.Length > 1)
                                {
                                    Analisis(Divisor[0], ContadorLineas);

                                    if (DividirComentario.Length > 1)
                                    {
                                        Analisis(DividirComentario[1], ContadorLineas);
                                        Comentarios.Add(Divisor[1] + "*/");
                                    }
                                    else
                                    {
                                        Comentarios.Add(Divisor[1] + DividirComentario[1] + "*/");
                                    }

                                }


                            }
                            else
                            {
                                Comentarios.Add(DividirComentario[0] + "*/");
                                if (!DividirComentario[1].Trim(' ').Equals(""))
                                {
                                    Analisis(DividirComentario[1], ContadorLineas);
                                }
                            }
                        }


                    }
                }

            }
            reader.Close();
        }
        public void Analisis(string linea, int x)
        {

        }

    }
}
