using System;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Globalization;
using System.IO;

public class RecorridoFincas : WebService
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
	            id_edad,
	            edad,
	            id_indicador,
	            indicador
            FROM
            (
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

	            UNION

	            SELECT
		            E.id AS id_edad,
		            E.edad,
		            I.id AS id_indicador,
		            I.abreviatura AS indicador
	            FROM dbo.M_edades AS E
	            CROSS JOIN
	            (
		            SELECT
			            id,
			            abreviatura
		            FROM dbo.M_indicadores AS I
		            WHERE I.id IN (9, 10, 11, 12, 13, 14) --INDICADORES DE SIN EDAD
	            ) AS I
	            WHERE E.id = 7 --SIN EDAD
            ) AS DATA
            WHERE 1 = 1 /* CONDICION PARA HACER VALIDOS FILTROS ANIDADOS */
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
            where = @"AND id_edad IN (" + filtros["edad"]["data"].ToString() + @")
            ";
        }
        if (filtros["indicador"]["data"].ToString() != "0" && int.Parse(filtros["indicador"]["state"].ToString()) != 1)
        {
            where += @"AND id_indicador IN (" + filtros["indicador"]["data"].ToString() + @")
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

    //CONSULTA DATOS DE UNA VISITA PARA MOSTAR EN EL MAPA
    [WebMethod(EnableSession = true)]
    public string visita(int id_visita, string edad, string infeccion)
    {
        JObject resultado = new JObject();
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    latitud AS lat,
	                    longitud AS lng
                    FROM dbo.D_recorrido_visita
                    WHERE id_visita = @id_visita
                    ORDER BY
	                    fecha ASC

                    SELECT
	                    CONCAT(
		                    I.abreviatura, 
		                    ' (', E.abreviatura, ') ', 
			                    CASE
				                    WHEN IPC.id_indicador IN (48, 49, 50) THEN CAST(EE.valor_equivalente AS NVARCHAR(255))
				                    WHEN IPC.id_indicador IN (9, 10, 11) THEN CAST(IV.valor AS NVARCHAR(255))
				                    WHEN IPC.id_indicador IN (13, 14) THEN CAST(IB.valor AS NVARCHAR(255))
				                    ELSE CAST(IPC.valor AS NVARCHAR(255))
			                    END,
		                    ' / ', CAST(PPC.fecha AS TIME(0))
	                    ) AS nombre,
	                    PPC.latitud AS lat,
	                    PPC.longitud AS lng,
	                    SI.color,
                        ISNULL(EI.semaforo_activo, 0) AS semaforo_activo,
                        SI.radio_mapa
                    --Puntos captura sigatoka para llegar a los indicadores YLI YLS
                    FROM dbo.D_punto_captura AS PC
                    --Edades de las plantas
                    INNER JOIN dbo.D_planta_punto_captura AS PPC
                    ON PC.id = PPC.id_punto_captura
                    AND PPC.id_edad IN ({0}) --FILTRO DE EDADES SELECCIONADAS
                    INNER JOIN dbo.M_edades AS E
                    ON PPC.id_edad = E.id
                    --Indicadores YLI YLS
                    INNER JOIN dbo.D_indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta
                    AND IPC.id_indicador IN ({1}) --FILTRO DE INDICADOR SELECCIONADO
                    INNER JOIN dbo.M_indicadores AS I
                    ON IPC.id_indicador = I.id
                    --Consulta de atributo organica para la finca visitada
                    INNER JOIN dbo.D_visita AS V
                    ON PC.id_visita = V.id
                    INNER JOIN dbo.M_valor_atributo AS VA
                    ON V.id_recurso = VA.id_recurso
                    AND VA.id_atributo = 10 --ATRIBUTO ORGANICA
                    INNER JOIN dbo.M_valor_atributo_discreto AS VAD
                    ON VA.id_valor_discreto = VAD.id
                    --Color del punto de acuerdo al valor
                    LEFT JOIN dbo.M_edad_indicador AS EI
                    ON PPC.id_edad = EI.id_edad
                    AND IPC.id_indicador = EI.id_indicador
                    LEFT JOIN dbo.M_semaforo_indicador AS SI
                    ON IPC.valor > SI.[min]
                    AND IPC.valor <= SI.[max]
                    AND EI.id = SI.id_edad_indicador
                    AND VAD.valor = CASE WHEN SI.organica = 0 THEN 'No' ELSE 'Si' END
                    LEFT JOIN dbo.M_valor_equivalente_estadio AS EE
                    ON IPC.valor = EE.id
                    LEFT JOIN dbo.M_indicadores_vasculares AS IV
                    ON IPC.valor = IV.id
                    LEFT JOIN dbo.M_indicadores_bioseguridad AS IB
                    ON IPC.valor =IB.id
                    WHERE PC.id_visita = @id_visita
                    ORDER BY
	                    PPC.fecha ASC

                ", edad, infeccion), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_visita", id_visita);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable recorrido = dt.Tables[0];
                DataTable indicadores = dt.Tables[1];

                resultado["RECORRIDO"] = JArray.Parse(JsonConvert.SerializeObject(recorrido, Formatting.None));
                resultado["INDICADOR"] = JArray.Parse(JsonConvert.SerializeObject(indicadores, Formatting.None));

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = resultado;
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_visita={0}, edad={1}, infeccion={2}", id_visita, edad, infeccion));
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

    //CONSULTA DATOS DE UNA VISITA PARA MOSTAR EN TABLAS
    [WebMethod(EnableSession = true)]
    public string tabla_visita(int id_visita)
    {
        JObject resultado = new JObject();
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    PV.id,
	                    PV.fecha AS [Fecha y Hora],
	                    PV.edades AS Edad,
	                    PV.[TH],
	                    PV.[YLI],
	                    PV.[YLS],
	                    PV.[CF],
	                    PV.[HF],
	                    H2.valor_equivalente AS H2,
	                    H3.valor_equivalente AS H3,
	                    H4.valor_equivalente AS H4,
	                    PV.[EFA],
		                PV.[Lote]
                    FROM
                    (
	                    SELECT
		                    C.id,
		                    CAST(C.fecha AS DATETIME2(0)) AS fecha,--FECHA DE EDAD
		                    E.abreviatura AS edades,--ABREVIATURA EDADES
		                    NI.abreviatura AS indicadores, --ABREVIATURA INDICADORES
                            COALESCE(CAST(I.valor AS NVARCHAR(255)), I.valor_texto) AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS P
	                    ON V.id = P.id_visita
	                    --EDADES
	                    INNER JOIN dbo.D_planta_punto_captura AS C
	                    ON P.id = C.id_punto_captura
	                    --NOMBRE DE LAS EDADES O ABREVIACIONES
	                    INNER JOIN dbo.M_edades AS E
	                    ON C.id_edad = E.id
	                    --INDICADORES
	                    INNER JOIN dbo.D_indicadores_punto_captura AS I
	                    ON C.id = I.id_planta
	                    --NOMBRES DE LOS INDICADORES O ABREVIATURAS
	                    INNER JOIN dbo.M_indicadores AS NI
	                    ON I.id_indicador = NI.id
	                    --CONDICIONES
	                    WHERE V.id = @id_visita --VISITA
	                    AND P.id_tipo = 1 --SIGATOKA
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([TH], [YLI], [YLS], [CF], [HF], [H2], [H3], [H4], [EFA], [Lote])
                    ) AS PV
                    LEFT JOIN dbo.M_valor_equivalente_estadio AS H2
                    ON PV.[H2] = H2.id
                    LEFT JOIN dbo.M_valor_equivalente_estadio AS H3
                    ON PV.[H3] = H3.id
                    LEFT JOIN dbo.M_valor_equivalente_estadio AS H4
                    ON PV.[H4] = H4.id
                    ORDER BY PV.fecha

                    -----------------------------------------------------------------------

                    SELECT
	                    PV.id,
	                    PV.fecha AS [Fecha y Hora],
	                    F.valor AS Fusarium,
	                    M.valor AS Moko,
	                    E.valor AS Erwinia
                    FROM
                    (
	                    SELECT
		                    C.id,
		                    CAST(C.fecha AS DATETIME2(0)) AS fecha,--FECHA DE EDAD
		                    E.abreviatura AS edades,--ABREVIATURA EDADES
		                    NI.abreviatura AS indicadores, --ABREVIATURA INDICADORES
		                    I.valor AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS P
	                    ON V.id = P.id_visita
	                    --EDADES
	                    INNER JOIN dbo.D_planta_punto_captura AS C
	                    ON P.id = C.id_punto_captura
	                    --NOMBRE DE LAS EDADES O ABREVIACIONES
	                    INNER JOIN dbo.M_edades AS E
	                    ON C.id_edad = E.id
	                    --INDICADORES
	                    INNER JOIN dbo.D_indicadores_punto_captura AS I
	                    ON C.id = I.id_planta
	                    --NOMBRES DE LOS INDICADORES O ABREVIATURAS
	                    INNER JOIN dbo.M_indicadores AS NI
	                    ON I.id_indicador = NI.id
	                    --CONDICIONES
	                    WHERE V.id = @id_visita --VISITA
	                    AND P.id_tipo = 2 --ENFERMEDADES VASCULARES
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([Fusarium], [Moko], [Erwinia])
                    ) AS PV
                    LEFT JOIN dbo.M_indicadores_vasculares AS F
                    ON PV.Fusarium = F.id
                    LEFT JOIN dbo.M_indicadores_vasculares AS M
                    ON PV.Moko = M.id
                    LEFT JOIN dbo.M_indicadores_vasculares AS E
                    ON PV.Erwinia = E.id
                    ORDER BY PV.fecha

                    -----------------------------------------------------------------------

                    SELECT
	                    PV.id,
	                    PV.fecha AS [Fecha y Hora],
	                    PV.NF,
	                    FIT.valor AS FIT,
		                PV.[Comentario FIT],
	                    RTI.valor AS RTI,
		                PV.[Comentario RTI]
                    FROM
                    (
	                    SELECT
		                    C.id,
		                    CAST(C.fecha AS DATETIME2(0)) AS fecha,--FECHA DE EDAD
		                    E.abreviatura AS edades,--ABREVIATURA EDADES
		                    NI.abreviatura AS indicadores, --ABREVIATURA INDICADORES
                            COALESCE(CAST(I.valor AS NVARCHAR(255)), I.valor_texto) AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS P
	                    ON V.id = P.id_visita
	                    --EDADES
	                    INNER JOIN dbo.D_planta_punto_captura AS C
	                    ON P.id = C.id_punto_captura
	                    --NOMBRE DE LAS EDADES O ABREVIACIONES
	                    INNER JOIN dbo.M_edades AS E
	                    ON C.id_edad = E.id
	                    --INDICADORES
	                    INNER JOIN dbo.D_indicadores_punto_captura AS I
	                    ON C.id = I.id_planta
	                    --NOMBRES DE LOS INDICADORES O ABREVIATURAS
	                    INNER JOIN dbo.M_indicadores AS NI
	                    ON I.id_indicador = NI.id
	                    --CONDICIONES
	                    WHERE V.id = @id_visita --VISITA
	                    AND P.id_tipo = 3 --CONDICIONES CULTURALES
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([NF], [FIT], [Comentario FIT], [RTI], [Comentario RTI])
                    ) AS PV
                    INNER JOIN dbo.M_indicadores_bioseguridad AS FIT
                    ON PV.FIT = FIT.id
                    INNER JOIN dbo.M_indicadores_bioseguridad AS RTI
                    ON PV.RTI = RTI.id
                    ORDER BY PV.fecha

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_visita", id_visita);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable sigatoka = dt.Tables[0];
                DataTable vasculares = dt.Tables[1];
                DataTable culturales = dt.Tables[2];

                resultado["SIGATOKA"] = JArray.Parse(JsonConvert.SerializeObject(sigatoka, Formatting.None));
                resultado["VASCULARES"] = JArray.Parse(JsonConvert.SerializeObject(vasculares, Formatting.None));
                resultado["CULTURALES"] = JArray.Parse(JsonConvert.SerializeObject(culturales, Formatting.None));

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = resultado;
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

    //CONSULTA LAS FOTOS PARA UNA PLANTA E INDICADOR
    [WebMethod(EnableSession = true)]
    public string fotos(int id_planta, int id_indicador)
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
	                    F.url
                    FROM dbo.D_planta_punto_captura AS P
                    INNER JOIN dbo.M_edades AS E
                    ON P.id_edad = E.id
                    INNER JOIN dbo.D_indicadores_punto_captura AS I
                    ON P.id = I.id_planta
                    INNER JOIN dbo.D_fotos_indicador AS F
                    ON I.id = F.id_lectura_indicador
                    WHERE P.id = @id_planta --ID_PLANTA
                    AND I.id_indicador = @id_indicador --ID_INDICADOR

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_planta", id_planta);
                adapter.SelectCommand.Parameters.AddWithValue("@id_indicador", id_indicador);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable tabla = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(tabla, Formatting.None));
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_planta={0}, id_indicador={1}", id_planta, id_indicador));
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