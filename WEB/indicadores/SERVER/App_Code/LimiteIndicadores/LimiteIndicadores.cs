using System;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;

public class LimiteIndicadores : WebService
{
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
	            I.abreviatura AS indicador,
	            SI.organica AS id_tipo_finca,
	            CASE
		            WHEN SI.organica = 1 THEN 'Orgánica' 
		            ELSE 'Convencional'
	            END AS tipo_finca
            FROM dbo.M_edad_indicador AS EI
            INNER JOIN dbo.M_edades AS E
            ON EI.id_edad = E.id
            INNER JOIN dbo.M_indicadores AS I
            ON EI.id_indicador = I.id
            INNER JOIN dbo.M_semaforo_indicador AS SI
            ON EI.id = SI.id_edad_indicador

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
        else if (int.Parse(filtros["tipoFinca"]["state"].ToString()) == 1)
        {
            consultaBegin = @"SELECT MAIN.id_tipo_finca AS id, MAIN.tipo_finca AS name
                                    FROM
	                        (";
            consultaEnd = @") AS MAIN
                                GROUP BY id_tipo_finca, tipo_finca
                                ORDER BY tipo_finca
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
        if (filtros["tipoFinca"]["data"].ToString() != "0" && int.Parse(filtros["tipoFinca"]["state"].ToString()) != 1)
        {
            where += @"AND SI.organica IN (" + filtros["tipoFinca"]["data"].ToString() + @")
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

    //CONSULTA DE LIMITES Y COLORES PARA LOS RANGOS DE LOS INDICADORES
    [WebMethod(EnableSession = true)]
    public string info(int edad, int infeccion, int tipoFinca)
    {
        JObject result = new JObject(); //Aca se guarda el JObject consultas
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    SI.orden,
	                    SI.[min],
	                    SI.[max],
	                    SI.color,
                        EI.semaforo_activo,
                        SI.radio_mapa
                    FROM dbo.M_edad_indicador AS EI
                    INNER JOIN dbo.M_semaforo_indicador AS SI
                    ON EI.id = SI.id_edad_indicador
                    WHERE EI.id_edad = @edad
                    AND EI.id_indicador = @infeccion
                    AND SI.organica = @tipoFinca
                    ORDER BY
	                    SI.orden

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@edad", edad);
                adapter.SelectCommand.Parameters.AddWithValue("@infeccion", infeccion);
                adapter.SelectCommand.Parameters.AddWithValue("@tipoFinca", tipoFinca);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

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

    //CONSULTA DE LIMITES Y COLORES PARA LOS RANGOS DE LOS INDICADORES
    [WebMethod(EnableSession = true)]
    public string save(float min, float max, string color1, string color2, string color3, int edad, int infeccion, int tipoFinca, int semaforo_activo, int radio)
    {
        JObject result = new JObject(); //Aca se guarda el JObject consultas
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE SI
                    SET
	                    SI.[max] = ROUND (@min, 2),
	                    SI.color = @color1,
                        SI.radio_mapa = @radio
                    FROM dbo.M_semaforo_indicador AS SI
                    INNER JOIN dbo.M_edad_indicador AS EI
                    ON SI.id_edad_indicador = EI.id
                    AND EI.id_edad = @edad
                    AND EI.id_indicador = @infeccion
                    WHERE SI.orden = 1
                    AND SI.organica = @tipoFinca

                    UPDATE SI
                    SET
	                    SI.[min] = ROUND (@min, 2),
	                    SI.[max] = ROUND (@max, 2),
	                    SI.color = @color2,
                        SI.radio_mapa = @radio                   
                    FROM dbo.M_semaforo_indicador AS SI
                    INNER JOIN dbo.M_edad_indicador AS EI
                    ON SI.id_edad_indicador = EI.id
                    AND EI.id_edad = @edad
                    AND EI.id_indicador = @infeccion
                    WHERE SI.orden = 2
                    AND SI.organica = @tipoFinca

                    UPDATE SI
                    SET
	                    SI.[min] = ROUND (@max, 2),	
	                    SI.color = @color3,
                        SI.radio_mapa = @radio                 
                    FROM dbo.M_semaforo_indicador AS SI
                    INNER JOIN dbo.M_edad_indicador AS EI
                    ON SI.id_edad_indicador = EI.id
                    AND EI.id_edad = @edad
                    AND EI.id_indicador = @infeccion
                    WHERE SI.orden = 3
                    AND SI.organica = @tipoFinca

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@min", min);
                adapter.SelectCommand.Parameters.AddWithValue("@max", max);
                adapter.SelectCommand.Parameters.AddWithValue("@color1", color1);
                adapter.SelectCommand.Parameters.AddWithValue("@color2", color2);
                adapter.SelectCommand.Parameters.AddWithValue("@color3", color3);
                adapter.SelectCommand.Parameters.AddWithValue("@edad", edad);
                adapter.SelectCommand.Parameters.AddWithValue("@infeccion", infeccion);
                adapter.SelectCommand.Parameters.AddWithValue("@tipoFinca", tipoFinca);
                adapter.SelectCommand.Parameters.AddWithValue("@semaforo_activo", semaforo_activo);
                adapter.SelectCommand.Parameters.AddWithValue("@radio", radio);
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
