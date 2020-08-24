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
        readonly List<string> ER = new List<string>() //Lista que contiene todas las expresiones regulares a usar para analizar el archivo y la sintaxis de este
        {
            @"^void$|^int$|^double$|^bool$|^string$|^class$|^const$|^interface$|^null$|^this$|^for$|^while$|^foreach$|^if$|^else$|^return$|^break$|^New$|^NewArray$|^Console$|^WriteLine$", //Palabras reservadas, 0
            @"^(true|false)$", //Constantes bool, 1
            @"^\d+((\.)(E\+|e\+)?\d+)?$", //Constantes double incluido notación exponencial, 2
            @"^[0]([x]|[X])((\d([a-f]|\d)*)|([a-f]([a-f]|\d)+))$", //Constantes Enteros incluidos el hexadecimal, 3         
            @"^\+$|^\-$|^\/$|^\*$|^\%$|^\<$|^\<=$|^\>$|^\>\=$|^\=$|^\=\=$|^\!=$|^\&\&$|^\|\|$|^\!$|^\;$|^\,$|^\.$|^\[\]$|^\[$|^\]|^\(\)$|^\{$|^\}$|^\{\}$|^\($|^\)$", //Operadores y signos de puntuación, 4
            @"^[A-z|$]([A-z0-9$]){0,30}$", //Identificadores de largo máximo = 30, 5
            "^\".+\"$", //String, 6
            @"^[\/][\/].+$", //Comentarios linea simple, 7
            @"^\s+$", // espacio vacío o secuencia de espacios vacíos, 8
            "^\"[^\"]*$", // Posible Secuencia De String, 9
            @"^[0]([x]|[X])", //Posible secuencia de número hexadecimales, 10
            @"^\d+(\.)$", //Posible secuencia double, 11
            @"^\d+(\.)(E\+|e\+)$", //Posible secuencia double, 12
            @"^\d+(\.)(E|e)" //Posible secuencia double, 13
        };
        public List<Token> ListaDeTokens = new List<Token>(); //Se listan todos los tokens encontrados en el archivo
        public static string ruta = string.Empty; //Ruta del archivo para generar el de salida
        int AnalizarLexema(string lexema) //Método para analizar cada lexema individualmente
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
            ruta = path;
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
                        ContadorLineas++;
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
                        ContadorLineas--;

                    }
                    if (CierreComentario == false)
                    {
                        ContadorLineas++;
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
        public void Analisis(string linea, int NoLinea)
        {
            string lexema = string.Empty;
            string InicioDePalabra = string.Empty;
            for (int i = 0; i <= linea.Length; i++) //Recorrer la línea del archivo como tal
            {
                if (i != linea.Length)
                {
                    lexema += linea[i];
                }

                int No_Token = AnalizarLexema(lexema);
                if (No_Token == -100 || i == linea.Length) //si el token no se reconoce
                {
                    string aux = string.Empty;
                    if (lexema.Length > 1 && i <= linea.Length - 1) //solo si el largo de la cadena es mayor a 1 y si la secuencia no compone un caracter completo
                    {
                        aux = lexema.Remove(lexema.Length - 1); //volverlo a reconocer sin el último caracter   

                        No_Token = AnalizarLexema(aux); //asignar el codigo del lexema encontrado o reconocido al número de token
                        Token token = new Token(aux, InicioDePalabra.Length, InicioDePalabra.Length + aux.Length - 1, NoLinea, No_Token); //Crear en sí el token

                        //Escribir en el archivo de salida de texto la información del token
                        EscribirEnArchivo(Path.GetFileNameWithoutExtension(ruta), token);


                        if (token.Tipo_token != 8 && token.Tipo_token != -100) //Para la tabla de símbolos se guardan todos los tokens que SÍ fueron RECONOCIDOS
                        {
                            ListaDeTokens.Add(token);
                        }
                        InicioDePalabra += aux;
                        lexema = Convert.ToString(lexema[^1]);
                    }
                    else //Esto se realiza si ya es el último caracter o es un caracter singular
                    {
                        aux = lexema;
                        Token token = new Token(aux, InicioDePalabra.Length, InicioDePalabra.Length + aux.Length - 1, NoLinea, No_Token); //Crear en sí el token
                        EscribirEnArchivo(Path.GetFileNameWithoutExtension(ruta), token);
                        if (token.Tipo_token != 8 && token.Tipo_token != -100) //Para la tabla de símbolos se guardan todos los tokens que SÍ fueron RECONOCIDOS
                        {
                            ListaDeTokens.Add(token);
                        }
                        InicioDePalabra = aux;
                        lexema = string.Empty;
                    }

                }
            }
            var f = ListaDeTokens.Count;
        }
        void EscribirEnArchivo(string ruta, Token token)
        {
            StreamWriter writer = new StreamWriter(ruta + ".out", true);
            string Tipo_token;
            switch (token.Tipo_token)
            {
                case 0:
                    Tipo_token = "T_Palabra_reservada";
                    break;
                case 1:
                    Tipo_token = "T_Constante_booleana";
                    break;
                case 2:
                    Tipo_token = "T_Constante_double";
                    break;
                case 3:
                    Tipo_token = "T_Constante_entero";
                    break;
                case 4:
                    Tipo_token = "T_Operador";
                    break;
                case 5:
                    Tipo_token = "T_Identificador";
                    break;
                case 6:
                    Tipo_token = "T_String";
                    break;
                case 7:
                    Tipo_token = "Comentario de linea simple";
                    break;
                default:
                    Tipo_token = "ERROR Token no reconocido";
                    break;
            }
            if (token.Tipo_token != 8)
            {
                writer.WriteLine(token.Palabra + @"         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token);
            }
            writer.Close();
        }
    }
}
