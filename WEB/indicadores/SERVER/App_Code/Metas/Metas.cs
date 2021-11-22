using System;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using Newtonsoft.Json;

public class Metas : WebService
{
    //SE CREA UN REGISTRO DE META
    [WebMethod(EnableSession = true)]
    public string insert_meta(int edad, int indicador, float meta)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        //Objeto que guarda errores de codigo
        JObject error = new JObject();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se verifica si la combinacion ya existe
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    M.id,
	                    EI.id_edad,
	                    EI.id_indicador,
	                    M.meta
                    FROM dbo.M_meta AS M
                    INNER JOIN dbo.M_edad_indicador AS EI
                    ON M.id_edad_indicador = EI.id
                    WHERE EI.id_edad = @edad
                    AND EI.id_indicador = @indicador
                    AND M.meta = @meta

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@edad", edad);
                adapter.SelectCommand.Parameters.AddWithValue("@indicador", indicador);
                adapter.SelectCommand.Parameters.AddWithValue("@meta", meta);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                if (table.Rows.Count == 0)
                {
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_meta
                        (id_edad_indicador, meta)
                        SELECT
	                        id AS id_edad_indicador,
	                        @meta AS meta
                        FROM dbo.M_edad_indicador
                        WHERE id_edad = @edad
                        AND id_indicador = @indicador

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@edad", edad);
                    adapter.SelectCommand.Parameters.AddWithValue("@indicador", indicador);
                    adapter.SelectCommand.Parameters.AddWithValue("@meta", meta);
                    adapter.SelectCommand.ExecuteScalar();
                }
                else
                {
                    result["RESULTADO"] = 1;
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"edad={0}, indicador={1}, meta={2}", edad, indicador, meta));
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

    //SE ELIMINA UN REGISTRO DE META
    [WebMethod(EnableSession = true)]
    public string delete_meta(int id)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        //Objeto que guarda errores de codigo
        JObject error = new JObject();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    DELETE FROM dbo.M_meta
                    WHERE id = @id;

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id", id);
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
                Mail.SendEmail(e, host, string.Format(@"id={0}", id));
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

    //CONSULTA DE SELECTORES ANIDADOS EDADES INDICADORES
    [WebMethod(EnableSession = true)]
    public string consulta_selectores_ei(string filter)
    {
        JObject consultas = new JObject();
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        /*objeto que contiene los ids a filtrar y que selector pertenecen*/
        JObject filtros = JObject.Parse(filter);
        string consultaBegin = @"";
        string consultaEnd = @"";
        string where = @"";
        /*Consulta donde se encuentra todo*/
        string consulta = string.Format(@"
            SELECT
	            E.id AS id_edad,
	            E.edad,
	            I.id AS id_indicador,
	            I.abreviatura AS indicador
            FROM dbo.M_edad_indicador AS EI
            INNER JOIN dbo.M_edades AS E
            ON EI.id_edad = E.id
            INNER JOIN dbo.M_indicadores AS I
            ON EI.id_indicador = I.id
        ");
        /*Se buca cual selector hizo la petición*/
        if (int.Parse(filtros["edad"]["state"].ToString()) == 1)
        {
            consultaBegin = @"SELECT MAIN.id_edad AS id, MAIN.edad AS name
                                    FROM
	                        (";
            consultaEnd = @") AS MAIN
                                GROUP BY id_edad, edad
                                ORDER BY edad
                                ";
        }
        else if (int.Parse(filtros["indicador"]["state"].ToString()) == 1)
        {
            consultaBegin = @"SELECT MAIN.id_indicador AS id, MAIN.indicador AS name
                                    FROM
	                        (";
            consultaEnd = @") AS MAIN
                                GROUP BY id_indicador, indicador
                                ORDER BY indicador
                                ";
        }
        else { }

        //Se arma el where de los filtros
        if (filtros["edad"]["data"].ToString() != "0" && int.Parse(filtros["edad"]["state"].ToString()) != 1)
        {
            where = @"AND E.id IN (" + filtros["edad"]["data"].ToString() + @")
            ";
        }
        if (filtros["indicador"]["data"].ToString() != "0" && int.Parse(filtros["indicador"]["state"].ToString()) != 1)
        {
            where += @"AND I.id IN (" + filtros["indicador"]["data"].ToString() + @")
            ";
        }
        else { }


        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se trae la informacion de paises, plantas o productos
                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                    {0}
                            {1}
                            {2}
                    {3}
                ", consultaBegin, consulta, where, consultaEnd), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable resultado = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(resultado, Formatting.None));
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"filter={0}", filter));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "Error en la conexion:" + conexion.openConexion();
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }
}