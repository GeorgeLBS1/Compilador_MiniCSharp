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

        public void Analizador(Queue<Token> ListaToken)
        {

            while(ListaToken.Count!= 0)
            {
               Token valorActual = ListaToken.Dequeue();

                if(valorActual.Palabra == "class")
                {
                    Dictionary<string, Variable> Variables = new Dictionary<string, Variable>();
                    Dictionary<string, Intermedio> Metodos = new Dictionary<string, Intermedio>();
                }
                //funciones y variables en contexto global
                else if(valorActual.Tipo_token == 5 || valorActual.Palabra =="int" || valorActual.Palabra == "string" || valorActual.Palabra == "bool" ||
                     valorActual.Palabra == "double" )
                {
                    string ident = ListaToken.Dequeue().Palabra;
                    int contadorLlaves = 0;

                    if (ListaToken.Peek().Palabra == "(") // Declaraciones de metodos fuera de clases
                    {
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
                        ListaToken.Dequeue();
                        if (ListaToken.Peek().Palabra == "{")
                        {
                            contadorLlaves++;
                            ListaToken.Dequeue();
                            while(contadorLlaves !=0)
                            {
                                valorActual = ListaToken.Dequeue();
                                if (valorActual.Tipo_token == 5 || valorActual.Palabra == "int" || valorActual.Palabra == "string" || valorActual.Palabra == "bool" ||valorActual.Palabra == "double")
                                {
                                    string tipoDato = valorActual.Palabra;
                                    if(ListaToken.Peek().Palabra=="[]")
                                    {
                                        tipoDato += ListaToken.Dequeue().Palabra;
                                    }
                                    Token temp = ListaToken.Dequeue();
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
                                else if(valorActual.Palabra == "{")
                                {
                                    contadorLlaves++;
                                    ListaToken.Dequeue();
                                }
                                else if(valorActual.Palabra == "}")
                                {
                                    contadorLlaves--;
                                    ListaToken.Dequeue();
                                        
                                }
                                else
                                {
                                    ListaToken.Dequeue();
                                }
                            }
                        }

                    }
                    else//Declaraciones de variables fuera de clases
                    {

                    }

                    //Agregar a global


                }  
                else if(valorActual.Palabra == "const")
                {

                }
            }
        }
    }
}
