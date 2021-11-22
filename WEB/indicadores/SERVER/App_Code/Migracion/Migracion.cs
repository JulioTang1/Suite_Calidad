using MySql.Data.MySqlClient;
using Newtonsoft.Json.Linq;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;

public class Migracion : WebService
{/*
    [WebMethod]
    public string Execute()
    {
        JObject result = new JObject(); //Aca se guarda el JObject consultas
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        //Conexion de MYSQL
        ConexionMYSQL conexionMYSQL = new ConexionMYSQL();
        MySqlCommand adapterMYSQL = new MySqlCommand();
        MySqlDataReader reader;
        DataTable tableMYSQL = new DataTable();

        if ((conexion.openConexion()) == "TRUE" && (conexionMYSQL.openConexion()) == "TRUE")
        {
            try
            {
                //SE CONSULTAN LAS FINCAS
                adapterMYSQL = new MySqlCommand(String.Format(@"

                    SELECT
                        G.id_grupo,
                        G.nombre,
                        G.hectareas,
                        G.activo,
                        NULLIF(GF.nombre, '') AS grupo
                    FROM banasan.grupos AS G
                    LEFT JOIN
                    (
                        SELECT
                            id_grupo_fincas,
                            id_finca
                        FROM banasan.grupos_fincas_detalles
                        WHERE id_grupo_fincas <> 1
                    ) AS GFD
                    ON G.id_grupo = GFD.id_finca
                    LEFT JOIN banasan.grupos_fincas AS GF
                    ON GFD.id_grupo_fincas = GF.id_grupo_fincas

                "), conexionMYSQL.getConexion());
                adapterMYSQL.CommandTimeout = 600;
                reader = adapterMYSQL.ExecuteReader();
                tableMYSQL = new DataTable();
                if (reader.HasRows)
                {
                    tableMYSQL.Load(reader);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

                using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                {
                    foreach (DataColumn col in tableMYSQL.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.fincas_mysql";
                    bulkCopy.WriteToServer(tableMYSQL);
                }

                //SE CONSULTAN LOS USUARIOS
                adapterMYSQL = new MySqlCommand(String.Format(@"

                    SELECT
	                    id_usuario AS id,
	                    nombre1,
	                    nombre2,
	                    apellido1,
                        apellido2,
                        activo,
                        eliminado
                    FROM banasan.usuarios

                "), conexionMYSQL.getConexion());
                adapterMYSQL.CommandTimeout = 600;
                reader = adapterMYSQL.ExecuteReader();
                tableMYSQL = new DataTable();
                if (reader.HasRows)
                {
                    tableMYSQL.Load(reader);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

                using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                {
                    foreach (DataColumn col in tableMYSQL.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.usuario_mysql";
                    bulkCopy.WriteToServer(tableMYSQL);
                }

                //SE CONSULTAN LOS INSPECTORES
                adapterMYSQL = new MySqlCommand(String.Format(@"

                    SELECT
                        id_usuario AS id,
                        nombre1,
                        nombre2,
                        apellido1,
                        apellido2,
                        activo,
                        eliminado
                    FROM banasan.inspectores

                "), conexionMYSQL.getConexion());
                adapterMYSQL.CommandTimeout = 600;
                reader = adapterMYSQL.ExecuteReader();
                tableMYSQL = new DataTable();
                if (reader.HasRows)
                {
                    tableMYSQL.Load(reader);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

                using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                {
                    foreach (DataColumn col in tableMYSQL.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.inspector_mysql";
                    bulkCopy.WriteToServer(tableMYSQL);
                }

                //SE CONSULTAN LAS PLANTAS 1
                adapterMYSQL = new MySqlCommand(String.Format(@"

                    SELECT
                        finca AS id_finca_mysql,
                        semana,
                        fecha,
                        inspector AS id_inspector_mysql,
                        planta,
                        ht0,
                        yli0,
                        yls0,
                        hf7,
                        yls7,
                        hf10,
                        yls10,
                        h3,
                        h4,
                        CAST(fecha_actualizacion AS DATETIME) AS fecha_actualizacion,
                        id_usuario AS id_usuario_mysql,
                        CONCAT(finca, '-', semana, '-', fecha) AS llave
                    FROM banasan.plantas

                "), conexionMYSQL.getConexion());
                adapterMYSQL.CommandTimeout = 600;
                reader = adapterMYSQL.ExecuteReader();
                tableMYSQL = new DataTable();
                if (reader.HasRows)
                {
                    tableMYSQL.Load(reader);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

                using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                {
                    foreach (DataColumn col in tableMYSQL.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.plantas1_mysql";
                    bulkCopy.WriteToServer(tableMYSQL);
                }

                //SE CONSULTAN LAS PLANTAS 2
                adapterMYSQL = new MySqlCommand(String.Format(@"

                    SELECT
                        finca AS id_finca_mysql,
                        semana,
                        fecha,
                        inspector AS id_inspector_mysql,
                        planta,
                        efa,
                        fecha_actualizacion,
                        id_usuario AS id_usuario_mysql,
                        CONCAT(finca, '-', semana, '-', fecha) AS llave
                    FROM banasan.plantas2 AS P

                "), conexionMYSQL.getConexion());
                adapterMYSQL.CommandTimeout = 600;
                reader = adapterMYSQL.ExecuteReader();
                tableMYSQL = new DataTable();
                if (reader.HasRows)
                {
                    tableMYSQL.Load(reader);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

                using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                {
                    foreach (DataColumn col in tableMYSQL.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.plantas2_mysql";
                    bulkCopy.WriteToServer(tableMYSQL);
                }

                //SE CONSULTAN LAS PLANTAS 3
                adapterMYSQL = new MySqlCommand(String.Format(@"

                    SELECT
                        finca AS id_finca_mysql,
                        semana,
                        fecha,
                        precipitacion,
                        fecha_precipitacion,
                        fecha_actualizacion,
                        id_usuario AS id_usuario_mysql
                    FROM banasan.plantas3

                "), conexionMYSQL.getConexion());
                adapterMYSQL.CommandTimeout = 600;
                reader = adapterMYSQL.ExecuteReader();
                tableMYSQL = new DataTable();
                if (reader.HasRows)
                {
                    tableMYSQL.Load(reader);
                }
                else
                {
                    Console.WriteLine("No se encontraron datos.");
                }

                using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                {
                    foreach (DataColumn col in tableMYSQL.Columns)
                    {
                        bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                    }

                    bulkCopy.BulkCopyTimeout = 600;
                    bulkCopy.DestinationTableName = "dbo.plantas3_mysql";
                    bulkCopy.WriteToServer(tableMYSQL);
                }


                //SE ORGANIZA EL NOMBRE DE LAS TABLAS DEMYSQL CON EL DEL EXCEL
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE dbo.fincas_mysql
                        SET nombre = 'San Ramón',
                        hectareas = 12
                    WHERE nombre COLLATE SQL_Latin1_General_Cp1_CI_AI = 'San Ramon'

                    UPDATE MYSQL
                    SET MYSQL.nombre = F.FarmName2
                    FROM dbo.fincas_mysql AS MYSQL
                    INNER JOIN dbo.Fincas AS F
                    ON MYSQL.nombre = CASE
                        WHEN F.FarmName2 = 'BUENAVISTA' THEN 'Buena vista'
                        WHEN F.FarmName2 = 'CABALLOS 1' THEN 'Caballo I'
                        WHEN F.FarmName2 = 'CABALLOS 2' THEN 'Caballo II'
                        WHEN F.FarmName2 = 'CECILIA LA FE' THEN 'La Fe Cecilia'
                        WHEN F.FarmName2 = 'DILIA ESTER' THEN 'Dilia Esther'
                        WHEN F.FarmName2 = 'LA CEIBA' THEN 'Ceiba'
                        WHEN F.FarmName2 = 'LAS MARGARITAS' THEN 'las  margaritas'
                        WHEN F.FarmName2 = 'MANANTIAL ORGANICO' THEN 'Manantial Organica'
                        WHEN F.FarmName2 = 'NARANJITOS' THEN 'NARANJITO'
                        WHEN F.FarmName2 = 'ÁNGELES' THEN 'Angeles'
                        WHEN F.FarmName2 = 'EL RUBI' THEN 'El Rubí'
                        WHEN F.FarmName2 = 'SAN JOSÉ' THEN 'San Jose'
                        WHEN F.FarmName2 = 'SAN JOSÉ 2' THEN 'San jose 2'
                        ELSE F.FarmName2
                    END
                    AND MYSQL.nombre <> F.FarmName2

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE ORGANIZA EL GRUPO DE LAS TABLAS DEMYSQL CON EL DEL EXCEL
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE MYSQL
                        SET MYSQL.grupo = F.Grupo
                    FROM dbo.fincas_mysql AS MYSQL
                    INNER JOIN dbo.Fincas AS F
                    ON MYSQL.grupo = CASE
                        WHEN F.Grupo = 'AGROBANACARIBE' THEN 'AGROBANCARIBE'
                        WHEN F.Grupo = 'AGROINVERSIONES LA CEIBA S.A.S.' THEN 'Agroinversines la Ce'
                        WHEN F.Grupo = 'C.I. BANAPALMA S.A.' THEN 'BANAPALMA'
                        WHEN F.Grupo = 'C.I. BANEX S.A.' THEN 'BANEX'
                        WHEN F.Grupo = 'FEDERICA S.A.' THEN 'Federica'
                        WHEN F.Grupo = 'FRUTESA S.A.' THEN 'FRUTESA'
                        WHEN F.Grupo = 'Juan Cueto' THEN 'JUAN CUETO BAUTISTA'
                        ELSE F.Grupo
                    END
                    AND MYSQL.grupo <> F.Grupo

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE INSERTAN LOS SECTORES EN M_RECURSO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_recurso (codigo, nombre, id_M_categoria_recurso)
                    SELECT
                        Municipio COLLATE SQL_Latin1_General_Cp1_CI_AI AS codigo,
                        Municipio COLLATE SQL_Latin1_General_Cp1_CI_AI AS nombre,
                        3 AS id_M_categoria_recurso
                    FROM dbo.Fincas
                    GROUP BY Municipio COLLATE SQL_Latin1_General_Cp1_CI_AI

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE INSERTAN LOS GRUPOS EN M_RECURSO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_recurso (codigo, nombre, id_M_categoria_recurso)
                    SELECT 
	                    S.grupo COLLATE SQL_Latin1_General_Cp1_CI_AI AS codigo,
	                    S.grupo COLLATE SQL_Latin1_General_Cp1_CI_AI AS nombre,
	                    4 AS id_M_categoria_recurso
                    FROM
                    (
	                    SELECT Grupo AS grupo
	                    FROM dbo.Fincas
	                    UNION
	                    SELECT grupo
	                    FROM dbo.fincas_mysql
                    ) AS S
                    WHERE S.grupo IS NOT NULL
                    GROUP BY grupo COLLATE SQL_Latin1_General_Cp1_CI_AI
                    ORDER BY grupo COLLATE SQL_Latin1_General_Cp1_CI_AI

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE INSERTAN LOS FINCAS EN M_RECURSO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_recurso (nombre, id_M_categoria_recurso)
                    SELECT
	                    S.nombre AS nombre,
	                    5 AS id_M_categoria_recurso
                    FROM
                    (
	                    SELECT FarmName2 AS nombre
	                    FROM dbo.Fincas
	                    UNION
	                    SELECT nombre
	                    FROM dbo.fincas_mysql
                    ) AS S
                    WHERE S.nombre <> ''
                    GROUP BY nombre
                    ORDER BY nombre

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE ACTUALIZAN LOS CODIGOS DE LAS FINCAS
                adapter = new SqlDataAdapter(String.Format(@"

                    UPDATE R
                        SET R.codigo = F.Code
                    FROM dbo.M_recurso AS R
                    INNER JOIN dbo.Fincas AS F
                    ON R.nombre = F.FarmName2
                    WHERE R.id_M_categoria_recurso = 5 --CATETGORIA FINCA

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE CREA LA RELACION DE MUNICIPIOS Y FINCAS M_ESTRUCURA_RECURSO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_estructura_recurso
                    (id_recurso_conjunto, id_recurso_subconjunto)
                    SELECT
                        RS.id AS id_recurso_conjunto,
                        RF.id AS id_recurso_subconjunto
                    FROM dbo.Fincas AS F
                    INNER JOIN dbo.M_recurso AS RF
                    ON F.FarmName2 COLLATE SQL_Latin1_General_Cp1_CI_AI = RF.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                    AND RF.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    INNER JOIN dbo.M_recurso AS RS
                    ON F.Sector COLLATE SQL_Latin1_General_Cp1_CI_AI = RS.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                    AND RS.id_M_categoria_recurso = 2 --CATEGORIA SECTOR

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE CREA LA RELACION DE SECTORES Y FINCAS M_ESTRUCURA_RECURSO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_estructura_recurso
                    (id_recurso_conjunto, id_recurso_subconjunto)
                    SELECT
                        RM.id AS id_recurso_conjunto,
                        RF.id AS id_recurso_subconjunto
                    FROM dbo.Fincas AS F
                    INNER JOIN dbo.M_recurso AS RF
                    ON F.FarmName2 COLLATE SQL_Latin1_General_Cp1_CI_AI = RF.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                    AND RF.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    INNER JOIN dbo.M_recurso AS RM
                    ON F.Municipio COLLATE SQL_Latin1_General_Cp1_CI_AI = RM.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                    AND RM.id_M_categoria_recurso = 3 --CATEGORIA MUNICIPIO


                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE CREA LA RELACION DE GRUPOS Y FINCAS M_ESTRUCURA_RECURSO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_estructura_recurso
                    (id_recurso_conjunto, id_recurso_subconjunto)
                    SELECT
                        COALESCE(E.id_recurso_conjunto, M.id_recurso_conjunto) AS id_recurso_conjunto,
                        COALESCE(E.id_recurso_subconjunto, M.id_recurso_subconjunto) AS id_recurso_subconjunto
                    FROM
                    (
                        SELECT
                            RG.id AS id_recurso_conjunto,
                            RF.id AS id_recurso_subconjunto
                            FROM dbo.Fincas AS F
                        INNER JOIN dbo.M_recurso AS RF
                        ON F.FarmName2 COLLATE SQL_Latin1_General_Cp1_CI_AI = RF.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                        AND RF.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                        INNER JOIN dbo.M_recurso AS RG
                        ON F.Grupo COLLATE SQL_Latin1_General_Cp1_CI_AI = RG.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                        AND RG.id_M_categoria_recurso = 4 --CATEGORIA GRUPO
                    ) AS E
                    FULL JOIN
                    (
                        SELECT
                            RG.id AS id_recurso_conjunto,
                            RF.id AS id_recurso_subconjunto
                        FROM dbo.fincas_mysql AS FM
                        INNER JOIN dbo.M_recurso AS RF
                        ON FM.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI = RF.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                        AND RF.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                        INNER JOIN dbo.M_recurso AS RG
                        ON FM.grupo COLLATE SQL_Latin1_General_Cp1_CI_AI = RG.nombre COLLATE SQL_Latin1_General_Cp1_CI_AI
                        AND RG.id_M_categoria_recurso = 4 --CATEGORIA GRUPO
                    ) AS M
                    ON E.id_recurso_subconjunto = M.id_recurso_subconjunto

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE INSERTAN LOS REGISTROS DE PRECIPITACIONES
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.D_precipitaciones (id_recurso, id_usuario, fecha, valor, id_usuario_mysql)
                    SELECT
	                    S.id_recurso,
	                    S.id_usuario,
	                    C.Calendar_Date AS fecha,
	                    S.valor,
	                    S.id_usuario_mysql
                    FROM
                    (
	                    SELECT
			                    R.id AS id_recurso,
			                    1 AS id_usuario,
			                    CAST(SUBSTRING(PM.semana, 1, 4) AS INT) AS ano,
			                    CAST(SUBSTRING(PM.semana, 5, 2) AS INT) AS semana,
			                    PM.precipitacion / 7 AS valor,
			                    PM.id_usuario_mysql AS id_usuario_mysql,
			                    ROW_NUMBER() OVER(PARTITION BY R.id, CAST(SUBSTRING(PM.semana, 1, 4) AS INT), CAST(SUBSTRING(PM.semana, 5, 2) AS INT) ORDER BY PM.fecha_actualizacion ASC) AS orden
	                    FROM dbo.plantas3_mysql AS PM
	                    --id_recurso
	                    INNER JOIN dbo.fincas_mysql AS FM
	                    ON PM.id_finca_mysql = FM.id_grupo
	                    INNER JOIN dbo.M_recurso AS R
	                    ON FM.nombre = R.nombre
	                    AND R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                        WHERE SUBSTRING(PM.semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
                    ) AS S
                    --fechas
                    INNER JOIN dbo.M_calendario AS C
                    ON S.ano = C.Year_ISO
                    AND S.semana = C.Week_of_Year_ISO
                    AND C.Year_ISO >= 2007 --FECHAS DESDE EL 2007
                    AND C.Calendar_Date <= DATEADD(wk,DATEDIFF(wk,0,GETDATE()),6) --FECHAS MENORES O IGUALES AL DIA ACTUAL
                    AND S.orden = 1

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE PASAN LAS VISITAS D_VISITA
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.D_visita
                    (id_recurso, id_usuario, id_tipo_visita, fecha, id_usuario_mysql, id_inspector_mysql, semana_mysql, llave)
                    SELECT
	                    R.id AS id_recurso,
	                    1 AS id_usuario,
	                    1 AS id_tipo_visita,
	                    S.fecha,
	                    S.id_usuario_mysql,
	                    S.id_inspector_mysql,
	                    S.semana AS semana_mysql,
	                    S.llave
                    FROM
                    (
	                    SELECT
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
	                    FROM dbo.plantas1_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
                        AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
	                    HAVING MIN(planta) > 0

	                    UNION

	                    SELECT
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
	                    FROM dbo.plantas2_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
                        AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
                    ) AS S
                    --id_recurso
                    INNER JOIN dbo.fincas_mysql AS FM
                    ON S.id_finca_mysql = FM.id_grupo
                    INNER JOIN dbo.M_recurso AS R
                    ON FM.nombre = R.nombre
                    AND R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    ORDER BY S.fecha

                "), conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 600;
                adapter.SelectCommand.ExecuteScalar();

                //SE PASAN LOS D_PUNTO_CAPTURA
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.D_punto_captura
                    (id_visita, id_tipo, fecha, planta_mysql)
                    SELECT
	                    V.id AS id_visita,
	                    1 AS id_tipo,
	                    V.fecha,
	                    S.planta
                    FROM
                    (
	                    SELECT
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    planta
	                    FROM dbo.plantas1_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
		                    AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    planta
	                    HAVING MIN(planta) > 0

	                    UNION

	                    SELECT
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    planta
	                    FROM dbo.plantas2_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
		                    AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    id_finca_mysql,
		                    semana,
		                    fecha,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    planta
                    ) AS S
                    --id_recurso
                    INNER JOIN dbo.fincas_mysql AS FM
                    ON S.id_finca_mysql = FM.id_grupo
                    INNER JOIN dbo.M_recurso AS R
                    ON FM.nombre = R.nombre
                    AND R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    INNER JOIN dbo.D_visita AS V
                    ON S.id_inspector_mysql = V.id_inspector_mysql
                    AND S.llave = V.llave
                    ORDER BY
	                    V.fecha,
	                    S.planta

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE PASAN LOS D_PLANTA_PUNTO_CAPTURA
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.D_planta_punto_captura
                    (id_punto_captura, id_edad, fecha, planta_mysql)
                    SELECT
	                    PC.id AS id_punto_captura,
	                    E.id AS id_edad,
	                    PC.fecha,
	                    S.planta AS planta_mysql
                    FROM
                    (
	                    SELECT
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
	                    FROM dbo.plantas1_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
		                    AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
	                    HAVING MIN(planta) > 0

	                    UNION

	                    SELECT
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
	                    FROM dbo.plantas2_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
		                    AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave
                    ) AS S
                    INNER JOIN dbo.D_visita AS V
                    ON S.id_inspector_mysql = V.id_inspector_mysql
                    AND S.llave = V.llave
                    INNER JOIN dbo.D_punto_captura AS PC
                    ON V.id = PC.id_visita
                    AND S.planta = PC.planta_mysql
                    INNER JOIN dbo.M_edades AS E
                    ON E.id <> 7
                    ORDER BY
	                    PC.fecha,
	                    S.planta

                "), conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 600;
                adapter.SelectCommand.ExecuteScalar();

                //SE PASAN LOS D_INDICADORES_PUNTO_CAPTURA
                adapter = new SqlDataAdapter(String.Format(@"

                    CREATE TABLE #consulta
                    (
		                    planta INT,
		                    id_usuario_mysql INT,
		                    id_inspector_mysql INT,
		                    llave NVARCHAR(255),
		                    ht0 INT,
		                    yli0 INT,
		                    yls0 INT,
		                    hf7 INT,
		                    yls7 INT,
		                    hf10 INT,
		                    yls10 INT,
		                    h3 INT,
		                    h4 INT
                    )

                    CREATE TABLE #consulta2
                    (
		                    planta INT,
		                    id_usuario_mysql INT,
		                    id_inspector_mysql INT,
		                    llave NVARCHAR(255),
		                    efa FLOAT
                    )

                    INSERT INTO #consulta
                    SELECT
	                    *
                    FROM
                    (
	                    SELECT
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    ht0,
		                    yli0,
		                    yls0,
		                    hf7,
		                    yls7,
		                    hf10,
		                    yls10,
		                    CASE
			                    WHEN h3 = '1+' OR h3 = '+1' OR h3 = '1' THEN 1
			                    WHEN h3 = '1-' OR h3 = '-1' THEN 2
			                    WHEN h3 = '2+' OR h3 = '+2' OR h3 = '2' THEN 3
			                    WHEN h3 = '2-' OR h3 = '-2' THEN 4
			                    WHEN h3 = '3+' OR h3 = '+3' OR h3 = '3' THEN 5
			                    WHEN h3 = '3-' OR h3 = '-3' THEN 6
			                    ELSE NULL
		                    END AS h3,
		                    CASE
			                    WHEN h4 = '1+' OR h4 = '+1' OR h4 = '1' THEN 1
			                    WHEN h4 = '1-' OR h4 = '-1' THEN 2
			                    WHEN h4 = '2+' OR h4 = '+2' OR h4 = '2' THEN 3
			                    WHEN h4 = '2-' OR h4 = '-2' THEN 4
			                    WHEN h4 = '3+' OR h4 = '+3' OR h4 = '3' THEN 5
			                    WHEN h4 = '3-' OR h4 = '-3' THEN 6
			                    ELSE NULL
		                    END AS h4
	                    FROM dbo.plantas1_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
                        AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    ht0,
		                    yli0,
		                    yls0,
		                    hf7,
		                    yls7,
		                    hf10,
		                    yls10,
		                    CASE
			                    WHEN h3 = '1+' OR h3 = '+1' OR h3 = '1' THEN 1
			                    WHEN h3 = '1-' OR h3 = '-1' THEN 2
			                    WHEN h3 = '2+' OR h3 = '+2' OR h3 = '2' THEN 3
			                    WHEN h3 = '2-' OR h3 = '-2' THEN 4
			                    WHEN h3 = '3+' OR h3 = '+3' OR h3 = '3' THEN 5
			                    WHEN h3 = '3-' OR h3 = '-3' THEN 6
			                    ELSE NULL
		                    END,
		                    CASE
			                    WHEN h4 = '1+' OR h4 = '+1' OR h4 = '1' THEN 1
			                    WHEN h4 = '1-' OR h4 = '-1' THEN 2
			                    WHEN h4 = '2+' OR h4 = '+2' OR h4 = '2' THEN 3
			                    WHEN h4 = '2-' OR h4 = '-2' THEN 4
			                    WHEN h4 = '3+' OR h4 = '+3' OR h4 = '3' THEN 5
			                    WHEN h4 = '3-' OR h4 = '-3' THEN 6
			                    ELSE NULL
		                    END
	                    HAVING MIN(planta) > 0
                    ) AS P1

                    INSERT INTO #consulta2
                    SELECT
	                    *
                    FROM
                    (
	                    SELECT
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    efa
	                    FROM dbo.plantas2_mysql
	                    WHERE id_finca_mysql <> 0 --ID FINCA VALIDO
	                    AND DATEPART(yy, fecha) >= 2007 --FECHAS DESDE EL 2007
	                    AND fecha <= CAST(GETDATE() AS DATE) --FECHAS MENORES O IGUALES AL DIA ACTUAL
                        AND SUBSTRING(semana, 5, 6) < 54 --SEMANAS QUE SE ENCUENTREN EN UN RANGO LOGICO MENOR A 54 SEMANAS
	                    GROUP BY
		                    planta,
		                    id_usuario_mysql,
		                    id_inspector_mysql,
		                    llave,
		                    efa
	                    HAVING MIN(planta) > 0
                    ) AS P2

                    INSERT INTO dbo.D_indicadores_punto_captura
                    (id_planta, id_indicador, valor)
                    SELECT
	                    DATA.id_planta,
	                    DATA.id_indicador,
	                    ROUND(DATA.valor, 1) AS valor
                    FROM
                    (
	                    SELECT
		                    PPC.id AS id_planta,
		                    CASE
			                    WHEN S.indicador = 'ht0' THEN 4
			                    WHEN S.indicador = 'yli0' THEN 5
			                    WHEN S.indicador = 'yls0' THEN 2
			                    WHEN S.indicador = 'hf7' THEN 1
			                    WHEN S.indicador = 'yls7' THEN 2
			                    WHEN S.indicador = 'hf10' THEN 1
			                    WHEN S.indicador = 'yls10' THEN 2
			                    WHEN S.indicador = 'h3' THEN 49
			                    WHEN S.indicador = 'h4' THEN 50
			                    WHEN S.indicador = 'efa' THEN 8
			                    ELSE 9999999999999999999999
		                    END AS id_indicador,
		                    S.valor
	                    FROM
	                    (
		                    SELECT
			                    *
		                    FROM #consulta
		                    UNPIVOT
		                    (
			                    valor
			                    FOR indicador IN (ht0, yli0, yls0,	hf7, yls7, hf10, yls10, h3,	h4)
		                    ) AS UNPVT

		                    UNION

		                    SELECT
			                    *
		                    FROM #consulta2
		                    UNPIVOT
		                    (
			                    valor
			                    FOR indicador IN (efa)
		                    ) AS UNPVT
	                    ) AS S
	                    INNER JOIN dbo.D_visita AS V
	                    ON S.id_inspector_mysql = V.id_inspector_mysql
	                    AND S.llave = V.llave
	                    INNER JOIN dbo.D_punto_captura AS PC
	                    ON V.id = PC.id_visita
	                    INNER JOIN dbo.D_planta_punto_captura AS PPC
	                    ON PC.id = PPC.id_punto_captura
	                    AND CASE
		                    WHEN S.indicador = 'ht0' OR S.indicador = 'yli0' OR S.indicador = 'yls0' THEN 3
		                    WHEN S.indicador = 'hf7' OR S.indicador = 'yls7' THEN 2
		                    WHEN S.indicador = 'hf10' OR S.indicador = 'yls10' THEN 1
		                    WHEN S.indicador = 'h3' OR S.indicador = 'h4' OR S.indicador = 'efa' THEN 6
		                    ELSE 9999999999999999999999
	                    END = PPC.id_edad
	                    AND S.planta = PPC.planta_mysql

	                    UNION

	                    SELECT
		                    id AS id_planta,
		                    7 AS id_indicador,
		                    0 AS valor
	                    FROM dbo.D_planta_punto_captura
                        WHERE id_edad IN (1, 2, 3, 6)

	                    UNION

	                    SELECT
		                    id AS id_planta,
		                    4 AS id_indicador,
		                    0 AS valor
	                    FROM dbo.D_planta_punto_captura
                        WHERE id_edad = 6

	                    UNION

	                    SELECT
		                    id AS id_planta,
		                    48 AS id_indicador,
		                    0 AS valor
	                    FROM dbo.D_planta_punto_captura
                        WHERE id_edad = 6
                    ) AS DATA
										WHERE DATA.id_indicador <> 9999999999999999999999
                    ORDER BY
	                    DATA.id_planta,
	                    DATA.id_indicador

                    DROP TABLE #consulta
                    DROP TABLE #consulta2

                "), conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 600;
                adapter.SelectCommand.ExecuteScalar();

                //SE CREAN LOS M_VALOR_ATRIBUTO_DISCRETO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_valor_atributo_discreto
                    (id_atributo, valor)
                    SELECT
	                    S.id_atributo,
	                    S.valor
                    FROM
                    (
	                    SELECT
		                    2	AS id_atributo,
		                    REPLACE(Latitud, ',', '.') AS valor
	                    FROM dbo.Fincas
	                    WHERE Latitud IS NOT NULL
	                    GROUP BY
		                    Latitud

	                    UNION

	                    SELECT
		                    4	AS id_atributo,
		                    REPLACE(Longitud, ',', '.') AS valor
	                    FROM dbo.Fincas
	                    WHERE Longitud IS NOT NULL
	                    GROUP BY
		                    Longitud

	                    UNION

	                    SELECT
		                    6 AS id_atributo,
		                    CASE
			                    WHEN Estado = 'Activa' THEN 'Activo'
			                    ELSE Estado
		                    END AS valor
	                    FROM dbo.Fincas
	                    GROUP BY
		                    Estado

	                    UNION

	                    SELECT
		                    6 AS id_atributo,
		                    'Inactivo' AS valor

                        UNION

                        SELECT
                            8 AS id_atributo,
                            CAST(ROUND(hectareas, 2) AS NVARCHAR(255)) AS valor
                        FROM dbo.fincas_mysql
                        WHERE hectareas <> 0
                        GROUP BY
                            hectareas
                    ) AS S

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE CREAN LOS PADRES PARA M_VALOR_ATRIBUTO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_valor_atributo
                    (id_recurso, id_atributo)
                    SELECT
	                    R.id AS id_recurso,
	                    CASE
		                    WHEN F.id_atributo IS NULL THEN 5
		                    ELSE F.id_atributo
	                    END AS id_atributo
                    FROM dbo.M_recurso AS R
                    LEFT JOIN
                    (
	                    SELECT
		                    FarmName2,
		                    CASE
			                    WHEN atributo = 'Latitud' THEN 1
			                    WHEN atributo = 'Longitud' THEN 3
			                    WHEN atributo = 'Estado' THEN 5
			                    ELSE 999999999999
		                    END AS id_atributo
	                    FROM
	                    (
		                    SELECT
			                    FarmName2,
			                    Latitud,
			                    Longitud,
			                    Estado
		                    FROM dbo.Fincas
	                    ) AS S
	                    UNPIVOT
	                    (
		                    valor
		                    FOR atributo IN (Latitud, Longitud, Estado)
	                    ) AS UNPVT
                    ) AS F
                    ON R.nombre = F.FarmName2
                    WHERE R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    ORDER BY
	                    R.id,
	                    CASE
		                    WHEN F.id_atributo IS NULL THEN 5
		                    ELSE F.id_atributo
	                    END

                    INSERT INTO dbo.M_valor_atributo
                    (id_recurso, id_atributo)
                    SELECT
                        R.id AS id_recurso,
                        7 AS id_atributo
                    FROM dbo.M_recurso AS R
                    INNER JOIN dbo.fincas_mysql AS F
                    ON R.nombre = F.nombre
                    AND F.hectareas <> 0
                    WHERE R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    GROUP BY
                        R.id
                    ORDER BY
	                    R.id

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                //SE CREAN LOS HIJOS PARA M_VALOR_ATRIBUTO
                adapter = new SqlDataAdapter(String.Format(@"

                    INSERT INTO dbo.M_valor_atributo
                    (id_recurso, id_registro, id_atributo, id_valor_discreto)
                    SELECT
	                    R.id AS id_recurso,
	                    VA.id AS id_registro,
	                    VA.id_atributo + 1 AS id_atributo,
	                    VAD.id AS id_valor_discreto
                    FROM dbo.M_recurso AS R
                    LEFT JOIN
                    (
	                    SELECT
		                    FarmName2,
		                    CASE
			                    WHEN atributo = 'Latitud' THEN 1
			                    WHEN atributo = 'Longitud' THEN 3
			                    WHEN atributo = 'Estado' THEN 5
			                    ELSE 999999999999
		                    END AS id_atributo,
		                    REPLACE(valor, ',', '.') AS valor
	                    FROM
	                    (
		                    SELECT
			                    FarmName2,
			                    Latitud,
			                    Longitud,
			                    Estado
		                    FROM dbo.Fincas
	                    ) AS S
	                    UNPIVOT
	                    (
		                    valor
		                    FOR atributo IN (Latitud, Longitud, Estado)
	                    ) AS UNPVT
                    ) AS F
                    ON R.nombre = F.FarmName2
                    INNER JOIN dbo.M_valor_atributo AS VA
                    ON R.id = VA.id_recurso
                    AND CASE
	                    WHEN F.id_atributo IS NULL THEN 5
	                    ELSE F.id_atributo
                    END = VA.id_atributo
                    INNER JOIN dbo.M_valor_atributo_discreto AS VAD
                    ON VA.id_atributo + 1 = VAD.id_atributo
                    AND CASE
	                    WHEN F.valor IS NULL THEN 'Inactivo'
	                    WHEN F.valor = 'Activa' THEN 'Activo'
	                    ELSE F.valor
                    END = VAD.valor
                    WHERE R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    ORDER BY
	                    VA.id

                    INSERT INTO dbo.M_valor_atributo
                    (id_recurso, id_registro, id_atributo, id_valor_discreto)
                    SELECT
                        R.id AS id_recurso,
                        VA.id AS id_registro,
                        VA.id_atributo + 1 AS id_atributo,
                        VAD.id AS id_valor_discreto
                    FROM dbo.M_recurso AS R
                    INNER JOIN dbo.fincas_mysql AS F
                    ON R.nombre = F.nombre
                    AND F.hectareas <> 0
                    INNER JOIN dbo.M_valor_atributo AS VA
                    ON R.id = VA.id_recurso
                    AND VA.id_atributo = 7
                    INNER JOIN dbo.M_valor_atributo_discreto AS VAD
                    ON VA.id_atributo + 1 = VAD.id_atributo
                    AND CAST(ROUND(F.hectareas, 2) AS NVARCHAR(255)) = VAD.valor
                    WHERE R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    GROUP BY
                        R.id,
                        VA.id,
                        VA.id_atributo,
                        VAD.id
                    ORDER BY
                        VA.id

                "), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";

                conexion.closeConexion();
                conexionMYSQL.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, "");

                conexion.closeConexion();
                conexionMYSQL.closeConexion();
            }

        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "ERROR";

            conexion.closeConexion();
            conexionMYSQL.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    // CLASE PARA CONEXION A MYSQL
    public class ConexionMYSQL
    {
        MySqlConnection conexion;

        public ConexionMYSQL()
        {
            string connectionString = "datasource=192.168.0.3;port=3306;username=mysql_banasan;password=7aYI29rhxxZA;database=banasan;Convert Zero Datetime = True";
            conexion = new MySqlConnection(connectionString);
        }
        
        public string openConexion()
        {
            try
            {
                if (conexion.State != ConnectionState.Open)
                {
                    conexion.Open();
                    //Apertura de conexión
                    return "TRUE";
                }
                else
                {
                    //Conexión ya abierta
                    return "TRUE";
                }
            }
            catch (SqlException ex)
            {
                //Error de conexión a base de datos
                return ex.ToString();
            }
        }
        
        public void closeConexion()
        {
            if (conexion.State == ConnectionState.Open)
                conexion.Close();

            //Limpieza de las conexiones abandonadas y que se mantienen abiertas
            MySqlConnection.ClearPool(conexion);
        }

        public MySqlConnection getConexion()
        {
            //Retorna el objeto conexión actualmente disponible
            return this.conexion;
        }        
    }*/
}