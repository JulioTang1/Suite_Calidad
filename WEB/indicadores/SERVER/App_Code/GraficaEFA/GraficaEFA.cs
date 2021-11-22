using System;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Globalization;

public class GraficaEFA : WebService
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
}
