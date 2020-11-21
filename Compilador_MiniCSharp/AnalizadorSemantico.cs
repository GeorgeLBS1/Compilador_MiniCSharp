using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Bau.Libraries.LibExpressionParser;
using Bau.Libraries.LibExpressionParser.Parser.Expressions;
using Bau.Libraries.LibExpressionParser.Variables;
using Compilador_MiniCSharp;


namespace MiniC
{
    class AnalizadorSemantico
    {
        Dictionary<string, Tipos> clases = new Dictionary<string, Tipos>();
        Dictionary<string, Metodo> parametros = new Dictionary<string, Metodo>();
        Dictionary<string, Variable> VariablesGlobal = new Dictionary<string, Variable>();
        Dictionary<string, Intermedio> MetodosGlobal = new Dictionary<string, Intermedio>();
        List<TablaDeSimbolos> tablaDeSimbolos = new List<TablaDeSimbolos>();

        public void Analizador(Queue<Token> ListaToken, List<Token> lTokens)
        {
            Console.WriteLine("--------------------------------------------------------ANÁLISIS SEMÁNTICO-------------------------------------------------------------");
            while (ListaToken.Count != 0)
            {
                Token valorActual = ListaToken.Dequeue();

                if (valorActual.Palabra == "class")
                {

                    int contadorLLave = 0;
                    int ContadorLlaveMetodo = 0;
                    bool erroLlaves = false;
                    Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
                    Dictionary<string, Intermedio> Metodos = new Dictionary<string, Intermedio>();
                    Dictionary<string, Metodo> variablesMetodo = new Dictionary<string, Metodo>();
                    Token NombreClase = ListaToken.Dequeue();
                    if (ListaToken.Peek().Palabra != "{")
                    {
                        while (ListaToken.Peek().Palabra != "{")
                        {
                            ListaToken.Dequeue();
                            if (ListaToken.Count == 0)
                            {
                                //no exisite llave de apertura para la calse
                                erroLlaves = true;
                                break;
                            }
                        }
                    }
                    if (!erroLlaves)
                    {
                        contadorLLave++;
                        ListaToken.Dequeue();
                        while (contadorLLave != 0)
                        {
                            valorActual = ListaToken.Dequeue();
                            if (valorActual.Palabra == "void" || valorActual.Palabra == "int" || valorActual.Palabra == "bool" || valorActual.Palabra == "string" || valorActual.Palabra == "double" || valorActual.Tipo_token == 5)
                            {
                                string TipoDatoFV = valorActual.Palabra;
                                if (ListaToken.Peek().Palabra == "[]")
                                {
                                    TipoDatoFV += ListaToken.Dequeue().Palabra;
                                }
                                Token NombreMV = ListaToken.Dequeue();

                                if (ListaToken.Peek().Palabra == "(")
                                {
                                    ListaToken.Dequeue();

                                    //Variables por parametros
                                    while (ListaToken.Peek().Palabra != ")")
                                    {
                                        Metodo modelo = new Metodo();
                                        string TipoDato = ListaToken.Dequeue().Palabra;
                                        if (ListaToken.Peek().Palabra == "[]")
                                        {
                                            TipoDato += ListaToken.Dequeue().Palabra;
                                        }
                                        Token temp = ListaToken.Dequeue();
                                        modelo.TipoDato = TipoDato;
                                        modelo.CInicio = temp.CInicio;
                                        modelo.CFinal = temp.CFinal;
                                        modelo.Contexto = 1;
                                        modelo.Valor = "";
                                        modelo.Linea = temp.Linea;

                                        if (parametros.ContainsKey(temp.Palabra))
                                        {
                                            Console.WriteLine("Ya se ha declarado un parametro con el nombre: " + temp.Palabra + " dentro del metodo: "
                                                + NombreMV.Palabra + " error en linea: " + temp.Linea + " columna: " + temp.CInicio + "-" + temp.CFinal);
                                        }
                                        else
                                        {
                                            variablesMetodo.Add(temp.Palabra, modelo);
                                            tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, modelo.Valor));
                                        }
                                        if (ListaToken.Peek().Palabra == ",")
                                        {
                                            ListaToken.Dequeue();
                                        }
                                    }

                                    if (ListaToken.Peek().Palabra == ")")
                                    {
                                        ListaToken.Dequeue();

                                    }//variables dentro del metodo
                                    if (ListaToken.Peek().Palabra == "{")
                                    {
                                        ListaToken.Dequeue();
                                        ContadorLlaveMetodo = contadorLLave;
                                        ContadorLlaveMetodo++;
                                        while (ContadorLlaveMetodo != contadorLLave)
                                        {
                                            valorActual = ListaToken.Dequeue();
                                            if (valorActual.Palabra == "void" || valorActual.Palabra == "int" || valorActual.Palabra == "bool" || valorActual.Palabra == "string" || valorActual.Palabra == "double" || valorActual.Tipo_token == 5)
                                            {
                                                string TPV = valorActual.Palabra;
                                                if (ListaToken.Peek().Palabra == "[]")
                                                {
                                                    TPV += ListaToken.Dequeue().Palabra;
                                                }
                                                Token temp = ListaToken.Dequeue();
                                                if (temp.Tipo_token == 5)
                                                {

                                                    if (ListaToken.Peek().Palabra == ";")
                                                    {

                                                        Metodo modelo = new Metodo();

                                                        int indiceLista = lTokens.FindIndex(x => x == temp);

                                                        if (TPV == "string" || TPV == "int" || TPV == "double" || TPV == "bool")
                                                        {
                                                            modelo.TipoDato = TPV;
                                                        }
                                                        else
                                                        {
                                                            if (clases.TryGetValue(TPV, out Tipos valor) == true) //Si existe el tipo de dato en las clases
                                                            {
                                                                modelo.TipoDato = TPV;
                                                            }
                                                            else
                                                            {
                                                                var TipoDeDato = lTokens[indiceLista - 1]; //Retroceder uno para ver el tipo de dato
                                                                Console.ForegroundColor = ConsoleColor.Red;
                                                                Console.WriteLine("No existe el objeto de tipo" + TPV + " error en linea: " + TipoDeDato.Linea + " columna: " + TipoDeDato.CInicio + "-" + TipoDeDato.CFinal);
                                                                Console.ResetColor();
                                                            }

                                                        }

                                                        modelo.CInicio = temp.CInicio;
                                                        modelo.CFinal = temp.CFinal;
                                                        modelo.Contexto = 0;
                                                        modelo.Valor = "";
                                                        modelo.Linea = temp.Linea;

                                                        if (variablesMetodo.ContainsKey(temp.Palabra))
                                                        {
                                                            Console.ForegroundColor = ConsoleColor.Red;
                                                            Console.WriteLine("Ya se ha declarado un variable con el nombre: " + temp.Palabra + " dentro del metodo: "
                                                                      + NombreMV.Palabra + " error en linea: " + temp.Linea + " columna: " + temp.CInicio + "-" + temp.CFinal);
                                                            Console.ResetColor();
                                                        }
                                                        else
                                                        {
                                                            variablesMetodo.Add(temp.Palabra, modelo);
                                                            tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, modelo.Valor));
                                                        }
                                                        if (ListaToken.Peek().Palabra == ";")
                                                        {
                                                            ListaToken.Dequeue();
                                                        }


                                                    }

                                                }
                                                if (temp.Palabra == "=") //Si se produce una asignación
                                                {
                                                    int Siguiente = lTokens.FindIndex(x => x == temp) + 1;
                                                    //ver que caso puede ser
                                                    switch (lTokens[Siguiente].Palabra)
                                                    {
                                                        case "(":
                                                            List<Token> Operar = new List<Token>();
                                                            temp = lTokens[Siguiente - 2]; //Valor a asignar

                                                            //Verificar que exista "temp" tanto en globales como en locales
                                                            if (variablesMetodo.TryGetValue(temp.Palabra, out Metodo Asignacion) == true) //Ver si está declara la variable que se va a asignar en el método
                                                            {
                                                                //Llenar lista con valores a operar
                                                                while (lTokens[Siguiente].Palabra != ";")
                                                                {
                                                                    Operar.Add(lTokens[Siguiente]);
                                                                    Siguiente++;
                                                                }
                                                                //Verificar tipos
                                                                bool tipos_correctos = true; //Se toma que todos los tipos de datos son correctos en un principio
                                                                string CadenaOperacion = "";
                                                                foreach (var item in Operar)
                                                                {
                                                                    //if (item.Palabra != ">" && item.Palabra != "<" && item.Palabra != ">=" && item.Palabra != "<=" && item.Palabra != "==" && item.Palabra != "!=")
                                                                    //{
                                                                    if (item.Tipo_token != 4) //Media vez no sean operadores y sean números o variables
                                                                    {
                                                                        if (item.Tipo_token == 5) //Verificar si item es una variable
                                                                        {
                                                                            //Si es una variable verificar si existe
                                                                            if (variablesMetodo.TryGetValue(item.Palabra, out Metodo LocalItem) == true)
                                                                            {
                                                                                CadenaOperacion += LocalItem.Valor;
                                                                            }
                                                                            else if (VariablesGlobal.TryGetValue(item.Palabra, out Variable GlobalItem) == true)
                                                                            {
                                                                                CadenaOperacion += GlobalItem.Valor;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine($"Error en la linea {item.Linea}. identificador: {item.Palabra} no fue declarado previamente");
                                                                            }
                                                                        }
                                                                        else //Es una constante de cierto tipo
                                                                        {
                                                                            if (temp.Tipo_token == 3 && (item.Tipo_token == 3 || item.Tipo_token == 2)) //Enteros se pueden operar con dobles
                                                                            {
                                                                                CadenaOperacion += item.Palabra; //Agregar eso a la expresion
                                                                            }
                                                                            else
                                                                            {
                                                                                tipos_correctos =false;
                                                                                Console.WriteLine($"Error en la linea {item.Linea}. identificador: {item.Palabra} tipo de dato inválido para operar");
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else //Agregar los operadores sin verificar nada
                                                                    {
                                                                        CadenaOperacion += item.Palabra;
                                                                    }


                                                                }
                                                                if (tipos_correctos != false) //Si todo está bien, añadirlo a la tabla de símbolos
                                                                {
                                                                    if (CadenaOperacion.Contains('>') || CadenaOperacion.Contains('<') || CadenaOperacion.Contains(">=") || CadenaOperacion.Contains("<=") || CadenaOperacion.Contains("==") || CadenaOperacion.Contains("!=") || CadenaOperacion.Contains("&&") || CadenaOperacion.Contains("||") || CadenaOperacion.Contains("%"))
                                                                    {
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, CadenaOperacion));
                                                                    }
                                                                    else
                                                                    {

                                                                        Compiler objCompiler = new Compiler();
                                                                        ExpressionsCollection objColExpressions;

                                                                        // Asigna las propiedades de compilación
                                                                        if (temp.Tipo_token == 3)
                                                                        {
                                                                            objCompiler.Properties.Decimals = 0;
                                                                        }
                                                                        else
                                                                        {
                                                                            objCompiler.Properties.Decimals = 2;
                                                                        }
                                                                        
                                                                        // Interpreta la expresión de una cadena de texto
                                                                        objColExpressions = objCompiler.Parse(CadenaOperacion);


                                                                        VariablesCollection objColVariables = new VariablesCollection();
                                                                        ValueBase objResult;


                                                                        // Ejecuta las expresiones
                                                                        objResult = objCompiler.Evaluate(objColExpressions, objColVariables, out string strError);
                                                                        // Muestra el resultado
                                                                        if (!string.IsNullOrEmpty(strError))
                                                                            Console.WriteLine("Error en la ejecución: " + strError);
                                                                        else if (objResult.Content == "")
                                                                        {
                                                                            tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, "0"));
                                                                        }
                                                                        else
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, objResult.Content));
                                                                    }
                                                                    
                                                                }
                                                            }
                                                            else if (VariablesGlobal.TryGetValue(temp.Palabra, out Variable VariableGlobal1) == true) //Si no está en el método buscarla en las globales
                                                            {

                                                            }
                                                            else //Erro porque el id no está declarado
                                                            {
                                                                Console.WriteLine($"Error en la linea {temp.Linea}. identificador: {temp.Palabra} no fue declarado previamente");
                                                            }





                                                            break;
                                                        case "New":
                                                            Token Instancia = lTokens[Siguiente + 2]; //Obtiene el nombre de la instancia del token
                                                            temp = lTokens[Siguiente - 2]; //Valor a asignar
                                                            if (variablesMetodo.TryGetValue(temp.Palabra, out Metodo Asignacion3) == true) //Ver si está declara la variable que se va a asignar en el método
                                                            {
                                                                //Verificar que exista la declaración de la clase a instanciar
                                                                if (variablesMetodo.TryGetValue(Instancia.Palabra, out Metodo Value) == true) //Ver si existe la instancia
                                                                {
                                                                    if (Asignacion3.TipoDato != Value.TipoDato)
                                                                    {
                                                                        Console.WriteLine($"Error en la linea, {Asignacion3.Linea}. los tipos de datos {temp.Palabra} y {Instancia.Palabra}, no coinciden");
                                                                    }
                                                                    else //todo bien asignar valor
                                                                    {
                                                                        variablesMetodo[temp.Palabra].Valor = Value.Valor;
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, Value.Valor));
                                                                    }
                                                                }
                                                            }
                                                            else if (VariablesGlobal.TryGetValue(temp.Palabra, out Variable VariableGlobal) == true) //Si no está en el método buscarla en las globales
                                                            {
                                                                //Verificar que exista la declaración de la clase a instanciar
                                                                if (variablesMetodo.TryGetValue(Instancia.Palabra, out Metodo Value) == true) //Ver si existe la instancia
                                                                {
                                                                    if (VariableGlobal.TipoDato != Value.TipoDato)
                                                                    {
                                                                        Console.WriteLine($"Error en la linea, {temp.Linea}. los tipos de datos {temp.Palabra} y {Instancia.Palabra}, no coinciden");
                                                                    }
                                                                    else //todo bien asignar valor
                                                                    {
                                                                        variablesMetodo[temp.Palabra].Valor = Value.Valor;
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, Value.Valor));
                                                                    }
                                                                }
                                                            }
                                                            else //Erro porque el id no está declarado
                                                            {
                                                                Console.WriteLine($"Error en la linea {temp.Linea}. identificador: {temp.Palabra} no fue declarado previamente");
                                                            }

                                                            break;
                                                        default: //Directamente una operación
                                                            List<Token> Operar2 = new List<Token>();
                                                            temp = lTokens[Siguiente - 2]; //Valor a asignar

                                                            //Verificar que exista "temp" tanto en globales como en locales
                                                            if (variablesMetodo.TryGetValue(temp.Palabra, out Metodo Asignacionnn) == true) //Ver si está declara la variable que se va a asignar en el método
                                                            {
                                                                //Llenar lista con valores a Operar2
                                                                while (lTokens[Siguiente].Palabra != ";")
                                                                {
                                                                    Operar2.Add(lTokens[Siguiente]);
                                                                    Siguiente++;
                                                                }
                                                                //Verificar tipos
                                                                bool tipos_correctos = true; //Se toma que todos los tipos de datos son correctos en un principio
                                                                string CadenaOperacion = "";
                                                                foreach (var item in Operar2)
                                                                {
                                                                    //if (item.Palabra != ">" && item.Palabra != "<" && item.Palabra != ">=" && item.Palabra != "<=" && item.Palabra != "==" && item.Palabra != "!=")
                                                                    //{
                                                                    if (item.Tipo_token != 4) //Media vez no sean operadores y sean números o variables
                                                                    {
                                                                        if (item.Tipo_token == 5) //Verificar si item es una variable
                                                                        {
                                                                            //Si es una variable verificar si existe
                                                                            if (variablesMetodo.TryGetValue(item.Palabra, out Metodo LocalItem) == true)
                                                                            {
                                                                                CadenaOperacion += LocalItem.Valor;
                                                                            }
                                                                            else if (VariablesGlobal.TryGetValue(item.Palabra, out Variable GlobalItem) == true)
                                                                            {
                                                                                CadenaOperacion += GlobalItem.Valor;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine($"Error en la linea {item.Linea}. identificador: {item.Palabra} no fue declarado previamente");
                                                                            }
                                                                        }
                                                                        else //Es una constante de cierto tipo
                                                                        {
                                                                            if (temp.Tipo_token == 3 && (item.Tipo_token == 3 || item.Tipo_token == 2)) //Enteros se pueden Operar2 con dobles
                                                                            {
                                                                                CadenaOperacion += item.Palabra; //Agregar eso a la expresion
                                                                            }
                                                                            else
                                                                            {
                                                                                tipos_correctos = false;
                                                                                Console.WriteLine($"Error en la linea {item.Linea}. identificador: {item.Palabra} tipo de dato inválido para Operar2");
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else //Agregar los operadores sin verificar nada
                                                                    {
                                                                        CadenaOperacion += item.Palabra;
                                                                    }


                                                                }
                                                                if (tipos_correctos != false) //Si todo está bien, añadirlo a la tabla de símbolos
                                                                {
                                                                    if (CadenaOperacion.Contains('>') || CadenaOperacion.Contains('<') || CadenaOperacion.Contains(">=") || CadenaOperacion.Contains("<=") || CadenaOperacion.Contains("==") || CadenaOperacion.Contains("!=") || CadenaOperacion.Contains("&&") || CadenaOperacion.Contains("||") || CadenaOperacion.Contains("%"))
                                                                    {
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, CadenaOperacion));
                                                                    }
                                                                    else
                                                                    {

                                                                        Compiler objCompiler = new Compiler();
                                                                        ExpressionsCollection objColExpressions;

                                                                        // Asigna las propiedades de compilación
                                                                        if (temp.Tipo_token == 3)
                                                                        {
                                                                            objCompiler.Properties.Decimals = 0;
                                                                        }
                                                                        else
                                                                        {
                                                                            objCompiler.Properties.Decimals = 2;
                                                                        }

                                                                        // Interpreta la expresión de una cadena de texto
                                                                        objColExpressions = objCompiler.Parse(CadenaOperacion);


                                                                        VariablesCollection objColVariables = new VariablesCollection();
                                                                        ValueBase objResult;


                                                                        // Ejecuta las expresiones
                                                                        objResult = objCompiler.Evaluate(objColExpressions, objColVariables, out string strError);
                                                                        // Muestra el resultado
                                                                        if (!string.IsNullOrEmpty(strError))
                                                                            Console.WriteLine("Error en la ejecución: " + strError);
                                                                        else
                                                                            Console.WriteLine("Resultado: " + objResult.Content);
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, objResult.Content));
                                                                    }

                                                                }
                                                            }
                                                            else if (VariablesGlobal.TryGetValue(temp.Palabra, out Variable VariableGlobal1) == true) //Si no está en el método buscarla en las globales
                                                            {
                                                                List<Token> Operar4 = new List<Token>();
                                                                //Llenar lista con valores a Operar4
                                                                while (lTokens[Siguiente].Palabra != ";")
                                                                {
                                                                    Operar4.Add(lTokens[Siguiente]);
                                                                    Siguiente++;
                                                                }
                                                                //Verificar tipos
                                                                bool tipos_correctos = true; //Se toma que todos los tipos de datos son correctos en un principio
                                                                string CadenaOperacion = "";
                                                                foreach (var item in Operar4)
                                                                {
                                                                    //if (item.Palabra != ">" && item.Palabra != "<" && item.Palabra != ">=" && item.Palabra != "<=" && item.Palabra != "==" && item.Palabra != "!=")
                                                                    //{
                                                                    if (item.Tipo_token != 4) //Media vez no sean operadores y sean números o variables
                                                                    {
                                                                        if (item.Tipo_token == 5) //Verificar si item es una variable
                                                                        {
                                                                            //Si es una variable verificar si existe
                                                                            if (variablesMetodo.TryGetValue(item.Palabra, out Metodo LocalItem) == true)
                                                                            {
                                                                                CadenaOperacion += LocalItem.Valor;
                                                                            }
                                                                            else if (VariablesGlobal.TryGetValue(item.Palabra, out Variable GlobalItem) == true)
                                                                            {
                                                                                CadenaOperacion += GlobalItem.Valor;
                                                                            }
                                                                            else
                                                                            {
                                                                                Console.WriteLine($"Error en la linea {item.Linea}. identificador: {item.Palabra} no fue declarado previamente");
                                                                            }
                                                                        }
                                                                        else //Es una constante de cierto tipo
                                                                        {
                                                                            if (temp.Tipo_token == 3 && (item.Tipo_token == 3 || item.Tipo_token == 2)) //Enteros se pueden Operar4 con dobles
                                                                            {
                                                                                CadenaOperacion += item.Palabra; //Agregar eso a la expresion
                                                                            }
                                                                            else
                                                                            {
                                                                                tipos_correctos = false;
                                                                                Console.WriteLine($"Error en la linea {item.Linea}. identificador: {item.Palabra} tipo de dato inválido para Operar4");
                                                                                break;
                                                                            }
                                                                        }
                                                                    }
                                                                    else //Agregar los operadores sin verificar nada
                                                                    {
                                                                        CadenaOperacion += item.Palabra;
                                                                    }


                                                                }
                                                                if (tipos_correctos != false) //Si todo está bien, añadirlo a la tabla de símbolos
                                                                {
                                                                    if (CadenaOperacion.Contains('>') || CadenaOperacion.Contains('<') || CadenaOperacion.Contains(">=") || CadenaOperacion.Contains("<=") || CadenaOperacion.Contains("==") || CadenaOperacion.Contains("!=") || CadenaOperacion.Contains("&&") || CadenaOperacion.Contains("||") || CadenaOperacion.Contains("%"))
                                                                    {
                                                                        tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, CadenaOperacion));
                                                                    }
                                                                    else
                                                                    {

                                                                        Compiler objCompiler = new Compiler();
                                                                        ExpressionsCollection objColExpressions;

                                                                        // asignacionGlobal las propiedades de compilación
                                                                        if (temp.Tipo_token == 3)
                                                                        {
                                                                            objCompiler.Properties.Decimals = 0;
                                                                        }
                                                                        else
                                                                        {
                                                                            objCompiler.Properties.Decimals = 2;
                                                                        }

                                                                        // Interpreta la expresión de una cadena de texto
                                                                        objColExpressions = objCompiler.Parse(CadenaOperacion);


                                                                        VariablesCollection objColVariables = new VariablesCollection();
                                                                        ValueBase objResult;


                                                                        // Ejecuta las expresiones
                                                                        objResult = objCompiler.Evaluate(objColExpressions, objColVariables, out string strError);
                                                                        // Muestra el resultado
                                                                        if (!string.IsNullOrEmpty(strError))
                                                                            Console.WriteLine("Error en la ejecución: " + strError);
                                                                        else if (objResult.Content == "")
                                                                        {
                                                                            tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, "0"));
                                                                        }
                                                                        else
                                                                            tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, objResult.Content));
                                                                    }

                                                                }
                                                            }
                                                            else //Erro porque el id no está declarado
                                                            {
                                                                Console.WriteLine($"Error en la linea {temp.Linea}. identificador: {temp.Palabra} no fue declarado previamente");
                                                            }
                                                            break;
                                                    }
                                                    int IndiceActual = lTokens.FindIndex(x => x == temp) + 2; //Para encontrar el nombre de la 






                                                    if (ListaToken.Peek().Palabra == ";")
                                                    {
                                                        ListaToken.Dequeue();
                                                    }
                                                }
                                                if(temp.Palabra == ".")
                                                {

                                                }
                                            }
                                            else if (valorActual.Palabra == "{")
                                            {
                                                ContadorLlaveMetodo++;
                                            }
                                            else if (valorActual.Palabra == "}")
                                            {
                                                ContadorLlaveMetodo--;
                                            }
                                            else
                                            {

                                            }
                                            if (ContadorLlaveMetodo != contadorLLave && ListaToken.Count == 0)
                                            {
                                                Console.ForegroundColor = ConsoleColor.Red;
                                                Console.WriteLine("Falta llave de cierre del metodo, error en linea: " + valorActual.Linea);
                                                Console.ResetColor();
                                            }
                                        }

                                        //cierre de metodo agregar el metodo
                                        Intermedio aux = new Intermedio(TipoDatoFV, variablesMetodo);
                                        if (Metodos.ContainsKey(NombreMV.Palabra))
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("Ya se ha declarado un metodo con el nombre: " + NombreMV.Palabra + " dentro de la clase: "
                                                     + NombreClase.Palabra + " error en linea: " + valorActual.Linea);
                                            Console.ResetColor();
                                            //Error existe un metodo con ese nombre
                                        }
                                        else
                                        {
                                            Metodos.Add(NombreMV.Palabra, aux);
                                        }

                                    }
                                    else
                                    {
                                        Console.ForegroundColor = ConsoleColor.Red;
                                        Console.WriteLine("No existe llave de apertura del metodo, error en linea: " + valorActual.Linea);
                                        Console.ResetColor();
                                        //Falta apertura del metodo
                                    }

                                }//variables declaradas dentro de la clase
                                else if (ListaToken.Peek().Palabra == ";")
                                {
                                    if (NombreMV.Tipo_token == 5)
                                    {
                                        Variable nueva = new Variable("", TipoDatoFV, NombreMV.CInicio, NombreMV.CFinal, NombreMV.Linea, 0);
                                        if (Variables.ContainsKey(NombreMV.Palabra))
                                        {
                                            Console.ForegroundColor = ConsoleColor.Red;
                                            Console.WriteLine("Ya se ha declarado un variable con el nombre: " + NombreMV.Palabra + " dentro de la clase: "
                                                    + NombreClase.Palabra + " error en linea: " + NombreMV.Linea + " columna: " + NombreMV.CInicio + "-" + NombreMV.CFinal);
                                            Console.ResetColor();
                                        }
                                        else
                                        {
                                            Variables.Add(NombreMV.Palabra, nueva);
                                            tablaDeSimbolos.Add(new TablaDeSimbolos(NombreMV.Palabra, nueva.Valor));
                                        }
                                    }
                                }
                                else
                                {
                                    //
                                }
                            }
                            else if (valorActual.Palabra == "{")
                            {
                                contadorLLave++;
                            }
                            else if (valorActual.Palabra == "}")
                            {
                                contadorLLave--;
                            }
                            else
                            {

                            }
                            if (contadorLLave != 0 || ListaToken.Count == 0)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.WriteLine("Falta llave de cierre, error en linea " + valorActual.Linea);
                                Console.ResetColor();
                            }
                        }

                        //Termina la clase entonces agregar la instancia de clase al diccionario
                        Tipos modelo2 = new Tipos(Variables, Metodos);
                        if (clases.ContainsKey(NombreClase.Palabra))
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Ya se ha declarado un variable con el nombre:" + NombreClase.Palabra +
                                "  error en linea: " + NombreClase.Linea + " columna: " + NombreClase.CInicio + "-" + NombreClase.CFinal);
                            Console.ForegroundColor = ConsoleColor.Red;
                            //Existe una clase con el mismo nombre
                        }
                        else
                        {
                            clases.Add(NombreClase.Palabra, modelo2);
                        }
                    }


                }



                //funciones y variables en contexto global
                else if (valorActual.Palabra == "void" || valorActual.Tipo_token == 5 || valorActual.Palabra == "int" || valorActual.Palabra == "string" || valorActual.Palabra == "bool" || valorActual.Palabra == "double")
                {
                    string TipoDatoGlob = valorActual.Palabra;
                    if (ListaToken.Peek().Palabra == "[]")
                    {
                        TipoDatoGlob += ListaToken.Dequeue().Palabra;
                    }
                    Token ident = ListaToken.Dequeue();
                    int contadorLlaves = 0;



                    if (ListaToken.Peek().Palabra == "(") // Declaraciones de metodos fuera de clases
                    {
                        Dictionary<string, Metodo> parametros = new Dictionary<string, Metodo>();
                        ListaToken.Dequeue();

                        while (ListaToken.Peek().Palabra != ")")
                        {
                            Metodo modelo = new Metodo();
                            string TipoDato = ListaToken.Dequeue().Palabra;
                            if (ListaToken.Peek().Palabra == "[]")
                            {
                                TipoDato += ListaToken.Dequeue().Palabra;
                            }
                            Token temp = ListaToken.Dequeue();
                            modelo.TipoDato = TipoDato;
                            modelo.CInicio = temp.CInicio;
                            modelo.CFinal = temp.CFinal;
                            modelo.Contexto = 1;
                            modelo.Valor = "";
                            modelo.Linea = temp.Linea;

                            if (parametros.ContainsKey(temp.Palabra))
                            {
                                Console.WriteLine("Ya se ha declarado un variable con el nombre: " + temp.Palabra + " dentro del metodo: "
                                           + ident.Palabra + " error en linea: " + temp.Linea + " columna: " + temp.CInicio + "-" + temp.CFinal);
                            }
                            else
                            {
                                parametros.Add(temp.Palabra, modelo);
                                tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, modelo.Valor));
                            }
                            if (ListaToken.Peek().Palabra == ",")
                            {
                                ListaToken.Dequeue();
                            }
                        }
                        if (ListaToken.Peek().Palabra == ")")
                        {
                            ListaToken.Dequeue();
                        }
                        //Variables dentro de un metodo
                        if (ListaToken.Peek().Palabra == "{")
                        {
                            contadorLlaves++;
                            ListaToken.Dequeue();
                            while (contadorLlaves != 0)
                            {
                                valorActual = ListaToken.Dequeue();

                                if (valorActual.Tipo_token == 5 || valorActual.Palabra == "int" || valorActual.Palabra == "string" || valorActual.Palabra == "bool" || valorActual.Palabra == "double")
                                {
                                    string tipoDato = valorActual.Palabra;
                                    if (ListaToken.Peek().Palabra == "[]")
                                    {
                                        tipoDato += ListaToken.Dequeue().Palabra;
                                    }
                                    Token temp = ListaToken.Dequeue();
                                    if (temp.Tipo_token == 5)
                                    {
                                        if (ListaToken.Peek().Palabra == ";")
                                        {
                                            Metodo modelo = new Metodo();
                                            modelo.TipoDato = tipoDato;
                                            modelo.CFinal = temp.CFinal;
                                            modelo.CInicio = temp.CInicio;
                                            modelo.Valor = "";
                                            modelo.Contexto = 0;
                                            modelo.Linea = temp.Linea;
                                            if (parametros.ContainsKey(temp.Palabra))
                                            {
                                                Console.WriteLine("Ya se ha declarado un variable con el nombre: " + temp.Palabra + " dentro del metodo: "
                                                    + ident.Palabra + " error en linea: " + temp.Linea + " columna: " + temp.CInicio + "-" + temp.CFinal);
                                            }
                                            else
                                            {
                                                parametros.Add(temp.Palabra, modelo);
                                                tablaDeSimbolos.Add(new TablaDeSimbolos(temp.Palabra, modelo.Valor));
                                                ListaToken.Dequeue();
                                            }
                                        }
                                        else
                                        {
                                            ListaToken.Dequeue();
                                        }
                                    }

                                }
                                else if (valorActual.Palabra == "{")
                                {
                                    contadorLlaves++;
                                }
                                else if (valorActual.Palabra == "}")
                                {
                                    contadorLlaves--;

                                }
                                else
                                {
                                    //
                                }
                                if (contadorLlaves != 0 && ListaToken.Count == 0)
                                {
                                    Console.WriteLine("Falta una llave de cierre, error en linea   " + valorActual.Linea);
                                    //Falta llave, marcar error
                                }

                            }
                            if (MetodosGlobal.ContainsKey(ident.Palabra))
                            {
                                Console.WriteLine("Ya se ha declarado un metodo con el nombre: " + ident.Palabra +
                                    " error en linea: " + ident.Linea + " columna: " + ident.CInicio + "-" + ident.CFinal);
                            }
                            else
                            {
                                Intermedio agregar = new Intermedio(TipoDatoGlob, parametros);
                                MetodosGlobal.Add(ident.Palabra, agregar);
                            }

                        }

                    }
                    else if (ListaToken.Peek().Palabra == ";")//Declaraciones de variables fuera de clases
                    {
                        if (ident.Tipo_token == 5)
                        {
                            if (VariablesGlobal.ContainsKey(ident.Palabra))
                            {
                                Console.WriteLine("Ya se ha declarado ua variable con el nombre: " + ident.Palabra +
                                    " error en linea: " + ident.Linea + " columna: " + ident.CInicio + "-" + ident.CFinal);
                                //Ya existe la variable en contexto global
                            }
                            else
                            {
                                Variable nuevo = new Variable("", TipoDatoGlob, ident.CInicio, ident.CFinal, ident.Linea, 0);
                                VariablesGlobal.Add(ident.Palabra, nuevo);
                            }
                        }
                    }
                    else
                    {
                        //no hacer nada
                    }
                }
                else if (valorActual.Palabra == "const")
                {
                    string tpd = ListaToken.Dequeue().Palabra;
                    Token identificador = ListaToken.Dequeue();
                    if (identificador.Tipo_token == 5)
                    {
                        Variable constantes = new Variable("", tpd, identificador.CInicio, identificador.CFinal, identificador.Linea, 1);
                        if (VariablesGlobal.ContainsKey(identificador.Palabra))
                        {
                            Console.WriteLine("Ya se ha declarado una variable con el nombre: " + identificador.Palabra +
                                    " error en linea: " + identificador.Linea + " columna: " + identificador.CInicio + "-" + identificador.CFinal);
                        }
                        else
                        {
                            VariablesGlobal.Add(identificador.Palabra, constantes);
                            tablaDeSimbolos.Add(new TablaDeSimbolos(identificador.Palabra, constantes.Valor));
                        }
                    }
                    if (ListaToken.Peek().Palabra == ";")
                    {
                        ListaToken.Dequeue();
                    }
                }
                else if (valorActual.Palabra == "Interface")
                {
                    while (ListaToken.Peek().Palabra != "{")
                    {
                        ListaToken.Dequeue();
                        if (ListaToken.Count == 0)
                        {
                            Console.WriteLine("Falta llave de apuertura en interfaz");
                            break;
                        }
                    }
                    if (ListaToken.Peek().Palabra != "{")
                    {
                        ListaToken.Dequeue();
                        int contadorLlaves = 1;
                        while (contadorLlaves != 0)
                        {
                            valorActual = ListaToken.Dequeue();
                            if (valorActual.Palabra == "{")
                            {
                                contadorLlaves++;
                            }
                            if (valorActual.Palabra == "}")
                            {
                                contadorLlaves--;
                            }
                        }
                        if (ListaToken.Peek().Palabra == "}")
                        {
                            ListaToken.Dequeue();
                        }
                    }
                }
                else
                {
                    //
                }
            }
            if (VariablesGlobal.Count > 0 && MetodosGlobal.Count > 0)
            {
                Tipos AgregarGlobal = new Tipos(VariablesGlobal, MetodosGlobal);

                clases.Add("CreacionVar_Met_Global", AgregarGlobal);
            }
            //Agregar global
            ImprimirLista(tablaDeSimbolos);
        }
        void ImprimirLista(List<TablaDeSimbolos> listaTokens)
        {
            StreamWriter writer = new StreamWriter("TablaDeSimbolos.txt");
            writer.WriteLine("Identificador\t\t\t\t\t\t\t\tValor");
            foreach (var item in listaTokens)
            {
                writer.WriteLine($"{item.Identificador}\t\t\t\t\t\t\t\t{item.Valor}");
            }
            writer.Close();
        }
    }
}
