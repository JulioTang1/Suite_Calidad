using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;
using System.Collections.Specialized;
using System.Net;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using System.Threading;


public class token : System.Web.Services.WebService
{

    [WebMethod(EnableSession = true)]
    public string Validar_token(int id_user, string token, int id_app, string navegador)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    Begin Transaction;

                    SELECT
	                    usuarios.url_foto AS [Foto],
	                    usuarios.usuario AS [usuario],
	                    usuarios.nombres AS [nombres],
	                    usuarios.apellidos AS [apellidos],
	                    
	                    usuarios.id AS [ID_user],
                        usuarios.correo AS [email],

                        usuarios.nombre_usuario AS [nombre_usuario],
                        roles.id AS [id_rol],
                        roles.nombre AS [rol],
                        
                        app.id [id_app]
                    FROM
	                    {0}.T_usuario_suite suite
                    INNER JOIN {0}.usuario usuarios ON suite.id_usuario = usuarios.id
                    INNER JOIN {0}.estado_usuario est ON usuarios.id = est.id_usuario
                    INNER JOIN {0}.rol roles ON est.id_rol = roles.id
                    INNER JOIN {0}.rol_aplicacion RA ON RA.id_rol = roles.id
                    INNER JOIN {0}.aplicacion app ON RA.id_app = app.id
                    WHERE
	                    suite.id_usuario = @id_user
                    AND suite.token = @token
                    AND app.id = @id_app

                    Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_user", id_user);
                adapter.SelectCommand.Parameters.AddWithValue("@token", token);
                adapter.SelectCommand.Parameters.AddWithValue("@id_app", id_app);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                if(table.Rows[0]["usuario"].ToString() != "")
                {
                    Session["User"] =           table.Rows[0]["usuario"].ToString();
                    Session["Email"] =          table.Rows[0]["email"].ToString();
                    Session["LastName"] =       table.Rows[0]["apellidos"].ToString();
                    Session["Name"] =           table.Rows[0]["nombres"].ToString();
                    Session["NameAndUser"] =    table.Rows[0]["nombre_usuario"].ToString();
                   
                    Session["token"] =          token;
                    Session["navegador"] =      navegador;

                    session SS = new session();
                    SS.InsertSession(id_app);
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(table, Formatting.None));
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_user: {0}, token: {1}, id_app: {2}, navegador: {3}", id_user, token, id_app, navegador));
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
    public string error_iframe(string App)
    {
        JObject result = new JObject();

        result["ESTADO"] = "TRUE";
        result["MENSAJE"] = "Consulta Correcta.";
        string host = HttpContext.Current.Request.Url.Host;
        Mail.SendEmail_Error(host, App);
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string error_url_DB(string App, string url_real, string url_BD)
    {
        JObject result = new JObject();

        result["ESTADO"] = "TRUE";
        result["MENSAJE"] = "Consulta Correcta.";
        string host = HttpContext.Current.Request.Url.Host;
        Mail.Error_URL(host, App, url_real, url_BD);
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

}
