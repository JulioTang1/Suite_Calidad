using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Data.SqlClient;

public class ConexionSQL
{
    SqlConnection conexion;

    /*
    **Nombre desarrollador
    **Fecha del comentario
    **Creador
    *Interfaz o componente que afecta
    *Titulo
    *Descripción detallada del proceso
    */
    public ConexionSQL()
    {
        conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings["CONEXION_BASE"].ConnectionString);
        //conexion = new SqlConnection("Server=190.146.168.212;Database=P360_WEB;User ID=plantilla_web;Password=dK5X1fBSyR9Y");
    }

    /*
    **Nombre desarrollador
    **Fecha del comentario
    **Creador
    *Interfaz o componente que afecta
    *Titulo
    *Descripción detallada del proceso
    */
    public ConexionSQL(string stringConexion)
    {
        conexion = new SqlConnection(System.Configuration.ConfigurationManager.ConnectionStrings[stringConexion].ConnectionString);
    }

    /*
    **Nombre desarrollador
    **Fecha del comentario
    **Creador
    *Interfaz o componente que afecta
    *Titulo
    *Descripción detallada del proceso
    */
    public string openConexion()
    {
        try
        {
            if (conexion.State != ConnectionState.Open)
            {
                conexion.Open();
                //Apertura de conexión
                return "TRUE";
            }
            else
            {
                //Conexión ya abierta
                return "TRUE";
            }
        }
        catch (SqlException ex)
        {
            //Error de conexión a base de datos
            return ex.ToString();
        }
    }

    /*
    **Nombre desarrollador
    **Fecha del comentario
    **Creador
    *Interfaz o componente que afecta
    *Titulo
    *Descripción detallada del proceso
    */
    public void closeConexion()
    {
        if (conexion.State == ConnectionState.Open)
            conexion.Close();

        //Limpieza de las conexiones abandonadas y que se mantienen abiertas
        SqlConnection.ClearPool(conexion);
    }

    /*
    **Nombre desarrollador
    **Fecha del comentario
    **Creador
    *Interfaz o componente que afecta
    *Titulo
    *Descripción detallada del proceso
    */
    public SqlConnection getConexion()
    {
        //Retorna el objeto conexión actualmente disponible
        return this.conexion;
    }
}