using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;


public class GestionReportes
{
    /// <summary>
    /// Método que borrar los archivos almacenados en una carpeta con fecha menor a la cantidad de días ingresada como parámetro
    /// </summary>
    /// <param name="Folder">Carpeta que contiene los archivos a borrar</param>
    /// <param name="Days">Cantidad de días a partir de la fecha actual del sistema que borrará los archivos</param>
    public static void ClearFolder(string Folder,int Days)
    {
        string[] Files = Directory.GetFiles(Folder);
        DateTime Lim = DateTime.Now.AddDays(-Days);
        foreach (string File in Files)
        {
            FileInfo fi = new FileInfo(File);
            if (fi.CreationTime <= Lim)
            {
                fi.Delete();
            }                    
        }
    }

}
