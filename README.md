# Compilador_MiniCSharp
Proyecto compiladores
José Díaz, Marcelo Rosales.

Laboratorio 1:


1. La mayoria de errores son manejados por la gramática, debido a que por ejemplo al faltar un paretensis, la gramática tomará lo que sigue como parte
de los Formals o Expr de dicha funcion o stmts. 

2. La gramática debe iniciar con la declaración de una variable, una funcion o un void, no se puede iniciar con un Stmt o un Expr

3. Cuando se haga un match token y este no sea el token esperado.

¿Cómo usarlo?
El uso del proyecto es simple:

1. Ejecute el programa .exe llamado MiniC

2. Espere a que cargue el mensaje en pantalla por completo y arrastre el archivo a analizar léxicamente a la consola que le aparecerá en la pantalla.

3. El archivo será analizado acontinuación y se mostrarán los errores en color rojo así como en verde si no existieron errores.

4. Se originará un archivo (.out) ubicado en la ruta donde se encuentra el programa MiniC.exe con el mismo nombre que el archivo de entrada. En este archivo se especificarán
todos los tokens reconocidos, así como la línea y las columnas en donde se encuentra este. También se especifican los errores encontrados en este archivo de texto. 
