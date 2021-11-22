using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Globalization;

public class Reporte_uso : System.Web.Services.WebService
{
    /**********************************************************************************/
    /**********************************************************************************/
    /**********************************************************************************/

    [WebMethod(EnableSession = true)]
    public string Hist_min_date()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                Begin Transaction;

                SELECT
	                COALESCE(CAST(MIN(TCE.fecha_cambio) AS NVARCHAR(10)),'') AS [fecha_min]
                FROM
	                dbo.T_cambio_estado_usuario TCE;

                Commit Transaction;
                "), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                string fecha = dt.Tables[0].Rows[0]["fecha_min"].ToString();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = fecha;

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, "");
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string Sesion_modulo_min_date()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                Begin Transaction;

                SELECT
	                COALESCE(CAST(MIN(TUL.fecha_sesion) AS NVARCHAR(10)),'') AS [fecha_min]
                FROM
	                dbo.T_usuario_login TUL;

                Commit Transaction;
                "), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                string fecha = dt.Tables[0].Rows[0]["fecha_min"].ToString();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = fecha;

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, "");
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string Sesion_suite_min_date()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                Begin Transaction;

                SELECT
	                COALESCE(CAST(MIN(TUS.fecha_sesion) AS NVARCHAR(10)),'') AS [fecha_min]
                FROM
	                dbo.T_usuario_suite TUS;

                Commit Transaction;
                "), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                string fecha = dt.Tables[0].Rows[0]["fecha_min"].ToString();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = fecha;

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, "");
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string Menu_min_date()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                Begin Transaction;

                SELECT
	                COALESCE(CAST(MIN(TLM.fecha_menu_select) AS NVARCHAR(10)),'') AS [fecha_min]
                FROM
	                dbo.T_login_menus TLM;

                Commit Transaction;
                "), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                string fecha = dt.Tables[0].Rows[0]["fecha_min"].ToString();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = fecha;

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, "");
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }


}

    /**********************************************************************************/
