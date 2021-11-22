using System;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Web;
using System.Globalization;

public class Nuevo : WebService
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

    //CONSULTA VISITAS A UNA FINCA EN UNA FECHA
    [WebMethod(EnableSession = true)]
    public string select_visita(string fecha, int id_finca)
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

                    SELECT
                        id,
                        name
                    FROM
                    (
                        SELECT
	                        id,
	                        CONCAT('Visita ', CAST(ROW_NUMBER() OVER(PARTITION BY id_recurso, CAST(fecha AS DATE) ORDER BY fecha) AS NVARCHAR(255))) AS name
                        FROM dbo.D_visita
                        WHERE id_recurso = @id_finca
                        AND CAST(fecha AS DATE) = @fecha

                        UNION

                        SELECT
                            0 AS id,
                            'Nuevo' AS name
                    ) AS s
                    ORDER BY
                        name

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

    //CONSULTA LOS DATOS DE UNA VISITA
    [WebMethod(EnableSession = true)]
    public string bring_visita(string fecha, int id_visita, int id_finca)
    {
        JObject result = new JObject();
        JObject consultas = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
    
                    --PRECIPITACION
                    SELECT P.valor
                    FROM dbo.D_precipitaciones AS P
                    WHERE P.id_recurso = @id_finca --FINCA
                    AND CAST(P.fecha AS DATE) = DATEADD(DD, -1, CAST(@fecha AS DATE)) --FECHA

                    --TEMPERATURAS Y HUMEDAD
                    SELECT
	                    V.temperatura_minima,
	                    V.temperatura,
	                    V.temperatura_maxima,
	                    V.humedad_relativa
                    FROM dbo.D_visita AS V
                    WHERE V.id = @id_visita

                    --PROXIMA A PARIR
                    SELECT
	                    PV.id AS ID,
	                    PV.[TH],
	                    PV.[YLI],
	                    PV.[YLS],
	                    NULLIF(PV.[CF], 0) AS [CF],
	                    PV.[Lote]
                    FROM
                    (
	                    SELECT
		                    PC.id,			
		                    V.id_recurso AS id_finca,
		                    CAST(PC.fecha AS DATE) AS fecha,
		                    CAST(PC.fecha AS TIME(0)) AS hora,
		                    I.abreviatura AS indicadores, --ABREVIATURA INDICADORES
		                    COALESCE(CAST(IPC.valor AS NVARCHAR(255)), IPC.valor_texto) AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SE DISCRIMINA POR SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS PC
	                    ON V.id = PC.id_visita
	                    AND PC.id_tipo = 1 --SIGATOKA
	                    --SE DISCRIMINA POR EDAD
	                    LEFT JOIN dbo.D_planta_punto_captura AS PPC
	                    ON PC.id = PPC.id_punto_captura
	                    AND PPC.id_edad = 3 --PROXIMA A PARIR
	                    --SE CONSULTAN LOS VALORES
	                    LEFT JOIN dbo.D_indicadores_punto_captura AS IPC
	                    ON PPC.id = IPC.id_planta
	                    --SE CONSULTAN LOS INDICADORES
	                    LEFT JOIN dbo.M_indicadores AS I
	                    ON IPC.id_indicador =I.id
	                    --ID_VISITA
	                    WHERE V.id = @id_visita
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([TH], [YLI], [YLS], [CF], [Lote])
                    ) AS PV
                    ORDER BY
                        PV.fecha,
                        PV.hora,
                        PV.id

                    --7 SEMANAS
                    SELECT
	                    PV.id AS ID,
	                    PV.[YLS],
	                    NULLIF(PV.[CF], 0) AS [CF],
	                    PV.[HF],
	                    PV.[Lote]
                    FROM
                    (
	                    SELECT
		                    PC.id,			
		                    V.id_recurso AS id_finca,
		                    CAST(PC.fecha AS DATE) AS fecha,
		                    CAST(PC.fecha AS TIME(0)) AS hora,
		                    I.abreviatura AS indicadores, --ABREVIATURA INDICADORES
		                    COALESCE(CAST(IPC.valor AS NVARCHAR(255)), IPC.valor_texto) AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SE DISCRIMINA POR SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS PC
	                    ON V.id = PC.id_visita
	                    AND PC.id_tipo = 1 --SIGATOKA
	                    --SE DISCRIMINA POR EDAD
	                    LEFT JOIN dbo.D_planta_punto_captura AS PPC
	                    ON PC.id = PPC.id_punto_captura
	                    AND PPC.id_edad = 2 --7 SEMANAS
	                    --SE CONSULTAN LOS VALORES
	                    LEFT JOIN dbo.D_indicadores_punto_captura AS IPC
	                    ON PPC.id = IPC.id_planta
	                    --SE CONSULTAN LOS INDICADORES
	                    LEFT JOIN dbo.M_indicadores AS I
	                    ON IPC.id_indicador =I.id
	                    --ID_VISITA
	                    WHERE V.id = @id_visita
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([YLS], [CF], [HF], [Lote])
                    ) AS PV
                    ORDER BY
		                PV.fecha,
		                PV.hora,
		                PV.id

                    --10 SEMANAS
                    SELECT
	                    PV.id AS ID,
	                    PV.[YLS],
	                    NULLIF(PV.[CF], 0) AS [CF],
	                    PV.[HF],
	                    PV.[Lote]
                    FROM
                    (
	                    SELECT
		                    PC.id,			
		                    V.id_recurso AS id_finca,
		                    CAST(PC.fecha AS DATE) AS fecha,
		                    CAST(PC.fecha AS TIME(0)) AS hora,
		                    I.abreviatura AS indicadores, --ABREVIATURA INDICADORES
		                    COALESCE(CAST(IPC.valor AS NVARCHAR(255)), IPC.valor_texto) AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SE DISCRIMINA POR SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS PC
	                    ON V.id = PC.id_visita
	                    AND PC.id_tipo = 1 --SIGATOKA
	                    --SE DISCRIMINA POR EDAD
	                    LEFT JOIN dbo.D_planta_punto_captura AS PPC
	                    ON PC.id = PPC.id_punto_captura
	                    AND PPC.id_edad = 1 --10 SEMANAS
	                    --SE CONSULTAN LOS VALORES
	                    LEFT JOIN dbo.D_indicadores_punto_captura AS IPC
	                    ON PPC.id = IPC.id_planta
	                    --SE CONSULTAN LOS INDICADORES
	                    LEFT JOIN dbo.M_indicadores AS I
	                    ON IPC.id_indicador =I.id
	                    --ID_VISITA
	                    WHERE V.id = @id_visita
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([YLS], [CF], [HF], [Lote])
                    ) AS PV
                    ORDER BY
		                PV.fecha,
		                PV.hora,
		                PV.id

                    --PARCELA FIJA
                    SELECT
	                    PV.id AS ID,
	                    PV.[TH],
	                    PV.[EFA],
	                    NULLIF(PV.[CF], 0) AS [CF],
	                    PV.[H2],
	                    PV.[H3],
	                    PV.[H4],
	                    PV.[Lote]
                    FROM
                    (
	                    SELECT
		                    PC.id,			
		                    V.id_recurso AS id_finca,
		                    CAST(PC.fecha AS DATE) AS fecha,
		                    CAST(PC.fecha AS TIME(0)) AS hora,
		                    I.abreviatura AS indicadores, --ABREVIATURA INDICADORES
		                    COALESCE(CAST(IPC.valor AS NVARCHAR(255)), IPC.valor_texto) AS valores --VALOR INDICADORES
	                    --VISITA
	                    FROM dbo.D_visita AS V
	                    --SE DISCRIMINA POR SIGATOKA
	                    INNER JOIN dbo.D_punto_captura AS PC
	                    ON V.id = PC.id_visita
	                    AND PC.id_tipo = 1 --SIGATOKA
	                    --SE DISCRIMINA POR EDAD
	                    LEFT JOIN dbo.D_planta_punto_captura AS PPC
	                    ON PC.id = PPC.id_punto_captura
	                    AND PPC.id_edad = 6 --PARCELA FIJA
	                    --SE CONSULTAN LOS VALORES
	                    LEFT JOIN dbo.D_indicadores_punto_captura AS IPC
	                    ON PPC.id = IPC.id_planta
	                    --SE CONSULTAN LOS INDICADORES
	                    LEFT JOIN dbo.M_indicadores AS I
	                    ON IPC.id_indicador =I.id
	                    --ID_VISITA
	                    WHERE V.id = @id_visita
                    ) AS S
                    PIVOT 
                    (
	                    MAX(S.valores) FOR S.indicadores
	                    IN ([TH], [EFA], [CF], [H2], [H3], [H4], [Lote])
                    ) AS PV
                    ORDER BY
	                    PV.fecha,
	                    PV.hora,
	                    PV.id

                ", id_visita, fecha), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_visita", id_visita);
                adapter.SelectCommand.Parameters.AddWithValue("@id_finca", id_finca);
                adapter.SelectCommand.Parameters.AddWithValue("@fecha", fecha);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable precipitacion = dt.Tables[0];
                DataTable temperaturas = dt.Tables[1];
                DataTable pp = dt.Tables[2];
                DataTable viisem = dt.Tables[3];
                DataTable xsem = dt.Tables[4];
                DataTable pf = dt.Tables[5];

                consultas["PRECIPITACION"] = JArray.Parse(JsonConvert.SerializeObject(precipitacion, Formatting.None));
                consultas["TEMPERATURAS"] = JArray.Parse(JsonConvert.SerializeObject(temperaturas, Formatting.None));
                consultas["PP"] = JArray.Parse(JsonConvert.SerializeObject(pp, Formatting.None));
                consultas["VIISEM"] = JArray.Parse(JsonConvert.SerializeObject(viisem, Formatting.None));
                consultas["XSEM"] = JArray.Parse(JsonConvert.SerializeObject(xsem, Formatting.None));
                consultas["PF"] = JArray.Parse(JsonConvert.SerializeObject(pf, Formatting.None));

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = consultas;
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"fecha={0}, id_visita={1}, id_finca={2}", fecha, id_visita, id_finca));
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

    //INSERTA NUEVA VISITA
    [WebMethod(EnableSession = true)]
    public string insert_visita(string json)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                JObject dataForm = JObject.Parse(json);

                //Se inserta una visita si es nueva
                if (dataForm["id_visita"].ToString() == "0")
                {
                    //Actualización de precipitaciones y creación de visita
                    adapter = new SqlDataAdapter(String.Format(@"

                        DECLARE @id_finca INT = {0}
                        DECLARE @fecha DATE = '{1}'
                        DECLARE @id_usuario INT = {2}
                        DECLARE @precipitacion FLOAT(53) = {3}

                        DECLARE @humedad FLOAT(53) = {4}
                        DECLARE @temp_min FLOAT(53) = {5}
                        DECLARE @temp FLOAT(53) = {6}
                        DECLARE @temp_max FLOAT(53) = {7}

                        UPDATE dbo.D_precipitaciones
                        SET
	                        id_usuario = @id_usuario,
	                        valor = @precipitacion
                        WHERE id_recurso = @id_finca
                        AND CAST(fecha AS DATE) = DATEADD(dd, -1, @fecha)
                        AND valor <> @precipitacion --ACTUALIZA SI SE MODFICO LA PRECIPITACION

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
		                        DATEADD(dd, -1, @fecha) AS fecha,
		                        @precipitacion AS valor
                        ) AS S
                        LEFT JOIN dbo.D_precipitaciones AS P
                        ON S.id_recurso = P.id_recurso
                        AND S.fecha = CAST(P.fecha AS DATE)
                        WHERE P.id IS NULL;

                        INSERT INTO dbo.D_visita
                        (id_recurso, id_usuario, id_tipo_visita, temperatura_minima, temperatura, temperatura_maxima, humedad_relativa, fecha)
                        VALUES
                        (@id_finca, @id_usuario, 1, @temp_min, @temp, @temp_max, @humedad, @fecha)

                        SELECT @@IDENTITY AS id_visita

                    ", 
                    dataForm["id_finca"].ToString(),
                    dataForm["fecha"].ToString(),
                    dataForm["id_user"].ToString(),
                    dataForm["precipitacion"].ToString() == "" ? "NULL" : dataForm["precipitacion"].ToString(),
                    dataForm["humedad"].ToString() == "" ? "NULL" : dataForm["humedad"].ToString(), 
                    dataForm["temperatura_minima"].ToString() == "" ? "NULL" : dataForm["temperatura_minima"].ToString(),
                    dataForm["temperatura"].ToString() == "" ? "NULL" : dataForm["temperatura"].ToString(),
                    dataForm["temperatura_maxima"].ToString() == "" ? "NULL" : dataForm["temperatura_maxima"].ToString()
                    ), conexion.getConexion());
                    DataSet dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable table = dt.Tables[0];

                    dataForm["id_visita"] = table.Rows[0]["id_visita"].ToString();

                    //Conversion de la informacion recibida a datatable
                    DataTable pp = (DataTable)JsonConvert.DeserializeObject(dataForm["pp"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar proxima a parir
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #pp
                        (
	                        ID INT,
                            ORDEN INT,
	                        TH FLOAT(53),
	                        YLI FLOAT(53),
                            YLS FLOAT(53),
                            CF INT,
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in pp.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#pp";
                        bulkCopy.WriteToServer(pp);
                    }

                    //Conversion de la informacion recibida a datatable
                    DataTable viisem = (DataTable)JsonConvert.DeserializeObject(dataForm["viisem"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar siete semanas
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #viisem
                        (
	                        ID INT,
                            ORDEN INT,
	                        YLS FLOAT(53),
                            CF INT,
	                        HF FLOAT(53),
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in viisem.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#viisem";
                        bulkCopy.WriteToServer(viisem);
                    }

                    //Conversion de la informacion recibida a datatable
                    DataTable xsem = (DataTable)JsonConvert.DeserializeObject(dataForm["xsem"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar diex semanas
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #xsem
                        (
	                        ID INT,
                            ORDEN INT,
	                        YLS FLOAT(53),
                            CF INT,
	                        HF FLOAT(53),
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in xsem.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#xsem";
                        bulkCopy.WriteToServer(xsem);
                    }

                    //Conversion de la informacion recibida a datatable
                    DataTable pfsem = (DataTable)JsonConvert.DeserializeObject(dataForm["pfsem"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar parcela fija
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #pfsem
                        (
	                        ID INT,
                            ORDEN INT,
	                        TH FLOAT(53),
                            EFA FLOAT(53),
                            CF INT,
	                        H2 INT,
                            H3 INT,
                            H4 INT,
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in pfsem.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#pfsem";
                        bulkCopy.WriteToServer(pfsem);
                    }

                    //SE PASAN LOS D_PUNTO_CAPTURA
                    adapter = new SqlDataAdapter(String.Format(@"
                        --SIEMPRE SE CREAN QUINCE REGISTROS
                        INSERT INTO dbo.D_punto_captura
                        (id_visita, id_tipo, fecha)
                        SELECT
	                        {0} AS id_visita,
	                        1 AS id_tipo,
	                        '{1}' AS fecha
                        FROM 
                        (
	                        SELECT ORDEN
	                        FROM #pp

	                        UNION

	                        SELECT ORDEN
	                        FROM #viisem

	                        UNION

	                        SELECT ORDEN
	                        FROM #xsem

	                        UNION

	                        SELECT ORDEN
	                        FROM #pfsem
                        ) AS S
                        ORDER BY
	                        S.ORDEN

                    ", dataForm["id_visita"].ToString(), dataForm["fecha"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //SE ACTUALIZAN ID EN TABLAS TEMPORALES
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #pp AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.TH IS NOT NULL
                        OR aux.YLI IS NOT NULL
                        OR aux.YLS IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.Lote IS NOT NULL

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #viisem AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.YLS IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.HF IS NOT NULL
                        OR aux.Lote IS NOT NULL

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #xsem AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.YLS IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.HF IS NOT NULL
                        OR aux.Lote IS NOT NULL

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #pfsem AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.TH IS NOT NULL
                        OR aux.EFA IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.H2 IS NOT NULL
                        OR aux.H3 IS NOT NULL
                        OR aux.H4 IS NOT NULL
                        OR aux.Lote IS NOT NULL

                    ", dataForm["id_visita"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //SE ACTUALIZA PLANTAS PUNTO CAPTURA
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.D_planta_punto_captura
                        (id_punto_captura, id_edad, fecha)
                        SELECT
	                        S.ID AS id_punto_captura,
	                        S.EDAD AS id_edad,
	                        '{0}' AS fecha
                        FROM
                        (	
	                        SELECT
		                        ID,
		                        3 AS EDAD,
		                        ORDEN
	                        FROM #pp
	                        WHERE TH IS NOT NULL
	                        OR YLI IS NOT NULL
	                        OR YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        2 AS EDAD,
		                        ORDEN
	                        FROM #viisem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        1 AS EDAD,
		                        ORDEN
	                        FROM #xsem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        6 AS EDAD,
		                        ORDEN
	                        FROM #pfsem
	                        WHERE TH IS NOT NULL
	                        OR EFA IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR H2 IS NOT NULL
	                        OR H3 IS NOT NULL
	                        OR H4 IS NOT NULL
	                        OR Lote IS NOT NULL
                        ) AS S
                        ORDER BY
	                        S.ORDEN,
	                        S.EDAD

                    ", dataForm["fecha"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //SE ACTUALIZA PLANTAS PUNTO CAPTURA
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.D_indicadores_punto_captura
                        (id_planta, id_indicador, valor, valor_texto)
                        SELECT
	                        PPC.id AS id_planta,
	                        CASE
		                        WHEN UNPVT.indicador = 'TH' THEN 4
		                        WHEN UNPVT.indicador = 'YLI' THEN 5
		                        WHEN UNPVT.indicador = 'YLS' THEN 2
		                        WHEN UNPVT.indicador = 'CF' THEN 7
		                        WHEN UNPVT.indicador = 'HF' THEN 1
		                        WHEN UNPVT.indicador = 'H2' THEN 48
		                        WHEN UNPVT.indicador = 'H3' THEN 49
		                        WHEN UNPVT.indicador = 'H4' THEN 50
		                        WHEN UNPVT.indicador = 'EFA' THEN 8
		                        WHEN UNPVT.indicador = 'Lote' THEN 51
		                        ELSE 9999999999999999999999
	                        END AS id_indicador,
	                        CASE
		                        WHEN UNPVT.indicador = 'Lote' THEN NULL
		                        ELSE UNPVT.valor
	                        END AS valor,
	                        CASE
		                        WHEN UNPVT.indicador = 'Lote' THEN UNPVT.valor
		                        ELSE NULL
	                        END AS valor_texto
                        FROM
                        (	
	                        SELECT
		                        ID,
		                        3 AS EDAD,
		                        CAST(TH AS NVARCHAR(255)) AS TH,
		                        CAST(YLI AS NVARCHAR(255)) AS YLI,
		                        CAST(YLS AS NVARCHAR(255)) AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        NULL AS HF,
		                        NULL AS EFA,
		                        NULL AS H2,
		                        NULL AS H3,
		                        NULL AS H4
	                        FROM #pp
	                        WHERE TH IS NOT NULL
	                        OR YLI IS NOT NULL
	                        OR YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION
		
	                        SELECT
		                        ID,
		                        2 AS EDAD,
		                        NULL AS TH,
		                        NULL AS YLI,
		                        CAST(YLS AS NVARCHAR(255)) AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        CAST(HF AS NVARCHAR(255)) AS HF,
		                        NULL AS EFA,
		                        NULL AS H2,
		                        NULL AS H3,
		                        NULL AS H4
	                        FROM #viisem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        1 AS EDAD,
		                        NULL AS TH,
		                        NULL AS YLI,
		                        CAST(YLS AS NVARCHAR(255)) AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        CAST(HF AS NVARCHAR(255)) AS HF,
		                        NULL AS EFA,
		                        NULL AS H2,
		                        NULL AS H3,
		                        NULL AS H4
	                        FROM #xsem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION
 
	                        SELECT
		                        ID,
		                        6 AS EDAD,
		                        CAST(TH AS NVARCHAR(255)) AS TH,
		                        NULL AS YLI,
		                        NULL AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        NULL AS HF,
		                        CAST(EFA AS NVARCHAR(255)) AS EFA,
		                        CAST(H2 AS NVARCHAR(255)) AS H2,
		                        CAST(H3 AS NVARCHAR(255)) AS H3,
		                        CAST(H4 AS NVARCHAR(255)) AS H4
	                        FROM #pfsem
	                        WHERE TH IS NOT NULL
	                        OR EFA IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR H2 IS NOT NULL
	                        OR H3 IS NOT NULL
	                        OR H4 IS NOT NULL
	                        OR Lote IS NOT NULL
                        ) AS S
                        UNPIVOT
                        (
	                        valor
	                        FOR indicador IN (TH, YLI, YLS, CF, Lote, HF, EFA, H2, H3, H4)
                        ) AS UNPVT
                        INNER JOIN dbo.D_planta_punto_captura AS PPC
                        ON UNPVT.ID = PPC.id_punto_captura
                        AND UNPVT.EDAD = PPC.id_edad

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();
                }
                //Se hace insert update para visitas existentes
                else
                {
                    //Actualización de precipitaciones y creación de visita
                    adapter = new SqlDataAdapter(String.Format(@"

                        DECLARE @id_finca INT = {0}
                        DECLARE @fecha DATE = '{1}'
                        DECLARE @id_usuario INT = {2}
                        DECLARE @precipitacion FLOAT(53) = {3}

                        DECLARE @humedad FLOAT(53) = {4}
                        DECLARE @temp_min FLOAT(53) = {5}
                        DECLARE @temp FLOAT(53) = {6}
                        DECLARE @temp_max FLOAT(53) = {7}

                        DECLARE @id_visita INT = {8}

                        UPDATE dbo.D_precipitaciones
                        SET
	                        id_usuario = @id_usuario,
	                        valor = @precipitacion
                        WHERE id_recurso = @id_finca
                        AND CAST(fecha AS DATE) = DATEADD(dd, -1, @fecha)
                        AND valor <> @precipitacion --ACTUALIZA SI SE MODFICO LA PRECIPITACION

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
		                        DATEADD(dd, -1, @fecha) AS fecha,
		                        @precipitacion AS valor
                        ) AS S
                        LEFT JOIN dbo.D_precipitaciones AS P
                        ON S.id_recurso = P.id_recurso
                        AND S.fecha = CAST(P.fecha AS DATE)
                        WHERE P.id IS NULL;

                        UPDATE dbo.D_visita
                        SET
                            temperatura_minima = @temp_min,
                            temperatura = @temp,
                            temperatura_maxima = @temp_max,
                            humedad_relativa = @humedad
                        WHERE id = @id_visita

                    ",
                    dataForm["id_finca"].ToString(),
                    dataForm["fecha"].ToString(),
                    dataForm["id_user"].ToString(),
                    dataForm["precipitacion"].ToString() == "" ? "NULL" : dataForm["precipitacion"].ToString(),
                    dataForm["humedad"].ToString() == "" ? "NULL" : dataForm["humedad"].ToString(),
                    dataForm["temperatura_minima"].ToString() == "" ? "NULL" : dataForm["temperatura_minima"].ToString(),
                    dataForm["temperatura"].ToString() == "" ? "NULL" : dataForm["temperatura"].ToString(),
                    dataForm["temperatura_maxima"].ToString() == "" ? "NULL" : dataForm["temperatura_maxima"].ToString(),
                    dataForm["id_visita"].ToString()
                    ), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Conversion de la informacion recibida a datatable
                    DataTable pp = (DataTable)JsonConvert.DeserializeObject(dataForm["pp"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar proxima a parir
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #pp
                        (
	                        ID INT,
                            ORDEN INT,
	                        TH FLOAT(53),
	                        YLI FLOAT(53),
                            YLS FLOAT(53),
                            CF INT,
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in pp.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#pp";
                        bulkCopy.WriteToServer(pp);
                    }

                    //Conversion de la informacion recibida a datatable
                    DataTable viisem = (DataTable)JsonConvert.DeserializeObject(dataForm["viisem"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar siete semanas
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #viisem
                        (
	                        ID INT,
                            ORDEN INT,
	                        YLS FLOAT(53),
                            CF INT,
	                        HF FLOAT(53),
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in viisem.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#viisem";
                        bulkCopy.WriteToServer(viisem);
                    }

                    //Conversion de la informacion recibida a datatable
                    DataTable xsem = (DataTable)JsonConvert.DeserializeObject(dataForm["xsem"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar diex semanas
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #xsem
                        (
	                        ID INT,
                            ORDEN INT,
	                        YLS FLOAT(53),
                            CF INT,
	                        HF FLOAT(53),
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in xsem.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#xsem";
                        bulkCopy.WriteToServer(xsem);
                    }

                    //Conversion de la informacion recibida a datatable
                    DataTable pfsem = (DataTable)JsonConvert.DeserializeObject(dataForm["pfsem"].ToString(), (typeof(DataTable)));

                    //Tabla temporal para guardar parcela fija
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #pfsem
                        (
	                        ID INT,
                            ORDEN INT,
	                        TH FLOAT(53),
                            EFA FLOAT(53),
                            CF INT,
	                        H2 INT,
                            H3 INT,
                            H4 INT,
                            Lote NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in pfsem.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.DestinationTableName = "#pfsem";
                        bulkCopy.WriteToServer(pfsem);
                    }

                    //SE PASAN LOS D_PUNTO_CAPTURA
                    adapter = new SqlDataAdapter(String.Format(@"
                        --SIEMPRE SE CREAN QUINCE REGISTROS
                        INSERT INTO dbo.D_punto_captura
                        (id_visita, id_tipo, fecha)
                        SELECT
	                        {0} AS id_visita,
	                        1 AS id_tipo,
	                        '{1}' AS fecha
                        FROM 
                        (
	                        SELECT ORDEN
	                        FROM #pp

	                        UNION

	                        SELECT ORDEN
	                        FROM #viisem

	                        UNION

	                        SELECT ORDEN
	                        FROM #xsem

	                        UNION

	                        SELECT ORDEN
	                        FROM #pfsem
                        ) AS S
                        LEFT JOIN
                        (
                            SELECT
	                            id,
	                            ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
                            FROM dbo.D_punto_captura
                            WHERE id_visita = {0}
                        ) AS PC
                        ON S.ORDEN = PC.ORDEN
                        WHERE
                            PC.id IS NULL
                        ORDER BY
	                        S.ORDEN

                    ", dataForm["id_visita"].ToString(), dataForm["fecha"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //SE ACTUALIZAN ID EN TABLAS TEMPORALES
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #pp AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.ID IS NULL --SOLO REGISTROS NUEVOS
                        AND (aux.TH IS NOT NULL
                        OR aux.YLI IS NOT NULL
                        OR aux.YLS IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.Lote IS NOT NULL)

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #viisem AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.ID IS NULL --SOLO REGISTROS NUEVOS
                        AND (aux.YLS IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.HF IS NOT NULL
                        OR aux.Lote IS NOT NULL)

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #xsem AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.ID IS NULL --SOLO REGISTROS NUEVOS
                        AND (aux.YLS IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.HF IS NOT NULL
                        OR aux.Lote IS NOT NULL)

                        UPDATE aux
                        SET aux.ID = data.id
                        FROM #pfsem AS aux
                        INNER JOIN
                        (
	                        SELECT
		                        id,
		                        ROW_NUMBER() OVER(ORDER BY id) AS ORDEN
	                        FROM dbo.D_punto_captura
	                        WHERE id_visita = {0}
                        ) AS data
                        ON aux.ORDEN = data.ORDEN
                        WHERE aux.ID IS NULL --SOLO REGISTROS NUEVOS
                        AND (aux.TH IS NOT NULL
                        OR aux.EFA IS NOT NULL
                        OR aux.CF IS NOT NULL
                        OR aux.H2 IS NOT NULL
                        OR aux.H3 IS NOT NULL
                        OR aux.H4 IS NOT NULL
                        OR aux.Lote IS NOT NULL)

                    ", dataForm["id_visita"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //SE ACTUALIZA PLANTAS PUNTO CAPTURA
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.D_planta_punto_captura
                        (id_punto_captura, id_edad, fecha)
                        SELECT
	                        S.ID AS id_punto_captura,
	                        S.EDAD AS id_edad,
	                        '{0}' AS fecha
                        FROM
                        (	
	                        SELECT
		                        ID,
		                        3 AS EDAD,
		                        ORDEN
	                        FROM #pp
	                        WHERE TH IS NOT NULL
	                        OR YLI IS NOT NULL
	                        OR YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        2 AS EDAD,
		                        ORDEN
	                        FROM #viisem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        1 AS EDAD,
		                        ORDEN
	                        FROM #xsem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        6 AS EDAD,
		                        ORDEN
	                        FROM #pfsem
	                        WHERE TH IS NOT NULL
	                        OR EFA IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR H2 IS NOT NULL
	                        OR H3 IS NOT NULL
	                        OR H4 IS NOT NULL
	                        OR Lote IS NOT NULL
                        ) AS S
                        LEFT JOIN dbo.D_planta_punto_captura AS PPC
                        ON S.ID = PPC.id_punto_captura
                        AND S.EDAD = PPC.id_edad
                        WHERE PPC.id IS NULL --PLANTAS PUNTO CAPTURA NO REGISTRADAS ANTES
                        ORDER BY
	                        S.ORDEN,
	                        S.EDAD

                    ", dataForm["fecha"].ToString()), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //SE ACTUALIZA INDICADORES PUNTO CAPTURA
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE IPC
                        SET
	                        IPC.valor = DATA.valor,
	                        IPC.valor_texto = DATA.valor_texto
                        FROM dbo.D_indicadores_punto_captura AS IPC
                        INNER JOIN
                        (
	                        SELECT
		                        PPC.id AS id_planta,
		                        CASE
			                        WHEN UNPVT.indicador = 'TH' THEN 4
			                        WHEN UNPVT.indicador = 'YLI' THEN 5
			                        WHEN UNPVT.indicador = 'YLS' THEN 2
			                        WHEN UNPVT.indicador = 'CF' THEN 7
			                        WHEN UNPVT.indicador = 'HF' THEN 1
			                        WHEN UNPVT.indicador = 'H2' THEN 48
			                        WHEN UNPVT.indicador = 'H3' THEN 49
			                        WHEN UNPVT.indicador = 'H4' THEN 50
			                        WHEN UNPVT.indicador = 'EFA' THEN 8
			                        WHEN UNPVT.indicador = 'Lote' THEN 51
			                        ELSE 9999999999999999999999
		                        END AS id_indicador,
		                        CASE
			                        WHEN UNPVT.indicador = 'Lote' THEN NULL
			                        ELSE UNPVT.valor
		                        END AS valor,
		                        CASE
			                        WHEN UNPVT.indicador = 'Lote' THEN UNPVT.valor
			                        ELSE NULL
		                        END AS valor_texto
	                        FROM
	                        (	
		                        SELECT
			                        ID,
			                        3 AS EDAD,
			                        CAST(TH AS NVARCHAR(255)) AS TH,
			                        CAST(YLI AS NVARCHAR(255)) AS YLI,
			                        CAST(YLS AS NVARCHAR(255)) AS YLS,
			                        CAST(CF AS NVARCHAR(255)) AS CF,
			                        CAST(Lote AS NVARCHAR(255)) AS Lote,
			                        NULL AS HF,
			                        NULL AS EFA,
			                        NULL AS H2,
			                        NULL AS H3,
			                        NULL AS H4
		                        FROM #pp
		                        WHERE TH IS NOT NULL
		                        OR YLI IS NOT NULL
		                        OR YLS IS NOT NULL
		                        OR CF IS NOT NULL
		                        OR Lote IS NOT NULL

		                        UNION

		                        SELECT
			                        ID,
			                        2 AS EDAD,
			                        NULL AS TH,
			                        NULL AS YLI,
			                        CAST(YLS AS NVARCHAR(255)) AS YLS,
			                        CAST(CF AS NVARCHAR(255)) AS CF,
			                        CAST(Lote AS NVARCHAR(255)) AS Lote,
			                        CAST(HF AS NVARCHAR(255)) AS HF,
			                        NULL AS EFA,
			                        NULL AS H2,
			                        NULL AS H3,
			                        NULL AS H4
		                        FROM #viisem
		                        WHERE YLS IS NOT NULL
		                        OR CF IS NOT NULL
		                        OR HF IS NOT NULL
		                        OR Lote IS NOT NULL

		                        UNION

		                        SELECT
			                        ID,
			                        1 AS EDAD,
			                        NULL AS TH,
			                        NULL AS YLI,
			                        CAST(YLS AS NVARCHAR(255)) AS YLS,
			                        CAST(CF AS NVARCHAR(255)) AS CF,
			                        CAST(Lote AS NVARCHAR(255)) AS Lote,
			                        CAST(HF AS NVARCHAR(255)) AS HF,
			                        NULL AS EFA,
			                        NULL AS H2,
			                        NULL AS H3,
			                        NULL AS H4
		                        FROM #xsem
		                        WHERE YLS IS NOT NULL
		                        OR CF IS NOT NULL
		                        OR HF IS NOT NULL
		                        OR Lote IS NOT NULL

		                        UNION

		                        SELECT
			                        ID,
			                        6 AS EDAD,
			                        CAST(TH AS NVARCHAR(255)) AS TH,
			                        NULL AS YLI,
			                        NULL AS YLS,
			                        CAST(CF AS NVARCHAR(255)) AS CF,
			                        CAST(Lote AS NVARCHAR(255)) AS Lote,
			                        NULL AS HF,
			                        CAST(EFA AS NVARCHAR(255)) AS EFA,
			                        CAST(H2 AS NVARCHAR(255)) AS H2,
			                        CAST(H3 AS NVARCHAR(255)) AS H3,
			                        CAST(H4 AS NVARCHAR(255)) AS H4
		                        FROM #pfsem
		                        WHERE TH IS NOT NULL
		                        OR EFA IS NOT NULL
		                        OR CF IS NOT NULL
		                        OR H2 IS NOT NULL
		                        OR H3 IS NOT NULL
		                        OR H4 IS NOT NULL
		                        OR Lote IS NOT NULL
	                        ) AS S
	                        UNPIVOT
	                        (
		                        valor
		                        FOR indicador IN (TH, YLI, YLS, CF, Lote, HF, EFA, H2, H3, H4)
	                        ) AS UNPVT
	                        INNER JOIN dbo.D_planta_punto_captura AS PPC
	                        ON UNPVT.ID = PPC.id_punto_captura
	                        AND UNPVT.EDAD = PPC.id_edad
                        ) AS DATA
                        ON IPC.id_planta = DATA.id_planta
                        AND IPC.id_indicador = DATA.id_indicador

                        INSERT INTO dbo.D_indicadores_punto_captura
                        (id_planta, id_indicador, valor, valor_texto)
                        SELECT
	                        PPC.id AS id_planta,
	                        CASE
		                        WHEN UNPVT.indicador = 'TH' THEN 4
		                        WHEN UNPVT.indicador = 'YLI' THEN 5
		                        WHEN UNPVT.indicador = 'YLS' THEN 2
		                        WHEN UNPVT.indicador = 'CF' THEN 7
		                        WHEN UNPVT.indicador = 'HF' THEN 1
		                        WHEN UNPVT.indicador = 'H2' THEN 48
		                        WHEN UNPVT.indicador = 'H3' THEN 49
		                        WHEN UNPVT.indicador = 'H4' THEN 50
		                        WHEN UNPVT.indicador = 'EFA' THEN 8
		                        WHEN UNPVT.indicador = 'Lote' THEN 51
		                        ELSE 9999999999999999999999
	                        END AS id_indicador,
	                        CASE
		                        WHEN UNPVT.indicador = 'Lote' THEN NULL
		                        ELSE UNPVT.valor
	                        END AS valor,
	                        CASE
		                        WHEN UNPVT.indicador = 'Lote' THEN UNPVT.valor
		                        ELSE NULL
	                        END AS valor_texto
                        FROM
                        (	
	                        SELECT
		                        ID,
		                        3 AS EDAD,
		                        CAST(TH AS NVARCHAR(255)) AS TH,
		                        CAST(YLI AS NVARCHAR(255)) AS YLI,
		                        CAST(YLS AS NVARCHAR(255)) AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        NULL AS HF,
		                        NULL AS EFA,
		                        NULL AS H2,
		                        NULL AS H3,
		                        NULL AS H4
	                        FROM #pp
	                        WHERE TH IS NOT NULL
	                        OR YLI IS NOT NULL
	                        OR YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION
		
	                        SELECT
		                        ID,
		                        2 AS EDAD,
		                        NULL AS TH,
		                        NULL AS YLI,
		                        CAST(YLS AS NVARCHAR(255)) AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        CAST(HF AS NVARCHAR(255)) AS HF,
		                        NULL AS EFA,
		                        NULL AS H2,
		                        NULL AS H3,
		                        NULL AS H4
	                        FROM #viisem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION

	                        SELECT
		                        ID,
		                        1 AS EDAD,
		                        NULL AS TH,
		                        NULL AS YLI,
		                        CAST(YLS AS NVARCHAR(255)) AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        CAST(HF AS NVARCHAR(255)) AS HF,
		                        NULL AS EFA,
		                        NULL AS H2,
		                        NULL AS H3,
		                        NULL AS H4
	                        FROM #xsem
	                        WHERE YLS IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR HF IS NOT NULL
	                        OR Lote IS NOT NULL

	                        UNION
 
	                        SELECT
		                        ID,
		                        6 AS EDAD,
		                        CAST(TH AS NVARCHAR(255)) AS TH,
		                        NULL AS YLI,
		                        NULL AS YLS,
		                        CAST(CF AS NVARCHAR(255)) AS CF,
		                        CAST(Lote AS NVARCHAR(255)) AS Lote,
		                        NULL AS HF,
		                        CAST(EFA AS NVARCHAR(255)) AS EFA,
		                        CAST(H2 AS NVARCHAR(255)) AS H2,
		                        CAST(H3 AS NVARCHAR(255)) AS H3,
		                        CAST(H4 AS NVARCHAR(255)) AS H4
	                        FROM #pfsem
	                        WHERE TH IS NOT NULL
	                        OR EFA IS NOT NULL
	                        OR CF IS NOT NULL
	                        OR H2 IS NOT NULL
	                        OR H3 IS NOT NULL
	                        OR H4 IS NOT NULL
	                        OR Lote IS NOT NULL
                        ) AS S
                        UNPIVOT
                        (
	                        valor
	                        FOR indicador IN (TH, YLI, YLS, CF, Lote, HF, EFA, H2, H3, H4)
                        ) AS UNPVT
                        INNER JOIN dbo.D_planta_punto_captura AS PPC
                        ON UNPVT.ID = PPC.id_punto_captura
                        AND UNPVT.EDAD = PPC.id_edad
                        LEFT JOIN dbo.D_indicadores_punto_captura AS IPC
                        ON PPC.id = IPC.id_planta
                        AND CASE
                            WHEN UNPVT.indicador = 'TH' THEN 4
                            WHEN UNPVT.indicador = 'YLI' THEN 5
                            WHEN UNPVT.indicador = 'YLS' THEN 2
                            WHEN UNPVT.indicador = 'CF' THEN 7
                            WHEN UNPVT.indicador = 'HF' THEN 1
                            WHEN UNPVT.indicador = 'H2' THEN 48
                            WHEN UNPVT.indicador = 'H3' THEN 49
                            WHEN UNPVT.indicador = 'H4' THEN 50
                            WHEN UNPVT.indicador = 'EFA' THEN 8
                            WHEN UNPVT.indicador = 'Lote' THEN 51
                            ELSE 9999999999999999999999
                        END = IPC.id_indicador
                        WHERE IPC.id IS NULL --INDICADORES PUNTO CAPTURA NO REGISTRADAS ANTES

                    "), conexion.getConexion());
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
                Mail.SendEmail(e, host, string.Format(@"json={0}", json));
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
