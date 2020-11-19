using System;
using System.Collections.Generic;
using System.Text;
using Compilador_MiniCSharp;


namespace MiniC
{
    class AnalizadorSemantico
    {
        Dictionary<string, Tipos> clases = new Dictionary<string, Tipos>();
        Dictionary<string, Metodo> parametros = new Dictionary<string, Metodo>();
        Dictionary<string, Variable> VariablesGlobal = new Dictionary<string, Variable>();
        Dictionary<string, Intermedio> MetodosGlobal = new Dictionary<string, Intermedio>();

        public void Analizador(Queue<Token> ListaToken)
        {

            while (ListaToken.Count != 0)
            {
                Token valorActual = ListaToken.Dequeue();

                if (valorActual.Palabra == "class")
                {
                    string NombreClase = "";
                    int contadorLLave = 0;
                    int ContadorLlaveMetodo = 0;
                    bool erroLlaves = false;
                    Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
                    Dictionary<string, Intermedio> Metodos = new Dictionary<string, Intermedio>();
                    Dictionary<string, Metodo> variablesMetodo = new Dictionary<string, Metodo>();
                    NombreClase = ListaToken.Dequeue().Palabra;
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
                                    if (!Metodos.ContainsKey(NombreMV.Palabra))
                                    {
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
                                                //Marcar error
                                            }
                                            else
                                            {
                                                variablesMetodo.Add(temp.Palabra, modelo);
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



                                                            modelo.TipoDato = TPV;
                                                            modelo.CInicio = temp.CInicio;
                                                            modelo.CFinal = temp.CFinal;
                                                            modelo.Contexto = 0;
                                                            modelo.Valor = "";
                                                            modelo.Linea = temp.Linea;

                                                            if (variablesMetodo.ContainsKey(temp.Palabra))
                                                            {
                                                                //Marcar error
                                                            }
                                                            else
                                                            {
                                                                variablesMetodo.Add(temp.Palabra, modelo);
                                                            }
                                                            if (ListaToken.Peek().Palabra == ";")
                                                            {
                                                                ListaToken.Dequeue();
                                                            }


                                                        }
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
                                                    //Error no hay llaves de cierre;
                                                }
                                            }

                                            //cierre de metodo agregar el metodo
                                            Intermedio aux = new Intermedio(TipoDatoFV, variablesMetodo);
                                            if (Metodos.ContainsKey(NombreMV.Palabra))
                                            {
                                                //Error existe un metodo con ese nombre
                                            }
                                            else
                                            {
                                                Metodos.Add(NombreMV.Palabra, aux);
                                            }

                                        }
                                        else
                                        {
                                            //Falta apertura del metodo
                                        }
                                    }
                                    else
                                    {
                                        //El metodo ingresado ya existe en la clase
                                    }
                                }//variables declaradas dentro de la clase
                                else if (ListaToken.Peek().Palabra == ";")
                                {
                                    if (NombreMV.Tipo_token == 5)
                                    {
                                        Variable nueva = new Variable("", TipoDatoFV, NombreMV.CInicio, NombreMV.CFinal, NombreMV.Linea, 0);
                                        if (Variables.ContainsKey(NombreMV.Palabra))
                                        {
                                            //Existe una variable con el mismo nombre dentro de la clase
                                        }
                                        else
                                        {


                                            Variables.Add(NombreMV.Palabra, nueva);
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
                        }

                        //Termina la clase entonces agregar la instancia de clase al diccionario
                        Tipos modelo2 = new Tipos(Variables, Metodos);
                        if (clases.ContainsKey(NombreClase))
                        {
                            //Existe una clase con el mismo nombre
                        }
                        else
                        {
                            clases.Add(NombreClase, modelo2);
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
                                //Marcar error
                            }
                            else
                            {
                                parametros.Add(temp.Palabra, modelo);
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

                                if ( valorActual.Tipo_token == 5 || valorActual.Palabra == "int" || valorActual.Palabra == "string" || valorActual.Palabra == "bool" || valorActual.Palabra == "double")
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
                                                //Marcar error por definicion ya establecida
                                            }
                                            else
                                            {
                                                parametros.Add(temp.Palabra, modelo);
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
                                    //Falta llave, marcar error
                                }

                            }
                            if (MetodosGlobal.ContainsKey(ident.Palabra))
                            {
                                //Existe un metoodo con el mismo nombre
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
                    }
                    if(ListaToken.Peek().Palabra == ";")
                    {
                        ListaToken.Dequeue();
                    }
                }
                else if (valorActual.Palabra == "Interface")
                {

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
        }
    }
}
