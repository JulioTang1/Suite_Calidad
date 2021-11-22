using System;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Globalization;
using System.IO;

public class Bioseguridad : WebService
{
    //CONSULTA DE SELECTORES ANIDADOS
    [WebMethod(EnableSession = true)]
    public string consulta_selectores(string filter)
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
	            DP.id AS id_departamento,
	            DP.nombre AS departamento,
	            MP.id AS id_municipio,
	            MP.nombre AS municipio,
	            F.id AS id_finca,
	            CONCAT(F.codigo, ' - ', F.nombre) AS finca
            FROM dbo.M_recurso AS DP
            INNER JOIN dbo.M_estructura_recurso AS DPE
            ON DP.id = DPE.id_recurso_conjunto
            AND DP.id_M_categoria_recurso = 1 --DEPARTAMENTO
            INNER JOIN dbo.M_recurso AS MP
            ON DPE.id_recurso_subconjunto = MP.id
            INNER JOIN dbo.M_estructura_recurso AS MPE
            ON MP.id = MPE.id_recurso_conjunto
            RIGHT JOIN dbo.M_recurso AS F
            ON MPE.id_recurso_subconjunto = F.id
            WHERE F.id_M_categoria_recurso = 5 --FINCA
        ");
        /*Se buca cual selector hizo la petición*/
        if (int.Parse(filtros["departamento"]["state"].ToString()) == 1)
        {
            consultaBegin = @"SELECT MAIN.id_departamento AS id, MAIN.departamento AS name
                                    FROM
	                        (";
            consultaEnd = @") AS MAIN
                                GROUP BY id_departamento, departamento
                                ORDER BY departamento
                                ";
        }
        else if (int.Parse(filtros["municipio"]["state"].ToString()) == 1)
        {
            consultaBegin = @"SELECT MAIN.id_municipio AS id, MAIN.municipio AS name
                                    FROM
	                        (";
            consultaEnd = @") AS MAIN
                                GROUP BY id_municipio, municipio
                                ORDER BY municipio
                                ";
        }
        else if (int.Parse(filtros["finca"]["state"].ToString()) == 1)
        {
            consultaBegin = @"SELECT MAIN.id_finca AS id, MAIN.finca AS name
                                    FROM
	                        (";
            consultaEnd = @") AS MAIN
                                GROUP BY id_finca, finca
                                ORDER BY finca
                                ";
        }
        else { }

        //Se arma el where de los filtros
        if (filtros["departamento"]["data"].ToString() != "0" && int.Parse(filtros["departamento"]["state"].ToString()) != 1)
        {
            where = @"AND DP.id IN (" + filtros["departamento"]["data"].ToString() + @")
            ";
        }
        if (filtros["municipio"]["data"].ToString() != "0" && int.Parse(filtros["municipio"]["state"].ToString()) != 1)
        {
            where += @"                AND MP.id IN (" + filtros["municipio"]["data"].ToString() + @")
            ";
        }
        if (filtros["finca"]["data"].ToString() != "0" && int.Parse(filtros["finca"]["state"].ToString()) != 1)
        {
            where += @"                AND F.id IN (" + filtros["finca"]["data"].ToString() + @")
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

    //CONSULTA DATOS DE UNA VISITA DE BIOSEGURIDAD PARA MOSTAR FORMULARIO
    [WebMethod(EnableSession = true)]
    public string visita(int id_visita)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    IPC.id,
						CASE
							WHEN IPC.id_indicador < 29 THEN IPC.id_indicador - 14
							WHEN IPC.id_indicador BETWEEN 29 AND 35 THEN IPC.id_indicador - 28
							WHEN IPC.id_indicador BETWEEN 36 AND 39 THEN IPC.id_indicador - 35
							WHEN IPC.id_indicador BETWEEN 40 AND 44 THEN IPC.id_indicador - 39
							ELSE ''
						END AS numero,
						I.indicador AS pregunta,
	                    CASE
		                    WHEN IPC.id_indicador < 45 AND IPC.valor = 1 THEN 'Si'
		                    WHEN IPC.id_indicador < 45 AND IPC.valor = 2 THEN 'No'
		                    WHEN IPC.id_indicador < 45 AND IPC.valor = 3 THEN 'N/A'
		                    ELSE IPC.valor_texto
	                    END AS respuesta,
	                    CASE
		                    WHEN SUM(ISNULL(FI.id, 0)) > 0 THEN 1
		                    ELSE 0
	                    END AS foto
                    --Puntos captura bioseguridad para llegar a los indicadores de chequeos
                    FROM dbo.D_punto_captura AS PC
                    --Edades de las plantas
                    INNER JOIN dbo.D_planta_punto_captura AS PPC
                    ON PC.id = PPC.id_punto_captura
                    --Chequeos
                    INNER JOIN dbo.D_indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta
                    INNER JOIN dbo.M_indicadores AS I
                    ON IPC.id_indicador = I.id
                    --Consulta de cantidad de fotos
                    LEFT JOIN dbo.D_fotos_indicador AS FI
                    ON IPC.id = FI.id_lectura_indicador
                    WHERE PC.id_visita = @id_visita
                    GROUP BY
	                    IPC.id,
						CASE
							WHEN IPC.id_indicador < 29 THEN CONCAT('<b>', IPC.id_indicador - 14, '.</b> ')
							WHEN IPC.id_indicador BETWEEN 29 AND 35 THEN CONCAT('<b>', IPC.id_indicador - 28, '.</b> ')
							WHEN IPC.id_indicador BETWEEN 36 AND 39 THEN CONCAT('<b>', IPC.id_indicador - 35, '.</b> ')
							WHEN IPC.id_indicador BETWEEN 40 AND 44 THEN CONCAT('<b>', IPC.id_indicador - 39, '.</b> ')
							ELSE ''
						END,
						I.indicador,
	                    CASE
		                    WHEN IPC.id_indicador < 45 AND IPC.valor = 1 THEN 'Si'
		                    WHEN IPC.id_indicador < 45 AND IPC.valor = 2 THEN 'No'
		                    WHEN IPC.id_indicador < 45 AND IPC.valor = 3 THEN 'N/A'
		                    ELSE IPC.valor_texto
	                    END,
	                    IPC.id_indicador
                    ORDER BY
	                    IPC.id_indicador ASC

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_visita", id_visita);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable chequeos = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(chequeos, Formatting.None));
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_visita={0}", id_visita));
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

    //CONSULTA NOMBRES DE FOTOS DE UN CHEQUEO DE BIOSEGURIDAD
    [WebMethod(EnableSession = true)]
    public string fotos(int id_indicador)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT url
                    FROM dbo.D_fotos_indicador
                    WHERE id_lectura_indicador = @id_indicador
                    ORDER BY id

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_indicador", id_indicador);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable fotos = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(fotos, Formatting.None));
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_indicador={0}", id_indicador));
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

    //Elimina datos de una vista
    [WebMethod(EnableSession = true)]
    public string delete_visita(int id_visita)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT FI.url
                    FROM dbo.D_fotos_indicador AS FI
                    INNER JOIN dbo.D_indicadores_punto_captura AS IPC
                    ON FI.id_lectura_indicador = IPC.id
                    INNER JOIN dbo.D_planta_punto_captura AS PPC
                    ON IPC.id_planta = PPC.id
                    INNER JOIN dbo.D_punto_captura AS PC
                    ON PPC.id_punto_captura = PC.id
                    INNER JOIN dbo.D_visita AS V
                    ON PC.id_visita = V.id
                    AND V.id = @id_visita

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_visita", id_visita);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable tabla = dt.Tables[0];

                adapter = new SqlDataAdapter(String.Format(@"

	                DELETE FI
	                FROM dbo.D_fotos_indicador AS FI
	                INNER JOIN dbo.D_indicadores_punto_captura AS IPC
	                ON FI.id_lectura_indicador = IPC.id
	                INNER JOIN dbo.D_planta_punto_captura AS PPC
	                ON IPC.id_planta = PPC.id
	                INNER JOIN dbo.D_punto_captura AS PC
	                ON PPC.id_punto_captura = PC.id
	                INNER JOIN dbo.D_visita AS V
	                ON PC.id_visita = V.id
	                AND V.id = @id_visita;

	                DELETE IPC
	                FROM dbo.D_indicadores_punto_captura AS IPC
	                INNER JOIN dbo.D_planta_punto_captura AS PPC
	                ON IPC.id_planta = PPC.id
	                INNER JOIN dbo.D_punto_captura AS PC
	                ON PPC.id_punto_captura = PC.id
	                INNER JOIN dbo.D_visita AS V
	                ON PC.id_visita = V.id
	                AND V.id = @id_visita;
	
	                DELETE PPC
	                FROM dbo.D_planta_punto_captura AS PPC
	                INNER JOIN dbo.D_punto_captura AS PC
	                ON PPC.id_punto_captura = PC.id
	                INNER JOIN dbo.D_visita AS V
	                ON PC.id_visita = V.id
	                AND V.id = @id_visita;
	
	                DELETE PC
	                FROM dbo.D_punto_captura AS PC
	                INNER JOIN dbo.D_visita AS V
	                ON PC.id_visita = V.id
	                AND V.id = @id_visita;
	
	                DELETE RV
	                FROM dbo.D_recorrido_visita AS RV
	                INNER JOIN dbo.D_visita AS V
	                ON RV.id_visita = V.id
	                AND V.id = @id_visita;
	
	                DELETE
	                FROM dbo.D_visita
	                WHERE id = @id_visita;

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_visita", id_visita);
                adapter.SelectCommand.CommandTimeout = 1800;
                adapter.SelectCommand.ExecuteScalar();

                //Eliminacion de fotos de la visita guardadas en el servidor
                string pathBase;
                for (int i = 0; i < tabla.Rows.Count; i++)
                {
                    pathBase = HttpContext.Current.Server.MapPath(string.Format(@"~/Fotos/{0}", tabla.Rows[i]["url"].ToString()));

                    if (File.Exists(pathBase))
                    {
                        File.Delete(pathBase);
                    }
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
                Mail.SendEmail(e, host, string.Format(@"id_visita={0}", id_visita));
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