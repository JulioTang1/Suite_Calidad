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
using System.IO;
using System.Drawing;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Threading.Tasks;

public class session : System.Web.Services.WebService
{

    [WebMethod(EnableSession = true)]
    public string InsertSession(int id_app)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {

                adapter = new SqlDataAdapter(String.Format(@"
                    SELECT
	                    COUNT(*)
                    FROM
	                    {0}.T_usuario_login T
                    INNER JOIN {0}.usuario U ON T.id_usuario = U.id
                    WHERE
	                    U.usuario = @login
                    AND T.token = @token

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@login", Session["User"]);
                adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet dt_Login = new DataSet();
                adapter.Fill(dt_Login);
                DataTable existsLogin = dt_Login.Tables[0];

                int count_existsLogin = int.Parse(existsLogin.Rows[0][0].ToString());

                if (count_existsLogin == 0)
                {
                    adapter = new SqlDataAdapter(String.Format(@"
                        INSERT INTO {0}.T_usuario_login
                        (id_usuario, fecha_sesion, token,navegador, id_aplicacion)
                        SELECT
	                        U.id,
	                        DATEADD(HOUR,-5,GETUTCDATE()),
	                        @token, 
	                        @navegador,
	                        @id_app
                        FROM
	                        {0}.usuario U
                        WHERE
	                        U.usuario = @login;

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@login", Session["User"]);
                    adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                    adapter.SelectCommand.Parameters.AddWithValue("@navegador", Session["navegador"]);
                    adapter.SelectCommand.Parameters.AddWithValue("@id_app", id_app);
                    adapter.SelectCommand.CommandTimeout = 90;
                    adapter.SelectCommand.ExecuteScalar();
                }

            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"Usuario: {0}, token: {1}", Session["User"], Session["token"]));
            }
        }
        return "";
    }

    [WebMethod(EnableSession = true)]
    public string UpdateSession()
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE T
                    SET 
	                    T.fecha_ultima_actividad = DATEADD(HOUR,-5,GETUTCDATE()),
	                    T.duracion = CONVERT(CHAR (8), DATEADD(SECOND, DATEDIFF(SS, T.fecha_sesion, DATEADD(HOUR,-5,GETUTCDATE())), '00:00:00'), 108)
                    FROM
	                    {0}.T_usuario_login T
                    INNER JOIN {0}.usuario U ON T.id_usuario = U.id
                    WHERE
	                    U.usuario = @login
                    AND T.token = @token

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@login", Session["User"]);
                adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                adapter.SelectCommand.CommandTimeout = 90;
                adapter.SelectCommand.ExecuteScalar();

            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"Usuario: {0}, token: {1}", Session["User"], Session["token"]));
            }
        }
        return "";
    }

    [WebMethod(EnableSession = true)]
    public string InsertSessionMenu(int id_menu)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO {0}.T_login_menus
                    (id_usuario_login, id_menu, fecha_menu_select)
                    SELECT
	                    T.id,
	                    @id_menu,
	                    DATEADD(HOUR,-5,GETUTCDATE())
                    FROM
	                    {0}.T_usuario_login T
                    INNER JOIN {0}.usuario U ON T.id_usuario = U.id
                    WHERE
	                    U.usuario = @login
                    AND T.token = @token

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@login", Session["User"]);
                adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                adapter.SelectCommand.Parameters.AddWithValue("@id_menu", id_menu);
                adapter.SelectCommand.CommandTimeout = 90;
                adapter.SelectCommand.ExecuteScalar();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_user: {0}, id_menu: {1}", Session["User"], id_menu));
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
