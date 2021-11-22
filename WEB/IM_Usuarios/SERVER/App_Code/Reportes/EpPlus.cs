using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Drawing;
using System.Data;

public class EpPlus
{

    public static void MergeCells(ExcelRange Range)
    {
        Range.Merge = true;
        Range.Style.VerticalAlignment = ExcelVerticalAlignment.Center;
        Range.Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

    }

    public static void Bordes(ExcelRange Range, ExcelBorderStyle Style)
    {
        Range.Style.Border.Top.Style = Style;
        Range.Style.Border.Bottom.Style = Style;
        Range.Style.Border.Left.Style = Style;
        Range.Style.Border.Right.Style = Style;
    }

    public static void Bordes(ExcelRange Range, ExcelBorderStyle Style, Color ColorBorde)
    {

        // Llamando la función tradicional
        Bordes(Range, Style);

        // Agregando el color a los bordes        
        Range.Style.Border.Top.Color.SetColor(ColorBorde);
        Range.Style.Border.Bottom.Color.SetColor(ColorBorde);
        Range.Style.Border.Left.Color.SetColor(ColorBorde);
        Range.Style.Border.Right.Color.SetColor(ColorBorde);

    }

    public static void PosicionaTabla(ExcelWorksheet Worksheet, DataTable Table, int X, int Y, string Titulos, bool Bold, Color ColorTitulo)
    {
        
        int Desp = 0;               // Desplazamiento
        int XiCelda = X;            // Posición inicial en el archivo de Excel
        int XfCelda = 0;            // Posición final en el archivo de Excel
        int Xtabla = -1;            // Columna actual del DataTable

        float Ancho = 0;            // Ancho de columna

        string Titulo = "";         // Título de la columna
        string FormatoCelda = "";   // Formato que tendrá la celda (dd-mm-yyyy, 0.00, 0.00%, etc)
        string Formula = "";        // Fórmula que tendrá la celda

        // Segmentando por número de titúlos, ya que la tabla puede tener un menor número de columnas
        string[] ArrayTitulos = Titulos.Split(';');
        for (int i = 0; i < ArrayTitulos.Length; i++)
        {
            // La cadena deberá tener el siguiente formato: 
            // Nombre - columnas a utilizar - ancho columna(s) - Formato (Número, Porcentaje, etc) - Fórmula (Si la celda requiere)
            string[] ArrayCelda = ArrayTitulos[i].Split('|');            
            if (ArrayCelda.Length == 5)
            {

                // Adquisición de parámetros
                Titulo = ArrayCelda[0];
                int.TryParse(ArrayCelda[1], out Desp);
                float.TryParse(ArrayCelda[2], out Ancho);
                FormatoCelda = ArrayCelda[3];
                Formula = ArrayCelda[4];

                // Determinando la posición final en el archivo
                XfCelda = XiCelda + Desp - 1;

                // Habrá desplazamiento en el datatable de forma horizontal únicamente cuando la columna actual no tiene fórmula
                // Esto se hace con el objetivo de no tener que crear columnas vacías en el datatable para que 
                // los títulos y la tabla fuente de información coincidan en número de columnas
                Xtabla = Formula == "" ? Xtabla + 1 : Xtabla;

                // Modificando el ancho de la celda cuando sea necesario
                if (Ancho > 0)
                {
                    Worksheet.Column(i + 1).Width = Ancho;
                }

                /////////////
                // TÍTULOS //
                /////////////

                Worksheet.Cells[Y, XiCelda].Value = Titulo;
                Worksheet.Cells[Y, XiCelda].Style.Font.Bold = Bold;
                Worksheet.Cells[Y, XiCelda].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                Worksheet.Cells[Y, XiCelda].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                // Combinando las celdas del título cuando hay mas de una columna
                if (XfCelda > XiCelda)
                {
                    MergeCells(Worksheet.Cells[Y, XiCelda, Y, XfCelda]);
                }

                ///////////
                // DATOS //
                ///////////

                for (int j = 0; j < Table.Rows.Count; j++)
                {

                    // Contenido de la celda
                    if (Formula == "")
                    {
                        Worksheet.Cells[Y + j + 1, XiCelda].Value = Table.Rows[j][Xtabla];
                    }
                    else
                    {
                        Worksheet.Cells[Y + j + 1, XiCelda].FormulaR1C1 = Formula;
                    }

                    // Formato
                    Worksheet.Cells[Y + j + 1, XiCelda].Style.Numberformat.Format = FormatoCelda;

                    // Combinando las celdas del contenido cuando hay mas de una columna
                    if (XfCelda > XiCelda)
                    {
                        MergeCells(Worksheet.Cells[Y + j + 1, XiCelda, Y + j + 1, XfCelda]);
                    }
                }

                XiCelda += Desp;
            }
        }

        if (ColorTitulo.IsEmpty == false)
        {
            Worksheet.Cells[Y, X, Y, XfCelda].Style.Fill.PatternType = OfficeOpenXml.Style.ExcelFillStyle.Solid;
            Worksheet.Cells[Y, X, Y, XfCelda].Style.Fill.BackgroundColor.SetColor(ColorTitulo);
        }
    }

}
