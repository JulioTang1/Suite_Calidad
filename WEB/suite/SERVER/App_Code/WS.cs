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

/* NOMBRE DEL ESQUEMA */
public static class Abi_maestro
{
    public const string esquema = "dbo";
}

public class WS : System.Web.Services.WebService
{

    /******************************************** Login ****************************************************************/

    public string query()
    {
        string Q = String.Format(@"

            Begin Transaction;
            SELECT
	            APP.id,
	            APP.nombre,
	            APP.titulo_login,
	            APP.url_login_img,
	            APP.descripcion,
	            APP.url_website,
	            APP.url_website_suite,
	            CAT.nombre AS [cat_nombre],
	            on_line = (
		            SELECT
			            COUNT(DISTINCT T.id_usuario)
		            FROM
			            {0}.T_usuario_login T
		            WHERE
			            T.fecha_ultima_actividad BETWEEN DATEADD(MINUTE,-15,(DATEADD(HOUR,-5,GETUTCDATE()))) AND DATEADD(HOUR,-5,GETUTCDATE())
		            AND T.id_aplicacion = APP.id
	            ),
	            CASE
		            WHEN RA.id_nivel_acceso <> 3 THEN 1
		            ELSE 0
	            END AS [Acceso]
            FROM
	            {0}.aplicacion APP
            INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
            INNER JOIN {0}.rol_aplicacion RA ON RA.id_app = APP.id
            INNER JOIN {0}.rol R ON RA.id_rol = R.id
            INNER JOIN {0}.estado_usuario EU ON EU.id_rol = R.id
            INNER JOIN {0}.usuario U ON EU.id_usuario = U.id
            AND U.usuario = @usuario
            WHERE
	            APP.enable = 1

            GROUP BY
	            APP.id,
	            APP.nombre,
	            APP.titulo_login,
	            APP.url_login_img,
	            APP.descripcion,
	            APP.url_website,
	            APP.url_website_suite,
	            CAT.nombre,
	            RA.id_nivel_acceso;

            SELECT
	            CAT.nombre AS [cat_nombre],
	            CAT.background,
	            CAT.color_letra
            FROM
	            {0}.aplicacion APP
            INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
            WHERE
	            APP.enable = 1
            GROUP BY
	            CAT.nombre,
	            CAT.background,
	            CAT.color_letra,
                CAT.orden
            ORDER BY
	            CAT.orden;

            SELECT
	            U.id AS ID_user,
	            U.url_foto AS Foto,
	            U.usuario AS usuario,
	            U.nombres AS nombres,
	            U.apellidos AS apellidos,
	            U.correo AS email,
	            U.nombre_usuario AS nombre_usuario,
                @token AS [Token]
            FROM
	            {0}.usuario U
            WHERE
	            U.usuario = @usuario;

            Commit Transaction;

        ", Abi_maestro.esquema);

        return Q;
    }

    /* CONSULTA PARA INGRESAR A LA APLICACIÓN */
    [WebMethod(EnableSession = true)]
    public string Login()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Informacion del usuario
                string usuario = Session["Email"].ToString();
                string email = Session["Email"].ToString();
                string apellido = Session["LastName"].ToString();
                string nombre = Session["Name"].ToString();
                string nombre_completo = nombre + " " + apellido;

                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    COUNT(*)
                    FROM
	                    {0}.usuario U
                    WHERE
	                    U.usuario = @usuario

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet dtUser = new DataSet();
                adapter.Fill(dtUser);
                DataTable existsUser = dtUser.Tables[0];

                int count_existsUser = int.Parse(existsUser.Rows[0][0].ToString());

                if (count_existsUser == 0)
                {
                    adapter = new SqlDataAdapter(String.Format(@"
                        SELECT
	                        @user AS usuario,
	                        @nombres AS nombres,
	                        @apellidos AS apellidos,
	                        @correo AS email,
	                        @nombre_usuario AS nombre_usuario

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@user", usuario);
                    adapter.SelectCommand.Parameters.AddWithValue("@nombres", nombre);
                    adapter.SelectCommand.Parameters.AddWithValue("@apellidos", apellido);
                    adapter.SelectCommand.Parameters.AddWithValue("@correo", email);
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre_usuario", nombre_completo);
                    adapter.SelectCommand.CommandTimeout = 90;
                    DataSet dt_error = new DataSet();
                    adapter.Fill(dt_error);
                    DataTable table_User = dt_error.Tables[0];

                    result["ESTADO"] = "TRUE";
                    result["MENSAJE"] = "Consulta Correcta.";
                    result["RESULTADO"] = new JObject();
                    result["RESULTADO"]["USER"] = JArray.Parse(JsonConvert.SerializeObject(table_User, Formatting.None));
                    result["RESULTADO"]["ESTADO"] = "FALSE";
                }
                else
                {
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE {0}.usuario
                        SET
                            nombres = @nombre,
                            apellidos = @apellido,
                            correo = @correo,
                            nombre_usuario = @nombre_completo
                        WHERE
                            usuario = @usuario;

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombre);
                    adapter.SelectCommand.Parameters.AddWithValue("@apellido", apellido);
                    adapter.SelectCommand.Parameters.AddWithValue("@correo", email);
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre_completo", nombre_completo);
                    adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                    adapter.SelectCommand.CommandTimeout = 90;
                    adapter.SelectCommand.ExecuteScalar();

                    string Query = this.query();

                    adapter = new SqlDataAdapter(Query, conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                    adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                    adapter.SelectCommand.CommandTimeout = 90;
                    DataSet dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable apps = dt.Tables[0];
                    DataTable cat_apps = dt.Tables[1];
                    DataTable table_User = dt.Tables[2];

                    int ID_user = int.Parse(table_User.Rows[0]["ID_user"].ToString());

                    adapter = new SqlDataAdapter(String.Format(@"
                        SELECT
	                        COUNT(*)
                        FROM
	                        {0}.T_usuario_suite T
                        WHERE
	                        T.id_usuario = @id_user
                        AND T.token = @token

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_user", ID_user);
                    adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                    adapter.SelectCommand.CommandTimeout = 90;
                    DataSet dt_Login = new DataSet();
                    adapter.Fill(dt_Login);
                    DataTable existsLogin = dt_Login.Tables[0];

                    int count_existsLogin = int.Parse(existsLogin.Rows[0][0].ToString());

                    if (count_existsLogin == 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"
                            INSERT INTO {0}.T_usuario_suite
                            (id_usuario, fecha_sesion, token)
                            VALUES
                            (@id_user,DATEADD(HOUR,-5,GETUTCDATE()), @token);

                        ", Abi_maestro.esquema), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id_user", ID_user);
                        adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                        adapter.SelectCommand.CommandTimeout = 90;
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    result["ESTADO"] = "TRUE";
                    result["MENSAJE"] = "Consulta Correcta.";
                    result["RESULTADO"] = new JObject();
                    result["RESULTADO"]["APP"] = JArray.Parse(JsonConvert.SerializeObject(apps, Formatting.None));
                    result["RESULTADO"]["CAT_APP"] = JArray.Parse(JsonConvert.SerializeObject(cat_apps, Formatting.None));
                    result["RESULTADO"]["USER"] = JArray.Parse(JsonConvert.SerializeObject(table_User, Formatting.None));
                    result["RESULTADO"]["ESTADO"] = "TRUE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"Usuario: {0}, token: {1}", Session["User"], Session["token"]));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR_CONEXION";
            conexion.closeConexion();
        }
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string Reg_User()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Informacion del usuario
                string usuario = Session["Email"].ToString();
                string correo = Session["Email"].ToString();
                string apellidos = Session["LastName"].ToString();
                string nombres = Session["Name"].ToString();
                string nombre_usuario = nombres + " " + apellidos;

                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    COUNT(*)
                    FROM
	                    {0}.usuario U
                    WHERE
	                    U.usuario = @usuario

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                DataSet dtUser = new DataSet();
                adapter.Fill(dtUser);
                DataTable existsUser = dtUser.Tables[0];

                int count_existsUser = int.Parse(existsUser.Rows[0][0].ToString());
                
                if (count_existsUser == 0)
                {
                    string ext = HttpContext.Current.Request.Form.Get("ext");
                    //Si no se envía una imagen, la extención tendra un string 0
                    //si hay una imagen, se guarda y se pone su dirección, si no, este campo en la tabla estara vacío
                    if (ext != "0")
                    {
                        //SE MAPEA LA RUTA DONDE SE GUARDARA EL ARCHIVO
                        String pathBase = HttpContext.Current.Server.MapPath("~/Img_Users/");
                        //SE OBTIENE EL ARCHIVO DE IMAGEN
                        HttpPostedFile file = HttpContext.Current.Request.Files.Get(0);

                        string idArchivo = usuario;
                        string ext_file = String.Format(@".{0}", ext);
                        string path = pathBase + idArchivo + ext_file;

                        //SE BORRA EL ARCHIVO SI YA EXISTE
                        if (File.Exists(path))
                        {
                            File.Delete(path);
                        }
                        //SE GUARDA EL ARCHIVO EN LA RUTA ESPECIFICADA
                        file.SaveAs(path);

                        //ruta de la foto
                        string urlFoto = string.Format(@"../apiServices/Img_Users/{0}.{1}", usuario, ext);

                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO {0}.usuario
                            (usuario, nombres, apellidos, correo, url_foto, nombre_usuario)
                            VALUES
                            (@user, @nombres, @apellidos, @correo, @urlFoto, @nombre_usuario); 

                        ", Abi_maestro.esquema), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@user", usuario);
                        adapter.SelectCommand.Parameters.AddWithValue("@nombres", nombres);
                        adapter.SelectCommand.Parameters.AddWithValue("@apellidos", apellidos);
                        adapter.SelectCommand.Parameters.AddWithValue("@correo", correo);
                        adapter.SelectCommand.Parameters.AddWithValue("@urlFoto", urlFoto);
                        adapter.SelectCommand.Parameters.AddWithValue("@nombre_usuario", nombre_usuario);
                        adapter.SelectCommand.CommandTimeout = 90;
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO {0}.usuario
                            (usuario, nombres, apellidos, correo, url_foto, nombre_usuario)
                            VALUES
                            (@user, @nombres, @apellidos, @correo, '', @nombre_usuario); 

                        ", Abi_maestro.esquema), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@user", usuario);
                        adapter.SelectCommand.Parameters.AddWithValue("@nombres", nombres);
                        adapter.SelectCommand.Parameters.AddWithValue("@apellidos", apellidos);
                        adapter.SelectCommand.Parameters.AddWithValue("@correo", correo);
                        adapter.SelectCommand.Parameters.AddWithValue("@nombre_usuario", nombre_usuario);
                        adapter.SelectCommand.CommandTimeout = 90;
                        adapter.SelectCommand.ExecuteScalar();
                    }

                    adapter = new SqlDataAdapter(String.Format(@"

                    DECLARE @id_user INT

                    SELECT
	                    @id_user = U.id
                    FROM
	                    {0}.usuario U
                    WHERE
	                    U.usuario = @usuario

                    INSERT INTO {0}.estado_usuario
                    VALUES
                    (@id_user, 2, 1);

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                    adapter.SelectCommand.CommandTimeout = 90;
                    adapter.SelectCommand.ExecuteScalar();

                    if(correo != "")
                    {
                        Mail.SendEmailNewUser(nombre_usuario, correo);
                    }
                }
                else
                {
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE {0}.usuario
                        SET
                            nombres = @nombre,
                            apellidos = @apellido,
                            correo = @correo,
                            nombre_usuario = @nombre_usuario
                        WHERE
                            usuario = @usuario;

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombres);
                    adapter.SelectCommand.Parameters.AddWithValue("@apellido", apellidos);
                    adapter.SelectCommand.Parameters.AddWithValue("@correo", correo);
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre_usuario", nombre_usuario);
                    adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                    adapter.SelectCommand.CommandTimeout = 90;
                    adapter.SelectCommand.ExecuteScalar();
                }

                string Query = this.query();

                adapter = new SqlDataAdapter(Query, conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@usuario", usuario);
                adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable apps = dt.Tables[0];
                DataTable cat_apps = dt.Tables[1];
                DataTable table_User = dt.Tables[2];

                int ID_user = int.Parse(table_User.Rows[0]["ID_user"].ToString());

                adapter = new SqlDataAdapter(String.Format(@"
                    SELECT
	                    COUNT(*)
                    FROM
	                    {0}.T_usuario_suite T
                    WHERE
	                    T.id_usuario = @id_user
                    AND T.token = @token

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_user", ID_user);
                adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet dt_Login = new DataSet();
                adapter.Fill(dt_Login);
                DataTable existsLogin = dt_Login.Tables[0];

                int count_existsLogin = int.Parse(existsLogin.Rows[0][0].ToString());

                if (count_existsLogin == 0)
                {
                    adapter = new SqlDataAdapter(String.Format(@"
                        INSERT INTO {0}.T_usuario_suite
                        (id_usuario, fecha_sesion, token)
                        VALUES
                        (@id_user,DATEADD(HOUR,-5,GETUTCDATE()), @token);

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_user", ID_user);
                    adapter.SelectCommand.Parameters.AddWithValue("@token", Session["token"]);
                    adapter.SelectCommand.CommandTimeout = 90;
                    adapter.SelectCommand.ExecuteScalar();
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["APP"] = JArray.Parse(JsonConvert.SerializeObject(apps, Formatting.None));
                result["RESULTADO"]["CAT_APP"] = JArray.Parse(JsonConvert.SerializeObject(cat_apps, Formatting.None));
                result["RESULTADO"]["USER"] = JArray.Parse(JsonConvert.SerializeObject(table_User, Formatting.None));
                conexion.closeConexion();

            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"Usuario: {0}, token: {1}", Session["User"], Session["token"]));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string closeSession()
    {

        JObject result = new JObject();
        Session.Abandon();

        result["ESTADO"] = "TRUE";
        result["MENSAJE"] = "Consulta Correcta.";
        result["RESULTADO"] = "";
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    /* METODO PARA EDITAR LA FOTO DE PERFIL */
    [WebMethod(EnableSession = true)]
    public string edit_Photo()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE MAPEA LA RUTA DONDE SE GUARDARA EL ARCHIVO
                String pathBase = HttpContext.Current.Server.MapPath("~/Img_Users/");
                //SE OBTIENE EL ARCHIVO DE IMAGEN
                HttpPostedFile file = HttpContext.Current.Request.Files.Get(0);

                //Informacion del usuario

                string user = HttpContext.Current.Request.Form.Get("user");
                string ext = HttpContext.Current.Request.Form.Get("ext");

                //SE VERIFICA QUE EL ARCHIVO SEA DE EXCEL -- FALTA

                string idArchivo = user;
                string ext_file = String.Format(@".{0}", ext);
                string path = pathBase + idArchivo + ext_file;

                //SE BORRA EL ARCHIVO SI YA EXISTE
                if (File.Exists(path))
                {
                    File.Delete(path);
                }
                //SE GUARDA EL ARCHIVO EN LA RUTA ESPECIFICADA
                file.SaveAs(path);

                if (File.Exists(path))
                {
                    adapter = new SqlDataAdapter(String.Format(@"
                        Begin Transaction;
                        UPDATE
                            {0}.usuario
                        SET
                            url_foto = '../apiServices/Img_Users/{1}.{2}'
                        WHERE
                            usuario = '{1}';

                        SELECT
	                        url_foto
                        FROM
	                        {0}.usuario
                        WHERE
	                        usuario = '{1}';

                        Commit Transaction;
                    ", Abi_maestro.esquema, user, ext), conexion.getConexion());
                    DataSet dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable tabla = dt.Tables[0];

                    result["ESTADO"] = "TRUE";
                    result["MENSAJE"] = "Consulta Correcta.";
                    result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(tabla, Formatting.None));

                    conexion.closeConexion();
                }
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
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string updUserOnline()
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
	                    APP.id,
	                    APP.nombre,
	                    APP.titulo_login,
	                    APP.url_login_img,
	                    APP.descripcion,
	                    APP.url_website,
	                    APP.url_website_suite,
	                    CAT.nombre AS [cat_nombre],
	                    on_line = (
		                    SELECT
			                    COUNT(DISTINCT T.id_usuario)
		                    FROM
			                    {0}.T_usuario_login T
		                    WHERE
			                    T.fecha_ultima_actividad BETWEEN DATEADD(MINUTE,-15,(DATEADD(HOUR,-5,GETUTCDATE()))) AND DATEADD(HOUR,-5,GETUTCDATE())
		                    AND T.id_aplicacion = APP.id
	                    ),
	                    CASE
		                    WHEN RA.id_nivel_acceso <> 3 THEN 1
		                    ELSE 0
	                    END AS [Acceso]
                    FROM
	                    {0}.aplicacion APP
                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
                    INNER JOIN {0}.rol_aplicacion RA ON RA.id_app = APP.id
                    INNER JOIN {0}.rol R ON RA.id_rol = R.id
                    INNER JOIN {0}.estado_usuario EU ON EU.id_rol = R.id
                    INNER JOIN {0}.usuario U ON EU.id_usuario = U.id
                    AND U.usuario = @user
                    WHERE
	                    APP.enable = 1

                    GROUP BY
	                    APP.id,
	                    APP.nombre,
	                    APP.titulo_login,
	                    APP.url_login_img,
	                    APP.descripcion,
	                    APP.url_website,
	                    APP.url_website_suite,
	                    CAT.nombre,
	                    RA.id_nivel_acceso;

                    Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@user", Session["Email"].ToString());
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet U_online = new DataSet();
                adapter.Fill(U_online);
                DataTable userOnline = U_online.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(userOnline, Formatting.None));
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
    public string updateSession()
    {
        JObject result = new JObject();

        result["ESTADO"] = "TRUE";
        result["MENSAJE"] = "Consulta Correcta.";
         
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

}