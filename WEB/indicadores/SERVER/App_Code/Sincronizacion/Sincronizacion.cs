using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System;
using System.Web;
using System.Web.Script.Services;
using System.Web.Services;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using System.IO;

public class Sincronizacion : WebService
{
    private static class Encrypter
    {
        public static string MD5Text(string text)
        {
            MD5 md5 = new MD5CryptoServiceProvider();

            //compute hash from the bytes of text  
            md5.ComputeHash(ASCIIEncoding.ASCII.GetBytes(text));

            //get hash result after compute it  
            byte[] result = md5.Hash;

            StringBuilder strBuilder = new StringBuilder();
            for (int i = 0; i < result.Length; i++)
            {
                //change it into 2 hexadecimal digits  
                //for each byte  
                strBuilder.Append(result[i].ToString("x2"));
            }

            return strBuilder.ToString();
        }
    }

    internal static readonly char[] chars =
        "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

    public static string GetUniqueKey()
    {
        int size = 32;
        byte[] data = new byte[4 * size];
        using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
        {
            crypto.GetBytes(data);
        }
        StringBuilder result = new StringBuilder(size);
        for (int i = 0; i < size; i++)
        {
            var rnd = BitConverter.ToUInt32(data, i * 4);
            var idx = rnd % chars.Length;

            result.Append(chars[idx]);
        }

        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void Autenticar(string consumerKey, string consumerSecret, string version)
    {
        int UserID = 0;

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT
	                    id
                    FROM
	                    dbo.consumers C
                    WHERE
	                    C.consumer_key = @consumer_key
                    AND C.consumer_secret = @consumer_secret

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@consumer_key", consumerKey);
                adapter.SelectCommand.Parameters.AddWithValue("@consumer_secret", consumerSecret);
                DataSet dtUser = new DataSet();
                adapter.Fill(dtUser);
                DataTable existsUser = dtUser.Tables[0];

                if (existsUser.Rows.Count != 0 && version == "210826")
                {
                    UserID = int.Parse(existsUser.Rows[0][0].ToString());
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"consumerKey: {0}, consumerSecret: {1}, version: {2}", consumerKey, consumerSecret, version));
                conexion.closeConexion();
            }

            if (UserID == 0)
            {
                result["ESTADO"] = "FALSE";
            }
            else
            {
                //Se genera la variable sesion
                Session["UserID"] = UserID;
                string token = GetUniqueKey();

                Session["Token"] = token;

                result["ESTADO"] = "TRUE";
                result["RESULTADO"] = token;
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void ValidacionUsuario(string user, string pass, string departamentos, string ciudades, string sectores, string fincas, string usuarios, string token)
    {
        JObject consultas = new JObject();
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT
	                        id,
                            ISNULL(app_habilitada, 0) AS app_habilitada,
                            contrasena_app AS pass
                        FROM dbo.usuario
                        WHERE usuario = @user

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@user", user);
                    DataSet dtUser = new DataSet();
                    adapter.Fill(dtUser);
                    DataTable existsUser = dtUser.Tables[0];

                    if (existsUser.Rows.Count != 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            ---------------------DEPARTAMENTO---------------------
                            SELECT nombre
                            FROM dbo.M_recurso
                            WHERE id_M_categoria_recurso = 1 --CATEGORIA DEPARTAMENTO
                            ORDER BY id

                            ---------------------CIUDAD---------------------
                            SELECT nombre
                            FROM dbo.M_recurso
                            WHERE id_M_categoria_recurso = 2 --CATEGORIA CIUDAD
                            ORDER BY id

                            ---------------------SECTOR---------------------
                            SELECT nombre
                            FROM dbo.M_recurso
                            WHERE id_M_categoria_recurso = 3 --CATEGORIA SECTOR
                            ORDER BY id

                            ---------------------FINCA---------------------
                            SELECT
	                            R.nombre,
	                            CASE
		                            WHEN VA.id_valor_discreto = 149 THEN 1
		                            ELSE 0
	                            END AS activo,
	                            S.sector
                            FROM dbo.M_recurso AS R
                            INNER JOIN dbo.M_valor_atributo AS VA
                            ON R.id = VA.id_recurso
                            AND VA.id_atributo = 6 --ATRIBUTO ESTADO
                            LEFT JOIN
                            (
	                            SELECT
		                            ER.id_recurso_subconjunto AS finca,
		                            ER.id_recurso_conjunto AS sector		
	                            FROM dbo.M_estructura_recurso AS ER
	                            INNER JOIN dbo.M_recurso AS RS
	                            ON ER.id_recurso_conjunto = RS.id
	                            AND RS.id_M_categoria_recurso = 3 --CATEGORIA SECTOR
                            ) AS S
                            ON R.id = S.finca
                            WHERE R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                            ORDER BY R.id

                            ---------------------USUARIO---------------------
                            SELECT
	                            usuario,
	                            ISNULL(contrasena_app, '') AS pass,
	                            nombres,
	                            apellidos AS apellido,
	                            ISNULL(app_habilitada, 0) AS activo
                            FROM dbo.usuario
                            WHERE usuario <> 'mysql' --EVITA SINCRONIZAR USUARIO DE MIGRACION MYSQL
                            ORDER BY id

                        "), conexion.getConexion());
                        DataSet maestros_ds = new DataSet();
                        adapter.Fill(maestros_ds);
                        DataTable departamento_dt = maestros_ds.Tables[0];
                        DataTable ciudad_dt = maestros_ds.Tables[1];
                        DataTable sector_dt = maestros_ds.Tables[2];
                        DataTable finca_dt = maestros_ds.Tables[3];
                        DataTable usuario_dt = maestros_ds.Tables[4];

                        //Validacion de diferencias entre datos maestros
                        consultas["DEPARTAMENTOS"] = departamentos != Encrypter.MD5Text((JsonConvert.SerializeObject(departamento_dt, Formatting.None))) ? true : false;
                        consultas["CIUDADES"] = ciudades != Encrypter.MD5Text(JsonConvert.SerializeObject(ciudad_dt, Formatting.None)) ? true : false;
                        consultas["SECTORES"] = sectores != Encrypter.MD5Text(JsonConvert.SerializeObject(sector_dt, Formatting.None)) ? true : false;
                        consultas["FINCAS"] = fincas != Encrypter.MD5Text(JsonConvert.SerializeObject(finca_dt, Formatting.None)) ? true : false;
                        consultas["USUARIOS"] = usuarios != Encrypter.MD5Text(JsonConvert.SerializeObject(usuario_dt, Formatting.None)) ? true : false;

                        if (int.Parse(existsUser.Rows[0]["app_habilitada"].ToString()) == 0 || Encrypter.MD5Text(existsUser.Rows[0]["pass"].ToString()) != pass)
                        {
                            result["ESTADO"] = "FALSE_USER";
                        }
                        else
                        {
                            //Se genera la variable sesion
                            Session["UserID"] = int.Parse(existsUser.Rows[0][0].ToString());
                            result["ESTADO"] = "TRUE";
                        }

                        //Se genera nuevo token para la sincronizacion de datos
                        string newToken = GetUniqueKey();
                        Session["Token"] = newToken;
                        consultas["TOKEN"] = newToken;

                        //Se crea registro para indicar inicio de sincronizacion
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.D_sincronizaciones
                            (id_usuario, fecha_inicio)
                            VALUES
                            (@id_usuario, GETDATE())

                            SELECT @@IDENTITY AS id_usuario

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id_usuario", int.Parse(existsUser.Rows[0][0].ToString()));
                        DataSet dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable insertSincronizacion = dt.Tables[0];
                        consultas["SINCRONIZACION"] = int.Parse(insertSincronizacion.Rows[0]["id_usuario"].ToString());

                        result["RESULTADO"] = consultas;
                    }
                    else
                    {
                        result["ESTADO"] = "FALSE";
                    }
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"user: {0}, pass: {1}, token: {2}, departamentos: {3}, ciudades: {4}, sectores: {5}, fincas: {6}, usuarios: {7}", user, pass, token, departamentos, ciudades, sectores, fincas, usuarios));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void SincronizacionMaestros(string tabla, string token)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    DataSet maestros_ds = new DataSet();
                    DataTable maestros_dt = new DataTable();

                    //Se consulta la tabla a sincronizar segun peticion de la APP
                    switch (tabla)
                    {
                        // ------------------------------------------------DEPARTAMENTOS------------------------------------------------
                        case "departamentos":

                            adapter = new SqlDataAdapter(String.Format(@"

                                SELECT
	                                id AS id_server,
	                                nombre
                                FROM dbo.M_recurso
                                WHERE id_M_categoria_recurso = 1 --CATEGORIA DEPARTAMENTO
                                ORDER BY id

                            "), conexion.getConexion());
                            maestros_ds = new DataSet();
                            adapter.Fill(maestros_ds);
                            maestros_dt = maestros_ds.Tables[0];

                            break;

                        // ------------------------------------------------CIUDADES------------------------------------------------
                        case "ciudades":

                            adapter = new SqlDataAdapter(String.Format(@"

                                SELECT
	                                R.id AS id_server,
	                                ER.id_recurso_conjunto AS id_server_departamento,
	                                R.nombre
                                FROM dbo.M_recurso AS R
                                INNER JOIN dbo.M_estructura_recurso AS ER
                                ON R.id  = ER.id_recurso_subconjunto
                                WHERE R.id_M_categoria_recurso = 2 --CATEGORIA CIUDAD
                                ORDER BY R.id

                            "), conexion.getConexion());
                            maestros_ds = new DataSet();
                            adapter.Fill(maestros_ds);
                            maestros_dt = maestros_ds.Tables[0];

                            break;

                        // ------------------------------------------------SECTORES------------------------------------------------
                        case "sectores":

                            adapter = new SqlDataAdapter(String.Format(@"

                                SELECT
	                                id AS id_server,
	                                nombre
                                FROM dbo.M_recurso
                                WHERE id_M_categoria_recurso = 3 --CATEGORIA SECTOR
                                ORDER BY id

                            "), conexion.getConexion());
                            maestros_ds = new DataSet();
                            adapter.Fill(maestros_ds);
                            maestros_dt = maestros_ds.Tables[0];

                            break;

                        // ------------------------------------------------FINCAS------------------------------------------------
                        case "fincas":

                            adapter = new SqlDataAdapter(String.Format(@"

                                SELECT
	                                R.id AS id_server,
	                                C.ciudad AS id_server_ciudad,
	                                R.nombre,
	                                S.sector AS id_server_sector,
	                                CASE
		                                WHEN VA.id_valor_discreto = 149 THEN 1
		                                ELSE 0
	                                END AS activo	
                                FROM dbo.M_recurso AS R
                                INNER JOIN dbo.M_valor_atributo AS VA
                                ON R.id = VA.id_recurso
                                AND VA.id_atributo = 6 --ATRIBUTO ESTADO
                                LEFT JOIN
                                (
	                                SELECT
		                                ER.id_recurso_subconjunto AS finca,
		                                ER.id_recurso_conjunto AS ciudad		
	                                FROM dbo.M_estructura_recurso AS ER
	                                INNER JOIN dbo.M_recurso AS RS
	                                ON ER.id_recurso_conjunto = RS.id
	                                AND RS.id_M_categoria_recurso = 2 --CATEGORIA CIUDAD
                                ) AS C
                                ON R.id = C.finca
                                LEFT JOIN
                                (
	                                SELECT
		                                ER.id_recurso_subconjunto AS finca,
		                                ER.id_recurso_conjunto AS sector		
	                                FROM dbo.M_estructura_recurso AS ER
	                                INNER JOIN dbo.M_recurso AS RS
	                                ON ER.id_recurso_conjunto = RS.id
	                                AND RS.id_M_categoria_recurso = 3 --CATEGORIA SECTOR
                                ) AS S
                                ON R.id = S.finca
                                WHERE R.id_M_categoria_recurso = 5 --CATEGORIA FINCA
                                ORDER BY R.id

                            "), conexion.getConexion());
                            maestros_ds = new DataSet();
                            adapter.Fill(maestros_ds);
                            maestros_dt = maestros_ds.Tables[0];

                            break;

                        // ------------------------------------------------USUARIOS------------------------------------------------
                        case "usuarios":

                            adapter = new SqlDataAdapter(String.Format(@"

                                SELECT
	                                id AS id_server,
	                                usuario,
	                                ISNULL(contrasena_app, '') AS pass,
	                                nombres,
	                                apellidos AS apellido,
	                                ISNULL(app_habilitada, 0) AS activo
                                FROM dbo.usuario
                                WHERE usuario <> 'mysql' --EVITA SINCRONIZAR USUARIO DE MIGRACION MYSQL
                                ORDER BY id                                

                            "), conexion.getConexion());
                            maestros_ds = new DataSet();
                            adapter.Fill(maestros_ds);
                            maestros_dt = maestros_ds.Tables[0];

                            break;

                        default:                            
                            break;
                    }

                    result["ESTADO"] = "TRUE";
                    result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(maestros_dt, Formatting.None));
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"tabla: {0}, token: {1}", tabla, token));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void SincronizacionPrecipitaciones(string tabla, string token)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    //Conversion de la informacion recibida a datatable
                    DataTable tabla_dt = (DataTable)JsonConvert.DeserializeObject(tabla, (typeof(DataTable)));

                    //Tabla temporal para guardar las precipitaciones a sincronizar
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #consulta
                        (
	                        id_recurso INT,
	                        id_usuario INT,
	                        fecha DATETIME2(7),
	                        valor FLOAT(53)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tabla_dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#consulta";
                        bulkCopy.WriteToServer(tabla_dt);
                    }

                    //Procesamiento
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE P
                        SET
	                        P.id_usuario = C.id_usuario,
	                        P.valor = C.valor
                        FROM dbo.D_precipitaciones AS P
                        INNER JOIN #consulta AS C
                        ON P.id_recurso = C.id_recurso
                        AND CAST(P.fecha AS DATE) = CAST(C.fecha AS DATE)

                        INSERT INTO dbo.D_precipitaciones
                        (id_recurso, id_usuario, fecha, valor)
                        SELECT
	                        C.id_recurso,
	                        C.id_usuario,
	                        C.fecha,
	                        C.valor
                        FROM #consulta AS C
                        LEFT JOIN dbo.D_precipitaciones AS P
                        ON C.id_recurso = P.id_recurso
                        AND CAST(P.fecha AS DATE) = CAST(C.fecha AS DATE)
                        WHERE P.id IS NULL

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se borra tabla temporal
                    adapter = new SqlDataAdapter(String.Format(@"

                        DROP TABLE #consulta

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    result["ESTADO"] = "TRUE";
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"tabla: {0}, token: {1}", tabla, token));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void SincronizacionVisitas(string json, string token)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    //Conversion de la informacion recibida a datatable
                    DataSet tablas_ds = (DataSet)JsonConvert.DeserializeObject(json, (typeof(DataSet)));

                    //Estructura temporal para guardar las visitas a sincronizar
                    adapter = new SqlDataAdapter(String.Format(@"

                        ---------------------visita---------------------
                        CREATE TABLE #visita (
                            id INT,
                            id_server_finca INT,
                            id_server_usuario INT,
                            id_server_tipo_visita INT,
                            temperatura_minima FLOAT(53),
                            temperatura FLOAT(53),
                            temperatura_maxima FLOAT(53),
                            humedad_relativa FLOAT(53),
                            fecha DATETIME2(7)
                        )

                        ---------------------recorrido---------------------
                        CREATE TABLE #recorrido (
                            id_visita INT,
                            latitud FLOAT(53),
                            longitud FLOAT(53),
                            fecha DATETIME2(7)
                        )

                        ---------------------punto---------------------
                        CREATE TABLE #punto (
                            id INT,
                            id_visita INT,
                            id_server_tipo INT,
                            fecha DATETIME2(7)
                        )

                        ---------------------planta---------------------
                        CREATE TABLE #planta (
                            id INT,
                            id_punto_captura INT,
                            id_server_edad INT,
                            latitud FLOAT(53),
                            longitud FLOAT(53),
                            fecha DATETIME2(7)
                        )

                        ---------------------indicador---------------------
                        CREATE TABLE #indicador (
                            id INT,
                            id_planta_punto_captura INT,
                            id_server_indicador INT,
                            valor FLOAT(53),
                            valor_texto NVARCHAR(255)
                        )

                        ---------------------foto---------------------
                        CREATE TABLE #foto (
                            id_lectura_indicador INT,
                            url NVARCHAR(255)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena estructura temporal
                    //tablas_ds.Tables[0] - #visita 
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tablas_ds.Tables[0].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#visita";
                        bulkCopy.WriteToServer(tablas_ds.Tables[0]);
                    }

                    //tablas_ds.Tables[1] - #recorrido
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tablas_ds.Tables[1].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#recorrido";
                        bulkCopy.WriteToServer(tablas_ds.Tables[1]);
                    }

                    //tablas_ds.Tables[2] - #punto
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tablas_ds.Tables[2].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#punto";
                        bulkCopy.WriteToServer(tablas_ds.Tables[2]);
                    }

                    //tablas_ds.Tables[3] - #planta
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tablas_ds.Tables[3].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#planta";
                        bulkCopy.WriteToServer(tablas_ds.Tables[3]);
                    }

                    //tablas_ds.Tables[4] - #indicador
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tablas_ds.Tables[4].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#indicador";
                        bulkCopy.WriteToServer(tablas_ds.Tables[4]);
                    }

                    //tablas_ds.Tables[5] - #foto
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tablas_ds.Tables[5].Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#foto";
                        bulkCopy.WriteToServer(tablas_ds.Tables[5]);
                    }

                    //Eliminacion de visitas repetidas por si falla comunicacion con app en una sincronizacion
                    adapter = new SqlDataAdapter(String.Format(@"

                        DELETE V
                        FROM #visita AS V
                        LEFT JOIN dbo.D_visita AS DV
                        ON V.id_server_finca = DV.id_recurso
                        AND V.id_server_usuario = DV.id_usuario
                        AND V.id_server_tipo_visita = DV.id_tipo_visita
                        AND V.fecha = DV.fecha
                        WHERE DV.id IS NOT NULL
                        AND V.fecha < '2021-08-27'

                    "), conexion.getConexion());
                    adapter.SelectCommand.CommandTimeout = 900;
                    adapter.SelectCommand.ExecuteScalar();

                    //Procesamiento
                    adapter = new SqlDataAdapter(String.Format(@"

                        ---------------------visita---------------------
                        INSERT INTO dbo.D_visita
                        (id_recurso, id_usuario, id_tipo_visita, temperatura_minima, temperatura, temperatura_maxima, humedad_relativa, fecha)
                        SELECT
	                        id_server_finca AS id_recurso,
	                        id_server_usuario AS id_usuario,
	                        id_server_tipo_visita AS id_tipo_visita,
	                        temperatura_minima,
	                        temperatura,
	                        temperatura_maxima,
	                        humedad_relativa,
	                        fecha
                        FROM #visita

                        ---------------------recorrido---------------------
                        INSERT INTO dbo.D_recorrido_visita
                        (id_visita, latitud, longitud, fecha)
                        SELECT
	                        DV.id AS id_visita,
	                        R.latitud,
	                        R.longitud,
	                        R.fecha
                        FROM #recorrido AS R
                        INNER JOIN #visita AS V
                        ON R.id_visita = V.id
                        INNER JOIN dbo.D_visita AS DV
                        ON V.id_server_finca = DV.id_recurso
                        AND V.id_server_usuario = DV.id_usuario
                        AND V.id_server_tipo_visita = DV.id_tipo_visita
                        AND V.fecha = DV.fecha

                        ---------------------punto---------------------
                        INSERT INTO dbo.D_punto_captura
                        (id_visita, id_tipo, fecha)
                        SELECT
	                        DV.id AS id_visita,
	                        P.id_server_tipo AS id_tipo,
	                        P.fecha
                        FROM #punto AS P
                        INNER JOIN #visita AS V
                        ON P.id_visita = V.id
                        INNER JOIN dbo.D_visita AS DV
                        ON V.id_server_finca = DV.id_recurso
                        AND V.id_server_usuario = DV.id_usuario
                        AND V.id_server_tipo_visita = DV.id_tipo_visita
                        AND V.fecha = DV.fecha

                        ---------------------planta---------------------
                        INSERT INTO dbo.D_planta_punto_captura
                        (id_punto_captura, id_edad, latitud, longitud, fecha)
                        SELECT
	                        DPC.id AS id_punto_captura,
	                        P.id_server_edad AS id_edad,
	                        P.latitud,
	                        P.longitud,
	                        P.fecha
                        FROM #planta AS P
                        INNER JOIN #punto AS PC
                        ON P.id_punto_captura = PC.id
                        INNER JOIN #visita AS V
                        ON PC.id_visita = V.id
                        INNER JOIN dbo.D_visita AS DV
                        ON V.id_server_finca = DV.id_recurso
                        AND V.id_server_usuario = DV.id_usuario
                        AND V.id_server_tipo_visita = DV.id_tipo_visita
                        AND V.fecha = DV.fecha
                        INNER JOIN dbo.D_punto_captura AS DPC
                        ON DV.id = DPC.id_visita
                        AND PC.fecha = DPC.fecha
                        AND PC.id_server_tipo = DPC.id_tipo

                        ---------------------indicador---------------------
                        INSERT INTO dbo.D_indicadores_punto_captura
                        (id_planta, id_indicador, valor, valor_texto)
                        SELECT
	                        DPPC.id AS id_planta,
	                        I.id_server_indicador AS id_indicador,
	                        I.valor,
	                        I.valor_texto
                        FROM #indicador AS I
                        INNER JOIN #planta AS P
                        ON I.id_planta_punto_captura = P.id
                        INNER JOIN #punto AS PC
                        ON P.id_punto_captura = PC.id
                        INNER JOIN #visita AS V
                        ON PC.id_visita = V.id
                        INNER JOIN dbo.D_visita AS DV
                        ON V.id_server_finca = DV.id_recurso
                        AND V.id_server_usuario = DV.id_usuario
                        AND V.id_server_tipo_visita = DV.id_tipo_visita
                        AND V.fecha = DV.fecha
                        INNER JOIN dbo.D_punto_captura AS DPC
                        ON DV.id = DPC.id_visita
                        AND PC.fecha = DPC.fecha
                        AND PC.id_server_tipo = DPC.id_tipo
                        INNER JOIN dbo.D_planta_punto_captura AS DPPC
                        ON DPC.id = DPPC.id_punto_captura
                        AND P.id_server_edad = DPPC.id_edad
                        AND P.fecha = DPPC.fecha

                        ---------------------foto---------------------
                        INSERT INTO dbo.D_fotos_indicador
                        (id_lectura_indicador, url)
                        SELECT
	                        DIPC.id AS id_lectura_indicador,
	                        F.url AS url
                        FROM #foto AS F
                        INNER JOIN #indicador AS I
                        ON F.id_lectura_indicador = I.id
                        INNER JOIN #planta AS P
                        ON I.id_planta_punto_captura = P.id
                        INNER JOIN #punto AS PC
                        ON P.id_punto_captura = PC.id
                        INNER JOIN #visita AS V
                        ON PC.id_visita = V.id
                        INNER JOIN dbo.D_visita AS DV
                        ON V.id_server_finca = DV.id_recurso
                        AND V.id_server_usuario = DV.id_usuario
                        AND V.id_server_tipo_visita = DV.id_tipo_visita
                        AND V.fecha = DV.fecha
                        INNER JOIN dbo.D_punto_captura AS DPC
                        ON DV.id = DPC.id_visita
                        AND PC.fecha = DPC.fecha
                        AND PC.id_server_tipo = DPC.id_tipo
                        INNER JOIN dbo.D_planta_punto_captura AS DPPC
                        ON DPC.id = DPPC.id_punto_captura
                        AND P.id_server_edad = DPPC.id_edad
                        AND P.fecha = DPPC.fecha
                        INNER JOIN dbo.D_indicadores_punto_captura AS DIPC
                        ON DPPC.id = DIPC.id_planta
                        AND I.id_server_indicador = DIPC.id_indicador

                    "), conexion.getConexion());
                    adapter.SelectCommand.CommandTimeout = 900;
                    adapter.SelectCommand.ExecuteScalar();

                    //Se borra estructura temporal
                    adapter = new SqlDataAdapter(String.Format(@"

                        DROP TABLE #visita
                        DROP TABLE #recorrido
                        DROP TABLE #punto
                        DROP TABLE #planta
                        DROP TABLE #indicador
                        DROP TABLE #foto

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    result["ESTADO"] = "TRUE";
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"json: {0}, token: {1}", json, token));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public string SincronizacionFotos()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();        
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                string token = HttpContext.Current.Request.Form["token"].ToString();

                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    string carpeta = HttpContext.Current.Request.Form["carpeta"].ToString();
                    string name = HttpContext.Current.Request.Form["nombre"].ToString();
                    string ext = HttpContext.Current.Request.Form["ext"].ToString();
                    string path = string.Format(@"~/{0}/{1}.{2}", carpeta, name, ext);

                    if (HttpContext.Current.Request.Files.Count > 0)
                    {
                        HttpPostedFile file = HttpContext.Current.Request.Files.Get(0);

                        //SE MAPEA LA RUTA DONDE SE GUARDARA EL ARCHIVO
                        string pathBase = HttpContext.Current.Server.MapPath(path);
                        //SE BORRA EL ARCHIVO SI YA EXISTE
                        if (File.Exists(pathBase))
                        {
                            File.Delete(pathBase);
                        }

                        file.SaveAs(pathBase);

                        result["ESTADO"] = "TRUE";
                    }
                    else
                    {
                        result["ESTADO"] = "FALSE";
                    }
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }                

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"carpeta='{0}', nombre='{1}'",
                HttpContext.Current.Request.Form["carpeta"].ToString(),
                HttpContext.Current.Request.Form["nombre"].ToString()));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();

        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void FechaFinSincronizacion(string id, string token)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    //Actualiza fecha fin de registro de sincronizacion
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE dbo.D_sincronizaciones
                        SET fecha_finalizacion = GETDATE()
                        WHERE id = @id

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    adapter.SelectCommand.ExecuteScalar();                    

                    result["ESTADO"] = "TRUE";
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id: {0}, token: {1}", id, token));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }

    [WebMethod(EnableSession = true)]
    [ScriptMethod(UseHttpGet = false, ResponseFormat = ResponseFormat.Json)]
    public void SincronizacionErrores(string tabla, string token)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (Session["UserID"] != null && Session["Token"] != null && (string)(Session["Token"]) == token)
                {
                    //Conversion de la informacion recibida a datatable
                    DataTable tabla_dt = (DataTable)JsonConvert.DeserializeObject(tabla, (typeof(DataTable)));

                    //Tabla temporal para guardar los errores a sincronizar
                    adapter = new SqlDataAdapter(String.Format(@"

                        CREATE TABLE #consulta
                        (
	                        id_usuario INT,
	                        id_servicio INT,
	                        fecha DATETIME2(7),
	                        mensaje NVARCHAR(4000)
                        )

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se llena tabla temporal
                    using (var bulkCopy = new SqlBulkCopy(conexion.getConexion()))
                    {
                        foreach (DataColumn col in tabla_dt.Columns)
                        {
                            bulkCopy.ColumnMappings.Add(col.ColumnName, col.ColumnName);
                        }

                        bulkCopy.BulkCopyTimeout = 600;
                        bulkCopy.DestinationTableName = "#consulta";
                        bulkCopy.WriteToServer(tabla_dt);
                    }

                    //Procesamiento
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.D_errores
                        (id_usuario, id_servicio, fecha, mensaje)
                        SELECT
	                        id_usuario,
	                        id_servicio,
	                        fecha,
	                        mensaje
                        FROM #consulta

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se borra tabla temporal
                    adapter = new SqlDataAdapter(String.Format(@"

                        DROP TABLE #consulta

                    "), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    result["ESTADO"] = "TRUE";
                }
                else
                {
                    result["ESTADO"] = "FALSE";
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"tabla: {0}, token: {1}", tabla, token));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            conexion.closeConexion();
        }

        Context.Response.Write(result.ToString());
    }
}
