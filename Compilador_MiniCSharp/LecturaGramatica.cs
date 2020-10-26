using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MiniC
{
    public class LecturaGramatica
    {
        List<string[]> lineas = new List<string[]>();
        Dictionary<Campo, string[]> Tabla = new Dictionary<Campo, string[]>();
        Dictionary<int, string> Simbolos = new Dictionary<int, string>();
        public void Leer(string path)
        {
            StreamReader reader = new StreamReader(path);
            string linea;
            while ((linea = reader.ReadLine()) != null)
            {
                var SepararCampos = linea.Split(';');
                lineas.Add(SepararCampos);
            }
            reader.Close();
            lineas[0][0] = ";";  //cambiar texto PuntoYComa por literal el ";"
            var TitulosList = new List<Campo>();
            TitulosList.Add(new Campo("Estado", "Simbolo"));    //Crear un título para el diccionario

            //Llenar los identificadores (estado, simbolo)
            for (int Estado = 0; Estado < lineas.Count - 1; Estado++)
            {
                for (int simbolo = 0; simbolo < lineas[0].Length; simbolo++) //para cada símbolo
                {
                    TitulosList.Add(new Campo(Convert.ToString(Estado), lineas[0][simbolo]));
                }
            }

            //Llenar el diccionario de símbolos
            for (int i = 0; i < lineas[0].Length; i++)
            {
                Simbolos.Add(i, lineas[0][i]);
            }

            lineas.RemoveAt(0); //quitar la columna de títulos

            //Generar tabla de transiciones
            for (int estado = 0; estado < lineas.Count; estado++)
            {
                for (int accion = 0; accion < lineas[0].Length; accion++)
                {
                    Campo insertar = new Campo(Convert.ToString(estado), Simbolos[accion]);
                    Tabla.Add(insertar, lineas[estado][accion].Split('-'));
                }
            }


            //BORRAR ;PUNTO DE CHECKEO
            foreach (var item in Tabla) //Impresión, quitar después
            {
                if (item.Value[0].Length > 0)
                {
                    Console.WriteLine($"{item.Key.Estado}, {item.Key.Simbolo} --> {item.Value[0]}");
                }

            }


            //BORRAR ;PUNTO DE CHECKEO
            int cantidad = TitulosList.Count;
            Console.WriteLine(Convert.ToString(cantidad)); //chequeo
        }
    }
}
