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
        public bool Errores = false;
        public int CantidadErrores = 0;
        public static List<string> Comentarios = new List<string>(); //Se guardan los comentarios
        readonly List<string> ER = new List<string>() //Lista que contiene todas las expresiones regulares a usar para analizar el archivo y la sintaxis de este
        {
            @"^void$|^int$|^double$|^bool$|^string$|^class$|^const$|^interface$|^null$|^this$|^for$|^while$|^foreach$|^if$|^else$|^return$|^break$|^New$|^NewArray$|^Console$|^WriteLine$", //Palabras reservadas, 0
            @"^(true|false)$", //Constantes bool, 1
            @"^\d+((\.)(E(\+|[-]|)|e(\+|[-]|))?\d+)?$", //Constantes double incluido notación exponencial, 2
            @"^[0]([x]|[X])(((\d|(([a-f]|[A-F])|\d)*)|([a-f]([a-f]|\d)+)))*$", //Constantes Enteros incluidos el hexadecimal, 3         
            @"^\+$|^\-$|^\/$|^\*$|^\%$|^\<$|^\<=$|^\>$|^\>\=$|^\=$|^\=\=$|^\!=$|^\&\&$|^\|\|$|^\!$|^\;$|^\,$|^\.$|^\[\]$|^\[$|^\]|^\(\)$|^\{$|^\}$|^\{\}$|^\($|^\)$", //Operadores y signos de puntuación, 4
            @"^[A-z|$]([A-z0-9$]){0,29}$", //Identificadores de largo máximo = 30, 5
            "^\".+\"$", //String, 6
            @"^[\/][\/].+$", //Comentarios linea simple, 7
            @"^\s+$", // espacio vacío o secuencia de espacios vacíos, 8
            "^\"[^\"]*$", // Posible Secuencia De String, 9
            @"^[0]([x]|[X])$", //Posible secuencia de número hexadecimales, 10
            @"^\d+(\.)$", //Posible secuencia double, 11
            @"^\d+(\.)(E(\+|[-]|)|e(\+|[-]|))$", //Posible secuencia double, 12
            @"^\d+(\.)(E|e)$", //Posible secuencia double, 13
            @"^[\/][\/]$", //Posibles Comentarios linea simple, 14
            @"^[A-z|$]([A-z0-9$]){30,}$", //Identificadores si se pasa de 31 para manejo de error, 15
            @"^\*?\/$", //Cierre de comentario sin abrir previamente comentario, 16
            @"^(\d)+$",//numeros enteros positivos, 17
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
            bool CierreComentario =false; //verifica si existe cierre de comentario
            int ContadorLineas = 0;
            string TextoAnalisis = "";
            string _comentario = "";

            while ((Linea = reader.ReadLine()) != null) //Leer todas las lineas del archivo 
            {
                ContadorLineas++;
                Linea = Linea.Replace("\t", " ");
                if (!Linea.Contains("/*") && !CierreComentario) //No trae comentario
                {
                    if (Linea.Equals(""))
                    {

                    }
                    else
                    {
                        //cambiar tabulaciones por espacio
                        Analisis(Linea, ContadorLineas);   //enviar al analizador

                    }

                }
                else //si viene comentario multilinea
                {
                    string comparación = "";
                    for (int i = 0; i < Linea.Length; i++) 
                    {
                        if (i + 1 != Linea.Length)  //concatena para verificar los simbolos de cierre o apertura de comentario
                        {
                            comparación = Convert.ToString(Linea[i]) + Convert.ToString(Linea[i + 1]);
                        }

                        if (!CierreComentario && !comparación.Equals("/*"))  // si hay codigo antes del comentario
                        {

                            TextoAnalisis += Convert.ToString(Linea[i]);

                        }
                        else if (!CierreComentario && comparación.Equals("/*"))  // inicio del comentario
                        {
                            CierreComentario = true;
                            if (!TextoAnalisis.Equals("")) // confirma si existe codigo para enviarlo al analisis
                            {
                                Analisis(TextoAnalisis, ContadorLineas);
                            }
                            TextoAnalisis = "";
                            _comentario += Convert.ToString(Linea[i]) + Convert.ToString(Linea[i + 1]);
                            i++;
                        }
                        else if (CierreComentario && !comparación.Equals("*/"))  // lo que dure el comentario se acumula
                        {

                            _comentario += Convert.ToString(Linea[i]);


                        }
                        else if (CierreComentario && comparación.Equals("*/"))  //cuando se cierra el comentario
                        {

                            _comentario += Convert.ToString(Linea[i]) + Convert.ToString(Linea[i + 1]);
                            i++;
                            Comentarios.Add(_comentario);
                            _comentario = "";
                            CierreComentario = false;

                        }
                        else
                        { }
                        if (i + 1 == Linea.Length && (!_comentario.Equals("") || !TextoAnalisis.Equals("")))  //si hay salto de linea y el comentario no ha terminado o queda algo de codiog
                        {
                            if (CierreComentario && !_comentario.Equals(""))  // comentario activo y comentario no vacio
                            {
                                _comentario += Convert.ToString(Linea[i]);
                                Comentarios.Add(_comentario);
                                _comentario = "";
                            }
                            if (!CierreComentario && !TextoAnalisis.Equals(""))  // comentario cerrado pero existe codigo despues del comentario
                            {
                                TextoAnalisis += Convert.ToString(Linea[i]);
                                Analisis(TextoAnalisis, ContadorLineas);
                                TextoAnalisis = "";
                            }
                        }


                    }
                }
                    
             }
            if (CierreComentario)  // si no se cerro el comentario existe error EOF
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.WriteLine("**********ERROR EOF, no se cerro comentario**********");
                Console.BackgroundColor = ConsoleColor.Black;
            }
            reader.Close();
        }
        public void Analisis(string linea, int NoLinea)
        {
            string lexema = string.Empty;
            string InicioDePalabra = string.Empty;
            if (linea != "\n") //Si la linea es diferente a una linea vacía
            {
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
                            Token token = new Token(aux, InicioDePalabra.Length, InicioDePalabra.Length + aux.Length, NoLinea, No_Token); //Crear en sí el token

                            //Escribir en el archivo de salida de texto la información del token
                            EscribirEnArchivo(Path.GetFileNameWithoutExtension(ruta), token);


                            if (token.Tipo_token != 8 && token.Tipo_token != -100 && token.Tipo_token != 7) //Para la tabla de símbolos se guardan todos los tokens que SÍ fueron RECONOCIDOS
                            {
                                ListaDeTokens.Add(token);
                            }
                            InicioDePalabra += aux;
                            lexema = Convert.ToString(lexema[^1]);
                        }
                        else //Esto se realiza si ya es el último caracter o es un caracter singular
                        {
                            aux = lexema;
                            Token token = new Token(aux, InicioDePalabra.Length, InicioDePalabra.Length + aux.Length, NoLinea, No_Token); //Crear en sí el token
                            EscribirEnArchivo(Path.GetFileNameWithoutExtension(ruta), token);
                            if (token.Tipo_token != 8 && token.Tipo_token != -100 && token.Tipo_token != 7) //Para la tabla de símbolos se guardan todos los tokens que SÍ fueron RECONOCIDOS
                            {
                                ListaDeTokens.Add(token);
                            }
                            InicioDePalabra = aux;
                            lexema = string.Empty;
                        }

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
                    if (Regex.IsMatch(token.Palabra, ER[17])) //Si encaja el lexema con cualquier expresión regular en la lista devolver el número de la lista
                    {
                        token.Tipo_token = 3;
                        Tipo_token = "T_Constante_entero";
                    }
                    else
                    {
                        Tipo_token = "T_Constante_double";
                    }                    
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
                    Tipo_token = "Token NO RECONOCIDO";
                    break;
            }
            if (token.Tipo_token != 8)
            {
                if (Tipo_token == "Token NO RECONOCIDO" && token.Tipo_token != 15)
                {
                    if (Regex.IsMatch(token.Palabra, ER[9]) == true && token.Palabra.Length > 1)
                    {
                        writer.WriteLine(token.Palabra + @" ******ERROR, posiblemente falten comillas de cierre.         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                        writer.WriteLine("");
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine(token.Palabra + @" ******ERROR, posiblemente falten comillas de cierre.         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine("");
                        Errores = true;
                        CantidadErrores++;
                    }
                    else
                    {
                        writer.WriteLine(token.Palabra + @" ******ERROR         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                        writer.WriteLine("");
                        Console.BackgroundColor = ConsoleColor.Red;
                        Console.WriteLine(token.Palabra + @" ******ERROR         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                        Console.BackgroundColor = ConsoleColor.Black;
                        Console.WriteLine("");
                        Errores = true;
                        CantidadErrores++;
                    }
                    
                    
                }
                else if (token.Tipo_token == 15) //Cuando se excede un identificador y se usan más de 30 caracteres
                {
                    token.Palabra = CortarID(token.Palabra);
                    token.Tipo_token = 5;
                    Tipo_token = "T_Identificador";
                    writer.WriteLine(token.Palabra + @" ******ERROR, el identificador excede la cantidad de caracteres que puede tener. Se conservarán solo los primeros 30.         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                    writer.WriteLine("");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(token.Palabra + @" ******ERROR, el identificador excede la cantidad de caracteres que puede tener. Se conservarán solo los primeros 30.       Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("");
                    Errores = true;
                    CantidadErrores++;
                }
                else if (token.Tipo_token == 16) //advertencia de signo de cierre de comentario sin abrir. */
                {
                    Tipo_token = "Comentario cerrado pero nunca abierto";
                    writer.WriteLine(token.Palabra + @" ******ERROR         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                    writer.WriteLine("");
                    Console.BackgroundColor = ConsoleColor.Red;
                    Console.WriteLine(token.Palabra + @" ******ERROR         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token + "*********");
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("");
                    Errores = true;
                    CantidadErrores++;
                }
                else if (token.Tipo_token != 7)
                {
                    writer.WriteLine(token.Palabra + @"         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token);
                    writer.WriteLine("");
                    //Console.WriteLine(token.Palabra + @"         Linea: " + token.Linea + ",     Columna: " + token.CInicio + "-" + token.CFinal + ",    ES: " + Tipo_token);
                    //Console.WriteLine("");
                }

            }
            writer.Close();
        }
        string CortarID(string id)
        {
            string salida = string.Empty;
            for (int i = 0; i < 30; i++)
            {
                salida += id[i];
            }
            var s = salida.Length;
            return salida;
        }
    }
}
