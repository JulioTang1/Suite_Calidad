using System;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Globalization;

public class Datos : WebService
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

    //SE ACTUALIZA LA INFORMACION DE LOS INDICADORES PUNTO CAPTURA (SIGATOKA P10 Y P7)
    [WebMethod(EnableSession = true)]
    public string save_sigatoka_p10_p7(int id_planta, float YLS, float CF, float HF, string Lote)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @YLS
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 2 --YLS

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @CF
                    WHERE id_planta = @id_planta
                    AND id_indicador = 7 --CF

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @HF
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 1 --HF

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor_texto = @Lote
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 51 --Lote

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_planta);
                adapter.SelectCommand.Parameters.AddWithValue("@YLS", YLS);
                adapter.SelectCommand.Parameters.AddWithValue("@CF", CF);
                adapter.SelectCommand.Parameters.AddWithValue("@HF", HF);
                adapter.SelectCommand.Parameters.AddWithValue("@Lote", Lote);
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
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, YLS={1}, CF={2}, HF={3}, HF={4}", id_planta, YLS, CF, HF, Lote));
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

    //SE ACTUALIZA LA INFORMACION DE LOS INDICADORES PUNTO CAPTURA (PXP)
    [WebMethod(EnableSession = true)]
    public string save_sigatoka_pxp(int id_planta, float TH, float YLI, float YLS, float CF, string Lote)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @TH
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 4 --TH

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @YLI
                    WHERE id_planta = @id_planta
                    AND id_indicador = 5 --YLI

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @YLS
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 2 --YLS

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @CF
                    WHERE id_planta = @id_planta
                    AND id_indicador = 7 --CF

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor_texto = @Lote
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 51 --Lote

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_planta);
                adapter.SelectCommand.Parameters.AddWithValue("@TH", TH);
                adapter.SelectCommand.Parameters.AddWithValue("@YLI", YLI);
                adapter.SelectCommand.Parameters.AddWithValue("@YLS", YLS);
                adapter.SelectCommand.Parameters.AddWithValue("@CF", CF);
                adapter.SelectCommand.Parameters.AddWithValue("@Lote", Lote);
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
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, TH={1}, YLI={2}, YLS={3}, CF={4}, CF={5}", id_planta, TH, YLI, YLS, CF, Lote));
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

    //SE ACTUALIZA LA INFORMACION DE LOS INDICADORES PUNTO CAPTURA (FIJA)
    [WebMethod(EnableSession = true)]
    public string save_sigatoka_fija(int id_planta, float TH, float EFA, float CF, float H2, float H3, float H4, string Lote)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @TH
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 4 --TH

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @EFA
                    WHERE id_planta = @id_planta
                    AND id_indicador = 8 --EFA

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @CF
                    WHERE id_planta = @id_planta
                    AND id_indicador = 7 --CF

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @H2
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 48 --H2

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @H3
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 49 --H3

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @H4
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 50 --H4

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor_texto = @Lote
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 51 --Lote

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_planta);
                adapter.SelectCommand.Parameters.AddWithValue("@TH", TH);
                adapter.SelectCommand.Parameters.AddWithValue("@EFA", EFA);
                adapter.SelectCommand.Parameters.AddWithValue("@CF", CF);
                adapter.SelectCommand.Parameters.AddWithValue("@H2", H2);
                adapter.SelectCommand.Parameters.AddWithValue("@H3", H3);
                adapter.SelectCommand.Parameters.AddWithValue("@H4", H4);
                adapter.SelectCommand.Parameters.AddWithValue("@Lote", Lote);
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
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, TH={1}, EFA={2}, CF={3}, H2={4}, H3={5}, H4={6}, Lote={7}", id_planta, TH, EFA, CF, H2, H3, H4, Lote));
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

    //SE ACTUALIZA LA INFORMACION DE LOS INDICADORES DE E.VASCULARES
    [WebMethod(EnableSession = true)]
    public string save_sigatoka_vasculares(int id_planta, float Fusarium, float Moko, float Erwinia)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @Fusarium
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 9 --Fusarium

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @Moko
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 10 --Moko

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @Erwinia
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 11 --Erwinia

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_planta);
                adapter.SelectCommand.Parameters.AddWithValue("@Fusarium", Fusarium);
                adapter.SelectCommand.Parameters.AddWithValue("@Moko", Moko);
                adapter.SelectCommand.Parameters.AddWithValue("@Erwinia", Erwinia);
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
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, Fusarium={1}, Moko={2}, Erwinia={3}", id_planta, Fusarium, Moko, Erwinia));
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

    //SE ACTUALIZA LA INFORMACION DE LOS INDICADORES DE E.VASCULARES
    [WebMethod(EnableSession = true)]
    public string save_sigatoka_culturales(int id_planta, float NF, float FIT, float RTI, string CFIT, string CRTI)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @NF
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 12 --NF

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @FIT
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 13 --FIT

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor =  @RTI
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 14 --RTI

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor_texto =  @CFIT
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 52 --Comentario FIT

                    UPDATE dbo.D_indicadores_punto_captura
                        SET valor_texto =  @CRTI
                    WHERE id_planta = @id_planta 
                    AND id_indicador = 53 --Comentario RTI

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_planta);
                adapter.SelectCommand.Parameters.AddWithValue("@NF", NF);
                adapter.SelectCommand.Parameters.AddWithValue("@FIT", FIT);
                adapter.SelectCommand.Parameters.AddWithValue("@RTI", RTI);
                adapter.SelectCommand.Parameters.AddWithValue("@CFIT", CFIT);
                adapter.SelectCommand.Parameters.AddWithValue("@CRTI", CRTI);
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
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, NF={1}, FIT={2}, RTI={3}, CFIT={4}, CRTI={5}", id_planta, NF, FIT, RTI, CFIT, CRTI));
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

    //SE ACTUALIZA LA INFORMACION DE PRECIPITACIONES
    [WebMethod(EnableSession = true)]
    public string save_precipitaciones(int id_precipitacion, float precipitacion)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se actualiza codigo y nombre de la finca
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.D_precipitaciones
                        SET valor = @precipitacion
                    WHERE id = @id_planta 

                    "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_precipitacion);
                adapter.SelectCommand.Parameters.AddWithValue("@precipitacion", precipitacion);
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
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, Fusarium={1}", id_precipitacion, precipitacion));
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
