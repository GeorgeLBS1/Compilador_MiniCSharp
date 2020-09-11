using Compilador_MiniCSharp;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace MiniC
{
    public class AnalizadorSintacticoRecursivo
    {
        Queue<Token> Cola_Tokens = new Queue<Token>();        
        List<string> Lista_Types = new List<string>()
        {
            "int", "double", "bool", "string", "ident"
        };
        public AnalizadorSintacticoRecursivo(List<Token> tokens) //CONSTRUCTOR Creación de una cola con todos los tokens que se pasarán al analizador sintáctico
        {
            foreach (var item in tokens)
            {
                Cola_Tokens.Enqueue(item);
            }
        }
        public void MatchToken(string expected) //Función match token
        {
            if (Cola_Tokens.Count > 0)
            {
                
                switch (expected)
                {
                    
                    case "ident": //En caso sea un ident comparar el tipo de token
                        if (Cola_Tokens.Peek().Tipo_token == 5) //Si el token es el que se esperaba
                        {
                            Cola_Tokens.Dequeue(); //Analizar el siguiente Token
                        }
                        else
                        {
                            Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {expected}");
                        }
                        break;
                    case "intConstant": //En caso sea un numero el esperado comparar con tipo de token
                        if (Cola_Tokens.Peek().Tipo_token == 3) //Si el token es el que se esperaba
                        {
                            Cola_Tokens.Dequeue(); //Analizar el siguiente Token
                        }
                        else
                        {
                            Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {expected}");
                        }
                        break;
                    case "doubleConstant":
                        if (Cola_Tokens.Peek().Tipo_token == 2) //Si el token es el que se esperaba
                        {
                            Cola_Tokens.Dequeue(); //Analizar el siguiente Token
                        }
                        else
                        {
                            Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {expected}");
                        }
                        break;
                    case "boolConstant":
                        if (Cola_Tokens.Peek().Tipo_token == 1) //Si el token es el que se esperaba
                        {
                            Cola_Tokens.Dequeue(); //Analizar el siguiente Token
                        }
                        else
                        {
                            Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {expected}");
                        }
                        break;
                    case "stringConstant":
                        if (Cola_Tokens.Peek().Tipo_token == 6) //Si el token es el que se esperaba
                        {
                            Cola_Tokens.Dequeue(); //Analizar el siguiente Token
                        }
                        else
                        {
                            Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {expected}");
                        }
                        break;
                    default: //En caso no sea un tipo de identificador o un constante de algun tipo comparar la palabra o caracter como tal
                        if (Cola_Tokens.Peek().Palabra == expected) //Si el token es el que se esperaba
                        {
                            Cola_Tokens.Dequeue(); //Analizar el siguiente Token
                        }
                        else
                        {

                            Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {expected}");
                        }
                        break;
                }
                
                
            }
            if (Cola_Tokens.Count == 0)
            {
                FinAnalisis();
            }



        }

        void FinAnalisis()
        {
            Console.WriteLine("Análisis sintáctico terminado");
            Environment.Exit(1);
        }
        public void Parse_Program()
        {
            Parse_Decl2(); //Parsear Decl'            
        }
        void Parse_Decl2()
        {
            Parse_Decl(); //Parsear Decl
            if (Lista_Types.Contains(Cola_Tokens.Peek().Palabra) == true || Cola_Tokens.Peek().Tipo_token == 5 || Cola_Tokens.Peek().Tipo_token == 0) //Si en dado caso viene un Decl'
            {
                Parse_Decl2(); //Parsear Decl'
            }
            

        }
      
        void Parse_Decl()
        {
            if (Lista_Types.Contains(Cola_Tokens.Peek().Palabra) == true || Cola_Tokens.Peek().Tipo_token == 5) //Entrar a VariableDecl
            {


                Parse_Type2();

                MatchToken("ident");
                if (Cola_Tokens.Peek().Palabra == "[]" || Cola_Tokens.Peek().Palabra == ";") //Si es identificador
                {
                    if (Cola_Tokens.Peek().Palabra == "[]")
                    {
                        MatchToken("[]");
                    }
                    MatchToken(";");
                }

                else if (Cola_Tokens.Peek().Palabra == "()" || Cola_Tokens.Peek().Palabra == "(") //Si es funcion
                {
                    Parse_FunctionDecl();
                }
                else
                {
                    Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: {Cola_Tokens.Peek().Palabra}");
                    if (Cola_Tokens.Count > 0 && Lista_Types.Contains(Cola_Tokens.Peek().Palabra))
                    {
                        Parse_Program();
                    }
                    else
                    {
                        Parse_Stmt2();
                    }
                }

            }
            else if(Cola_Tokens.Peek().Palabra == "void") //se va a analizar por el lado de FunctionDecl
            {
                Parse_FunctionDecl();
            }

        }
        void Parse_VariableDecl()
        {
            Parse_Variable(); //Parsear variable
            MatchToken(";"); //MatchToken con ";"
            
        }
        void Parse_Variable()
        {
            Parse_Type(); //Parsear type
            MatchToken("ident"); //MatchToken con "ident"
        }
        void Parse_Type()
        {
            Parse_Type2(); //Parsear Type'
            if (Cola_Tokens.Peek().Palabra == "[]") //En dado caso venga []
            {
                MatchToken("[]");
            }
            else
            {
                return;
            }
            //Si viene epsilon
        }
        void Parse_Type2() //Parse de Type'
        {
            if (Cola_Tokens.Peek().Tipo_token != 5) //si no es un identificador
            {
                switch (Cola_Tokens.Peek().Palabra)
                {
                    case "int":
                        MatchToken("int"); //Match con token int
                        break;
                    case "double":
                        MatchToken("double"); //Match con token double
                        break;
                    case "bool":
                        MatchToken("bool"); //Match con token bool
                        break;
                    case "string":
                        MatchToken("string"); //Match con token string
                        break;
                    default:
                        Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Se esperaba un: int/double/bool/string");
                        break;
                }
            }
            else
            {
                MatchToken("ident"); //Hacer match token con ident
            }
            
        }

        void Parse_FunctionDecl()
        {
            if (Cola_Tokens.Peek().Palabra == "(" || Cola_Tokens.Peek().Palabra == "()") //Si viene un Type
            {
                if (Cola_Tokens.Peek().Palabra == "(")
                {
                    Parse_FunctionDecl2(); //Parsear functionDecl'
                }
                else
                {
                    MatchToken("()");
                    Parse_Stmt2();
                }
            }
            else if (Cola_Tokens.Peek().Palabra == "void") //Si viene la palabra void
            {
                MatchToken("void"); //Hacer matchtoken de void
                MatchToken("ident"); //Hacer matchtoken de void
                if (Cola_Tokens.Peek().Palabra == "(")
                {
                    Parse_FunctionDecl2(); //Parsear functionDecl'
                }
                else
                {
                    MatchToken("()");
                    Parse_Stmt2();
                }
                
            }
            else
            {
                Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}.");
            }
        }
        void Parse_FunctionDecl2()
        {
            MatchToken("("); //Hacer match del token "("
            Parse_Formals(); //Parsear Formals
            MatchToken(")"); //Hacer match del token ")"
            Parse_Stmt2();
        }
        void Parse_Formals()
        {
            if (Lista_Types.Contains(Cola_Tokens.Peek().Palabra) == true || Cola_Tokens.Peek().Tipo_token == 5) //En caso venga una variable '
            {
                Parse_Variable2(); //Parsear Variable'
            }
            else
            {
                return;
            }
        }
        void Parse_Variable2() //Parser de Variable'
        {
            
            Parse_Variable(); //Parsear variable
            if (Cola_Tokens.Peek().Palabra == ",")
            {
                MatchToken(",");
                if (Lista_Types.Contains(Cola_Tokens.Peek().Palabra) == true || Cola_Tokens.Peek().Tipo_token == 5)//Si viene otra variable
                {
                    Parse_Variable2(); //Parsear variable'
                }
                else
                {
                    //ERROR
                }
            }
            else
            {
                return;
            }
        }
        void Parse_Stmt2()
        {
            if (Cola_Tokens.Peek().Palabra == "while" || Cola_Tokens.Peek().Palabra == "Print" || Cola_Tokens.Peek().Tipo_token == 5 || Cola_Tokens.Peek().Tipo_token ==  3 || Cola_Tokens.Peek().Tipo_token == 2 || Cola_Tokens.Peek().Tipo_token == 1 || Cola_Tokens.Peek().Tipo_token == 6 || Cola_Tokens.Peek().Palabra == "null" || Cola_Tokens.Peek().Palabra == "this" || Cola_Tokens.Peek().Palabra == "(" || Cola_Tokens.Peek().Palabra == "=" || Cola_Tokens.Peek().Palabra == "-" || Cola_Tokens.Peek().Palabra == "!")
            {
                Parse_Stmt(); //Parsear Stmt
                Parse_Stmt2(); //Parsear Stmt'
            }
            
            else
            {
               

                return;
            }
        }
        void Parse_Stmt()
        {
            if (Cola_Tokens.Peek().Palabra == "while") //En dado caso el Stmt sea un while
            {
                Parse_WhileStmt(); //Parsear while
                Parse_Stmt2();
            }
            else if (Cola_Tokens.Peek().Palabra == "Print") //En dado caso el stmt sea un print
            {
                Parse_PrintStmt(); //Parsear print
                Parse_Stmt2();
            }
            else if (Cola_Tokens.Peek().Tipo_token == 5 || Cola_Tokens.Peek().Tipo_token == 3 || Cola_Tokens.Peek().Tipo_token == 2 || Cola_Tokens.Peek().Tipo_token == 1 || Cola_Tokens.Peek().Tipo_token == 6 || Cola_Tokens.Peek().Palabra == "null" || Cola_Tokens.Peek().Palabra == "this" || Cola_Tokens.Peek().Palabra == "(" || Cola_Tokens.Peek().Palabra == "-" || Cola_Tokens.Peek().Palabra == "!" || Cola_Tokens.Peek().Palabra == "=") //En dado caso sea una Expresión
            {
                Parse_Expr(); //Parse de Expresión
                MatchToken(","); //Match con la coma que puede venir después de una exp
                Parse_Stmt2();
            }
            else
            {
                Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Palabra no esperada");
            }
        }
        void Parse_WhileStmt()
        {
            MatchToken("while"); //Match de "while"
            MatchToken("("); //Match de "("
            Parse_Expr(); //Parsear expresión
            MatchToken(")");
            Parse_Stmt(); //Parse stmt
        }
        void Parse_PrintStmt()
        {
            MatchToken("Print");
            MatchToken("(");
            Parse_Expre2(); //Parsear Expre'
            MatchToken(")");
            MatchToken(";");
        }
        void Parse_Expre2() //Parse de Expre'
        {
            Parse_Expr();
            if (Cola_Tokens.Peek().Palabra == ",")
            {
                MatchToken(",");
                Parse_Expre2();
            }
            else
            {
                return;
            }
            
        }
        void Parse_Expr() //Parse de expre
        {
            Parse_ExprM();
            Parse_Expr2();
        }
        void Parse_Expr2() //Parser de Expr'
        {
            if (Cola_Tokens.Peek().Palabra == "&&") //Si viene AND
            {
                MatchToken("&&");
                Parse_ExprM();
                Parse_Expr2();
            }
            else if (Cola_Tokens.Peek().Palabra == "||") //Si viene OR
            {
                MatchToken("||");
                Parse_ExprM();
                Parse_Expr2();
            }
            else
            {
                return;
            }

        }
        void Parse_ExprM()
        {
            Parse_ExprN();
            Parse_ExprM2();
        }
      
        void Parse_ExprM2() //Parser de ExprM'
        {
            if (Cola_Tokens.Peek().Palabra == "==")
            {
                MatchToken("==");
                Parse_ExprN();
                Parse_ExprM2();
            }
            else if (Cola_Tokens.Peek().Palabra == "!=")
            {
                MatchToken("!=");
                Parse_ExprN();
                Parse_ExprM2();
            }
            else if (Cola_Tokens.Peek().Palabra == ">=")
            {
                MatchToken(">=");
                Parse_ExprN();
                Parse_ExprM2();
            }
            else if (Cola_Tokens.Peek().Palabra == "<=")
            {
                MatchToken("<=");
                Parse_ExprN();
                Parse_ExprM2();
            }
            else if (Cola_Tokens.Peek().Palabra == "<")
            {
                MatchToken("<");
                Parse_ExprN();
                Parse_ExprM2();
            }
            else if (Cola_Tokens.Peek().Palabra == ">")
            {
                MatchToken(">");
                Parse_ExprN();
                Parse_ExprM2();
            }
        
            else
            {
                return;
            }
        }

        void Parse_ExprN()
        {
            Parse_ExprO();
            Parse_ExprN2();

        }
        void Parse_ExprN2()
        {
            if(Cola_Tokens.Peek().Palabra == "+")
            {
                MatchToken("+");
                Parse_ExprO();
                Parse_ExprN2();
            }
            else if(Cola_Tokens.Peek().Palabra == "-")
            {
                MatchToken("-");
                Parse_ExprO();
                Parse_ExprN2();
            }
            else 
            {
                return;
            }
        }
        void Parse_ExprO()
        {
            Parse_ExprP();
            Parse_ExpreO2();
        }
        void Parse_ExpreO2()
        {
            if (Cola_Tokens.Peek().Palabra == "/")
            {
                MatchToken("/");
                Parse_ExprP();
                Parse_ExpreO2();
            }
            else if (Cola_Tokens.Peek().Palabra == "*")
            {
                MatchToken("*");
                Parse_ExprP();
                Parse_ExpreO2();
            }
            else if (Cola_Tokens.Peek().Palabra == "%")
            {
                MatchToken("%");
                Parse_ExprP();
                Parse_ExpreO2();
            }
            else
            {
                return;
            }
        }
        void Parse_ExprP()
        {
            if(Cola_Tokens.Peek().Palabra == "this")
            {
                MatchToken("this");
                if (Cola_Tokens.Count > 0)
                {
                    Parse_Expr();
                }
            }
            else if (Cola_Tokens.Peek().Palabra == "(")
            {
                MatchToken("(");
                Parse_Expr();
                MatchToken(")");
            }
            else if (Cola_Tokens.Peek().Palabra == "-")
            {
                MatchToken("-");
                Parse_Expr();
            }
            else if (Cola_Tokens.Peek().Palabra == "!")
            {
                MatchToken("!");
                Parse_Expr();
            }
            else if (Cola_Tokens.Peek().Palabra == "new")
            {
                MatchToken("new");
                MatchToken("(");
                MatchToken("ident");
                MatchToken(")");
                if (Cola_Tokens.Count > 0)
                {
                    Parse_Expr();
                }
            }
            else if(Cola_Tokens.Peek().Tipo_token == 3 || Cola_Tokens.Peek().Tipo_token == 2 || Cola_Tokens.Peek().Tipo_token == 1 || Cola_Tokens.Peek().Tipo_token == 6 || Cola_Tokens.Peek().Palabra == "null")
            {
                Parse_Constant();
            }
            else if(Cola_Tokens.Peek().Palabra == "=")
            {
                MatchToken("=");
                Parse_Expr();
            }
            else if(Cola_Tokens.Peek().Tipo_token ==5 || Cola_Tokens.Peek().Palabra == "." || Cola_Tokens.Peek().Palabra == "[")
            {
                Parse_LValue();
                if (Cola_Tokens.Count > 0)
                {
                    Parse_Expr();
                }
                
            }
            else
            {
                if (Cola_Tokens.Count > 0)
                {
                    if (Cola_Tokens.Peek().Tipo_token == 0)
                    {
                        Parse_Stmt2();
                    }
                }
                return;
            }
        }
        void Parse_LValue()
        {
            if(Cola_Tokens.Peek().Tipo_token == 5)
            {
                MatchToken("ident");
            }
            else if(Cola_Tokens.Peek().Palabra == ".")
            {
                MatchToken(".");
                MatchToken("ident");

            }
            else if(Cola_Tokens.Peek().Palabra == "[")
            {
                MatchToken("[");
                Parse_Expr();
                MatchToken("]");
            }
            else
            {
                return;
            }
        }

        void Parse_Constant() //Parser para constantes
        {
            if (Cola_Tokens.Peek().Tipo_token == 3) //Caso de constante entero
            {
                MatchToken("intConstant");
            }
            else if (Cola_Tokens.Peek().Tipo_token == 2) //Constante double
            {
                MatchToken("doubleConstant");
            }
            else if (Cola_Tokens.Peek().Tipo_token == 1) //Constante bool
            {
                MatchToken("boolConstant");
            }
            else if (Cola_Tokens.Peek().Tipo_token == 6) //Constante string
            {
                MatchToken("stringConstant");
            }
            else if (Cola_Tokens.Peek().Palabra == "null") //null
            {
                MatchToken("null");
            }
            else
            {
                Console.WriteLine($"Error sintáctico en linea: {Cola_Tokens.Peek().Linea}, columnas: {Cola_Tokens.Peek().CInicio}-{Cola_Tokens.Peek().CFinal}. Palabra no esperada");
            }
        }
    }
}
