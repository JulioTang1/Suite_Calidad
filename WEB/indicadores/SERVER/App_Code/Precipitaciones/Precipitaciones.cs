using System;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Globalization;

public class Precipitaciones : WebService
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

    //CONSULTA DATOS DE PRECIPITACION PARA UNA FINCA EN UNA SEMANA
    [WebMethod(EnableSession = true)]
    public string bring_precipitaciones(string fecha, int id_finca)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    DECLARE @id_finca INT = {0}
                    DECLARE @fecha DATE = '{1}'
                    DECLARE @semana INT
                    DECLARE @año INT

                    SELECT
	                    @semana = Week_of_Year_ISO,
	                    @año = Year_ISO
                    FROM dbo.M_calendario
                    WHERE Calendar_Date = @fecha

                    SELECT
	                    REPLACE(REPLACE(LOWER(C.Day_Name), 'á', 'a'), 'é', 'e') AS id,
	                    C.Day_Name AS dias,
	                    CAST(C.Calendar_Date AS DATE) AS fecha,
	                    REPLACE(CAST(ROUND(ISNULL(P.valor, 0), 2) AS NVARCHAR(255)), '.', ',') AS valor,
	                    CASE
		                    WHEN CAST(C.Calendar_Date AS DATE) > CAST(GETDATE() AS DATE) THEN 0
		                    ELSE 1
	                    END AS editable
                    FROM dbo.M_recurso AS R
                    INNER JOIN dbo.M_calendario AS C
                    ON C.Week_of_Year_ISO = @semana
                    AND C.Year_ISO = @año
                    LEFT JOIN dbo.D_precipitaciones AS P
                    ON CAST(C.Calendar_Date AS DATE) = CAST(P.fecha AS DATE)
                    AND R.id = P.id_recurso
                    WHERE R.id = @id_finca
                    ORDER BY C.Day_of_Week

                ", id_finca, fecha), conexion.getConexion());
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
                Mail.SendEmail(e, host, string.Format(@"fecha={0}, id_finca={1}", fecha, id_finca));
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

	//RECIBE PRECIPITACIONES A GUARDAR
	[WebMethod(EnableSession = true)]
	public string save_precipitaciones(string data, int id_finca, int id_user)
	{
		JObject result = new JObject();
		ConexionSQL conexion = new ConexionSQL();
		SqlDataAdapter adapter = new SqlDataAdapter();
		if ((conexion.openConexion()) == "TRUE")
		{
			try
			{
				JArray dataForm = JArray.Parse(data);
				JObject registro;

				for (int i = 0; i < dataForm.Count; i++)
				{
					registro = JObject.Parse(dataForm[i].ToString());

                    adapter = new SqlDataAdapter(String.Format(@"

                        DECLARE @id_finca INT = {0}
                        DECLARE @fecha DATE = '{1}'
                        DECLARE @id_usuario INT = {2}
                        DECLARE @valor FLOAT(53) = {3}

                        UPDATE dbo.D_precipitaciones
                        SET
	                        id_usuario = @id_usuario,
	                        valor = @valor
                        WHERE id_recurso = @id_finca
                        AND CAST(fecha AS DATE) = @fecha
                        AND valor <> @valor --ACTUALIZA SI SE MODFICO LA PRECIPITACION

                        INSERT INTO dbo.D_precipitaciones
                        (id_recurso, id_usuario, fecha, valor)
                        SELECT
	                        S.id_recurso,
	                        S.id_usuario,
	                        S.fecha,
	                        S.valor
                        FROM
                        (
	                        SELECT
		                        @id_finca AS id_recurso,
		                        @id_usuario AS id_usuario,
		                        @fecha AS fecha,
		                        @valor AS valor
                        ) AS S
                        LEFT JOIN dbo.D_precipitaciones AS P
                        ON S.id_recurso = P.id_recurso
                        AND S.fecha = CAST(P.fecha AS DATE)
                        WHERE P.id IS NULL

                    ", id_finca, registro["fecha"].ToString(), id_user, registro["valor"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();
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
				Mail.SendEmail(e, host, string.Format(@"data={0}, id_finca={1}, id_user={2}", data, id_finca, id_user));
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
