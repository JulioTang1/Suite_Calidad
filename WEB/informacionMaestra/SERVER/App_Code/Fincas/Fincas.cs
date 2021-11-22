using System;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;

public class Fincas : WebService
{
    //CONSULTA OPCIONES DE SELECTORES PARA EL FORMULARIO DE CREACION DE FINCAS (Departamento, Sector, Grupo, Estado)
    [WebMethod(EnableSession = true)]
    public string select_form()
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

                    ---------------------Departamentos-----------------------
                    SELECT
	                    id,
	                    nombre
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 1 --CATEGORIA DEPARTAMENTO
                    ORDER BY
	                    nombre

                    ---------------------Sectores-----------------------
                    SELECT
	                    id,
	                    nombre
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 3 --CATEGORIA SECTOR
                    ORDER BY
	                    nombre

                    ---------------------Grupos-----------------------
                    SELECT
	                    id,
	                    nombre
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 4 --CATEGORIA GRUPO
                    ORDER BY
	                    nombre

                    ---------------------Estados-----------------------
                    SELECT
	                    valor AS id,
	                    valor AS nombre
                    FROM dbo.M_valor_atributo_discreto
                    WHERE id_atributo = 6 --ATRIBUTO ESTADO
                    ORDER BY
	                    valor

                    ---------------------Organica-----------------------
                    SELECT
	                    valor AS id,
	                    valor AS nombre
                    FROM dbo.M_valor_atributo_discreto
                    WHERE id_atributo = 10 --ATRIBUTO ORGANICA
                    ORDER BY
	                    valor

                "), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable departamentos = dt.Tables[0];
                DataTable sectores = dt.Tables[1];
                DataTable grupos = dt.Tables[2];
                DataTable estados = dt.Tables[3];
                DataTable organica = dt.Tables[4];

                resultado["DEPARTAMENTOS"] = JArray.Parse(JsonConvert.SerializeObject(departamentos, Formatting.None));
                resultado["SECTORES"] = JArray.Parse(JsonConvert.SerializeObject(sectores, Formatting.None));
                resultado["GRUPOS"] = JArray.Parse(JsonConvert.SerializeObject(grupos, Formatting.None));
                resultado["ESTADOS"] = JArray.Parse(JsonConvert.SerializeObject(estados, Formatting.None));
                resultado["ORGANICA"] = JArray.Parse(JsonConvert.SerializeObject(organica, Formatting.None));

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

    //TRAE LA LISTA DE MUNICIPIOS PARA UN DEPARTAMENTO SELECCIONADO
    [WebMethod(EnableSession = true)]
    public string bring_municipios(int id)
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
	                    R.id,
	                    R.nombre
                    FROM dbo.M_recurso AS R
                    INNER JOIN dbo.M_estructura_recurso AS ER
                    ON R.id = ER.id_recurso_subconjunto
                    AND ER.id_recurso_conjunto = @id

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id", id);
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

    //SE CREA UNA NUEVA FINCA VALIDANDO QUE NO ESTE REPETIDA
    [WebMethod(EnableSession = true)]
    public string insert_finca(string codigo, string nombre, int municipio, int sector, int grupo, string estado, float latitud, float longitud, float hectareas, string organica)
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
                //Se verifica que el codigo y el nombre de la finca no esten repetidos
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT codigo
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    AND codigo = @codigo

                    SELECT nombre
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    AND nombre COLLATE SQL_Latin1_General_Cp1_CI_AI = @nombre

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@codigo", codigo);
                adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombre);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable existCodigo = dt.Tables[0];
                DataTable existNombre = dt.Tables[1];

                //Registra solo si el codigo o el nombre no estan repetidos
                if (existCodigo.Rows.Count == 0 && existNombre.Rows.Count == 0)
                {
                    //Se crea la finca
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_recurso
                        (codigo, nombre, id_M_categoria_recurso)
                        VALUES
                        (@codigo, @nombre, 5)

                        SELECT @@IDENTITY AS id_finca

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@codigo", codigo);
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombre);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable insertFinca = dt.Tables[0];

                    //Se relaciona con el municipio, el sector y el grupo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_estructura_recurso
                        (id_recurso_conjunto, id_recurso_subconjunto)
                        VALUES
                        (@municipio, @finca),
                        (@sector, @finca),
                        (@grupo, @finca)

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@municipio", municipio);
                    adapter.SelectCommand.Parameters.AddWithValue("@sector", sector);
                    adapter.SelectCommand.Parameters.AddWithValue("@grupo", grupo);
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    adapter.SelectCommand.ExecuteScalar();

                    //Se busca si el valor de los atributos existe
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 6 --ATRIBUTO ESTADO
                        AND valor = @estado

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 2 --ATRIBUTO LATITUD
                        AND valor = @latitud

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 4 --ATRIBUTO LONGITUD
                        AND valor = @longitud

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 8 --ATRIBUTO HECTAREA
                        AND valor = @hectareas

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 10 --ATRIBUTO ORGANICA
                        AND valor = @organica

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@estado", estado);
                    adapter.SelectCommand.Parameters.AddWithValue("@latitud", latitud);
                    adapter.SelectCommand.Parameters.AddWithValue("@longitud", longitud);
                    adapter.SelectCommand.Parameters.AddWithValue("@hectareas", hectareas);
                    adapter.SelectCommand.Parameters.AddWithValue("@organica", organica);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable idValorEstado = dt.Tables[0];
                    DataTable idValorLatitud = dt.Tables[1];
                    DataTable idValorLongitud = dt.Tables[2];
                    DataTable idValorHectarea = dt.Tables[3];
                    DataTable idValorOrganica = dt.Tables[4];

                    int idValorEstadoInt;
                    int idValorLatitudInt;
                    int idValorLongitudInt;
                    int idValorHectareaInt;
                    int idValorOrganicaInt;

                    //Se extrae el id del valor y si no existe se inserta el valor y retorna el id
                    // --------------------------------------------- Estado
                    if (idValorEstado.Rows.Count > 0)
                    {
                        idValorEstadoInt = int.Parse(idValorEstado.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (6, @estado) --ATRIBUTO ESTADO

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@estado", estado);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorEstadoInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }
                    //padre
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_atributo)
                        VALUES
                        (@finca, 5) --ATRIBUTO PADRE ESTADO

                        SELECT @@IDENTITY AS id_estado_padre

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable tabla_id_estado_padre = dt.Tables[0];
                    int id_estado_padre = int.Parse(tabla_id_estado_padre.Rows[0]["id_estado_padre"].ToString());
                    //hijo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_registro, id_atributo, id_valor_discreto)
                        VALUES
                        (@finca, @id_registro_padre, 6, @id_valor_discreto) --ATRIBUTO ESTADO

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_estado_padre);
                    adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorEstadoInt);
                    adapter.SelectCommand.ExecuteScalar();

                    // --------------------------------------------- Latitud
                    if (idValorLatitud.Rows.Count > 0)
                    {
                        idValorLatitudInt = int.Parse(idValorLatitud.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (2, @latitud) --ATRIBUTO LATITUD

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@latitud", latitud);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorLatitudInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }
                    //padre
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_atributo)
                        VALUES
                        (@finca, 1) --ATRIBUTO PADRE LATITUD

                        SELECT @@IDENTITY AS id_latitud_padre

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable tabla_id_latitud_padre = dt.Tables[0];
                    int id_latitud_padre = int.Parse(tabla_id_latitud_padre.Rows[0]["id_latitud_padre"].ToString());
                    //hijo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_registro, id_atributo, id_valor_discreto)
                        VALUES
                        (@finca, @id_registro_padre, 2, @id_valor_discreto) --ATRIBUTO LATITUD

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_latitud_padre);
                    adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorLatitudInt);
                    adapter.SelectCommand.ExecuteScalar();

                    // --------------------------------------------- Longitud
                    if (idValorLongitud.Rows.Count > 0)
                    {
                        idValorLongitudInt = int.Parse(idValorLongitud.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (4, @longitud) --ATRIBUTO LONGITUD

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@longitud", longitud);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorLongitudInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }
                    //padre
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_atributo)
                        VALUES
                        (@finca, 3) --ATRIBUTO PADRE LONGITUD

                        SELECT @@IDENTITY AS id_longitud_padre

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable tabla_id_longitud_padre = dt.Tables[0];
                    int id_longitud_padre = int.Parse(tabla_id_longitud_padre.Rows[0]["id_longitud_padre"].ToString());
                    //hijo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_registro, id_atributo, id_valor_discreto)
                        VALUES
                        (@finca, @id_registro_padre, 4, @id_valor_discreto) --ATRIBUTO LONGITUD

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_longitud_padre);
                    adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorLongitudInt);
                    adapter.SelectCommand.ExecuteScalar();

                    // --------------------------------------------- Hectarea
                    if (idValorHectarea.Rows.Count > 0)
                    {
                        idValorHectareaInt = int.Parse(idValorHectarea.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (8, @hectareas) --ATRIBUTO HECTAREA

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@hectareas", hectareas);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorHectareaInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }
                    //padre
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_atributo)
                        VALUES
                        (@finca, 7) --ATRIBUTO PADRE HECTAREA

                        SELECT @@IDENTITY AS id_hectarea_padre

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable tabla_id_hectarea_padre = dt.Tables[0];
                    int id_hectarea_padre = int.Parse(tabla_id_hectarea_padre.Rows[0]["id_hectarea_padre"].ToString());
                    //hijo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_registro, id_atributo, id_valor_discreto)
                        VALUES
                        (@finca, @id_registro_padre, 8, @id_valor_discreto) --ATRIBUTO HECTAREA

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_hectarea_padre);
                    adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorHectareaInt);
                    adapter.SelectCommand.ExecuteScalar();

                    // --------------------------------------------- Organica
                    if (idValorOrganica.Rows.Count > 0)
                    {
                        idValorOrganicaInt = int.Parse(idValorOrganica.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (10, @organica) --ATRIBUTO ORGANICA

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@organica", organica);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorOrganicaInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }
                    //padre
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_atributo)
                        VALUES
                        (@finca, 9) --ATRIBUTO PADRE ORGANICA

                        SELECT @@IDENTITY AS id_organica_padre

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable tabla_id_organica_padre = dt.Tables[0];
                    int id_organica_padre = int.Parse(tabla_id_organica_padre.Rows[0]["id_organica_padre"].ToString());
                    //hijo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_valor_atributo
                        (id_recurso, id_registro, id_atributo, id_valor_discreto)
                        VALUES
                        (@finca, @id_registro_padre, 10, @id_valor_discreto) --ATRIBUTO ORGANICA

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@finca", insertFinca.Rows[0]["id_finca"].ToString());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_organica_padre);
                    adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorOrganicaInt);
                    adapter.SelectCommand.ExecuteScalar();
                }
                else
                {
                    error["CODIGO"] = existCodigo.Rows.Count;
                    error["NOMBRE"] = existNombre.Rows.Count;
                    result["RESULTADO"] = error;
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
                Mail.SendEmail(e, host, string.Format(@"codigo={0}, nombre={1}, municipio={2}, sector={3}, grupo={4}, estado={5}, latitud={6}, longitud={7}, hectareas={8}, organica={9}", codigo, nombre, municipio, sector, grupo, estado, latitud, longitud, hectareas, organica));
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

    //SE ACTUALIZA LA INFORMACION DE UNA FINCA VALIDANDO QUE NO INTERFIERA CON EL CODIGO O NOMBRE DE OTRA
    [WebMethod(EnableSession = true)]
    public string edit_finca(int id, string codigo, string nombre, int municipio, int sector, int grupo, string estado, float latitud, float longitud, float hectareas, string organica)
    {
        //Aca se guarda el JObject consultas
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        //Objeto que guarda errores de cogigo
        JObject error = new JObject();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se verifica que el codigo y el nombre de la finca no esten repetidos
                adapter = new SqlDataAdapter(String.Format(@"

                    SELECT codigo
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    AND id <> @id
                    AND codigo = @codigo

                    SELECT nombre
                    FROM dbo.M_recurso
                    WHERE id_M_categoria_recurso = 5 --CATEGORIA FINCA
                    AND id <> @id
                    AND nombre COLLATE SQL_Latin1_General_Cp1_CI_AI = @nombre

                "), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                adapter.SelectCommand.Parameters.AddWithValue("@codigo", codigo);
                adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombre);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable existCodigo = dt.Tables[0];
                DataTable existNombre = dt.Tables[1];

                //Actualiza solo si el codigo o el nombre no estan repetidos
                if (existCodigo.Rows.Count == 0 && existNombre.Rows.Count == 0)
                {
                    //Se actualiza codigo y nombre de la finca
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE dbo.M_recurso
                        SET
	                        codigo = @codigo,
	                        nombre = @nombre
                        WHERE id = @id

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    adapter.SelectCommand.Parameters.AddWithValue("@codigo", codigo);
                    adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombre);
                    adapter.SelectCommand.ExecuteScalar();

                    //Se actualizan las relaciones existentes con el municipio, el sector y el grupo
                    adapter = new SqlDataAdapter(String.Format(@"

                        UPDATE ER
                        SET ER.id_recurso_conjunto = S.id
                        FROM dbo.M_estructura_recurso AS ER
                        INNER JOIN dbo.M_recurso AS R
                        ON ER.id_recurso_conjunto = R.id
                        INNER JOIN
                        (
	                        SELECT @municipio AS id, 2 AS categoria
	                        UNION
	                        SELECT @sector AS id, 3 AS categoria
	                        UNION
	                        SELECT @grupo AS id, 4 AS categoria
                        ) AS S
                        ON R.id_M_categoria_recurso = S.categoria
                        WHERE ER.id_recurso_subconjunto = @id

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    adapter.SelectCommand.Parameters.AddWithValue("@municipio", municipio);
                    adapter.SelectCommand.Parameters.AddWithValue("@sector", sector);
                    adapter.SelectCommand.Parameters.AddWithValue("@grupo", grupo);
                    adapter.SelectCommand.ExecuteScalar();

                    //Se insertan las relaciones no existentes con el municipio, el sector y el grupo
                    adapter = new SqlDataAdapter(String.Format(@"

                        INSERT INTO dbo.M_estructura_recurso
                        (id_recurso_conjunto, id_recurso_subconjunto)
                        SELECT
	                        S.id AS id_recurso_conjunto,
	                        R.id AS id_recurso_subconjunto
                        FROM dbo.M_recurso AS R
                        CROSS JOIN
                        (
	                        SELECT @municipio AS id
	                        UNION
	                        SELECT @sector AS id
	                        UNION
	                        SELECT @grupo AS id
                        ) AS S
                        LEFT JOIN dbo.M_estructura_recurso AS ER
                        ON S.id = ER.id_recurso_conjunto
                        AND R.id = ER.id_recurso_subconjunto
                        WHERE R.id = @id
                        AND ER.id IS NULL

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    adapter.SelectCommand.Parameters.AddWithValue("@municipio", municipio);
                    adapter.SelectCommand.Parameters.AddWithValue("@sector", sector);
                    adapter.SelectCommand.Parameters.AddWithValue("@grupo", grupo);
                    adapter.SelectCommand.ExecuteScalar();

                    //Se busca si el valor de los atributos existe
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 6 --ATRIBUTO ESTADO
                        AND valor = @estado

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 2 --ATRIBUTO LATITUD
                        AND valor = @latitud

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 4 --ATRIBUTO LONGITUD
                        AND valor = @longitud

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 8 --ATRIBUTO HECTAREA
                        AND valor = @hectareas

                        SELECT id
                        FROM dbo.M_valor_atributo_discreto
                        WHERE id_atributo = 10 --ATRIBUTO ORGANICA
                        AND valor = @organica

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@estado", estado);
                    adapter.SelectCommand.Parameters.AddWithValue("@latitud", latitud);
                    adapter.SelectCommand.Parameters.AddWithValue("@longitud", longitud);
                    adapter.SelectCommand.Parameters.AddWithValue("@hectareas", hectareas);
                    adapter.SelectCommand.Parameters.AddWithValue("@organica", organica);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable idValorEstado = dt.Tables[0];
                    DataTable idValorLatitud = dt.Tables[1];
                    DataTable idValorLongitud = dt.Tables[2];
                    DataTable idValorHectarea = dt.Tables[3];
                    DataTable idValorOrganica = dt.Tables[4];

                    int idValorEstadoInt;
                    int idValorLatitudInt;
                    int idValorLongitudInt;
                    int idValorHectareaInt;
                    int idValorOrganicaInt;

                    //Se extrae el id del valor y si no existe se inserta el valor y retorna el id
                    // --------------------------------------------- Estado
                    if (idValorEstado.Rows.Count > 0)
                    {
                        idValorEstadoInt = int.Parse(idValorEstado.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (6, @estado) --ATRIBUTO ESTADO

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@estado", estado);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorEstadoInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }

                    //Se busca si la finca ya tiene definido el atributo
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo
                        WHERE id_recurso = @id
                        AND id_atributo = 6 --ATRIBUTO ESTADO HIJO

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable existEstado = dt.Tables[0];

                    //Se actualiza el valor del atributo si ya tiene uno configurado, de lo contrario lo crea desde cero
                    if (existEstado.Rows.Count > 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            UPDATE dbo.M_valor_atributo
                            SET id_valor_discreto = @id_valor_discreto
                            WHERE id = @id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id", int.Parse(existEstado.Rows[0]["id"].ToString()));
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorEstadoInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    else
                    {
                        //padre
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_atributo)
                            VALUES
                            (@finca, 5) --ATRIBUTO PADRE ESTADO

                            SELECT @@IDENTITY AS id_estado_padre

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable tabla_id_estado_padre = dt.Tables[0];
                        int id_estado_padre = int.Parse(tabla_id_estado_padre.Rows[0]["id_estado_padre"].ToString());
                        //hijo
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_registro, id_atributo, id_valor_discreto)
                            VALUES
                            (@finca, @id_registro_padre, 6, @id_valor_discreto) --ATRIBUTO ESTADO

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_estado_padre);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorEstadoInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }

                    // --------------------------------------------- Latitud
                    if (idValorLatitud.Rows.Count > 0)
                    {
                        idValorLatitudInt = int.Parse(idValorLatitud.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (2, @latitud) --ATRIBUTO LATITUD

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@latitud", latitud);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorLatitudInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }

                    //Se busca si la finca ya tiene definido el atributo
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo
                        WHERE id_recurso = @id
                        AND id_atributo = 2 --ATRIBUTO LATITUD HIJO

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable existLatitud = dt.Tables[0];

                    //Se actualiza el valor del atributo si ya tiene uno configurado, de lo contrario lo crea desde cero
                    if (existLatitud.Rows.Count > 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            UPDATE dbo.M_valor_atributo
                            SET id_valor_discreto = @id_valor_discreto
                            WHERE id = @id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id", int.Parse(existLatitud.Rows[0]["id"].ToString()));
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorLatitudInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    else
                    {
                        //padre
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_atributo)
                            VALUES
                            (@finca, 1) --ATRIBUTO PADRE LATITUD

                            SELECT @@IDENTITY AS id_latitud_padre

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable tabla_id_latitud_padre = dt.Tables[0];
                        int id_latitud_padre = int.Parse(tabla_id_latitud_padre.Rows[0]["id_latitud_padre"].ToString());
                        //hijo
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_registro, id_atributo, id_valor_discreto)
                            VALUES
                            (@finca, @id_registro_padre, 2, @id_valor_discreto) --ATRIBUTO LATITUD

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_latitud_padre);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorLatitudInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }

                    // --------------------------------------------- Longitud
                    if (idValorLongitud.Rows.Count > 0)
                    {
                        idValorLongitudInt = int.Parse(idValorLongitud.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (4, @longitud) --ATRIBUTO LONGITUD

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@longitud", longitud);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorLongitudInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }

                    //Se busca si la finca ya tiene definido el atributo
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo
                        WHERE id_recurso = @id
                        AND id_atributo = 4 --ATRIBUTO LONGITUD HIJO

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable existLongitud = dt.Tables[0];

                    //Se actualiza el valor del atributo si ya tiene uno configurado, de lo contrario lo crea desde cero
                    if (existLongitud.Rows.Count > 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            UPDATE dbo.M_valor_atributo
                            SET id_valor_discreto = @id_valor_discreto
                            WHERE id = @id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id", int.Parse(existLongitud.Rows[0]["id"].ToString()));
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorLongitudInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    else
                    {
                        //padre
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_atributo)
                            VALUES
                            (@finca, 3) --ATRIBUTO PADRE LONGITUD

                            SELECT @@IDENTITY AS id_longitud_padre

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable tabla_id_longitud_padre = dt.Tables[0];
                        int id_longitud_padre = int.Parse(tabla_id_longitud_padre.Rows[0]["id_longitud_padre"].ToString());
                        //hijo
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_registro, id_atributo, id_valor_discreto)
                            VALUES
                            (@finca, @id_registro_padre, 4, @id_valor_discreto) --ATRIBUTO LONGITUD

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_longitud_padre);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorLongitudInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }

                    // --------------------------------------------- Hectarea
                    if (idValorHectarea.Rows.Count > 0)
                    {
                        idValorHectareaInt = int.Parse(idValorHectarea.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (8, @hectareas) --ATRIBUTO HECTAREA

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@hectareas", hectareas);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorHectareaInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }

                    //Se busca si la finca ya tiene definido el atributo
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo
                        WHERE id_recurso = @id
                        AND id_atributo = 8 --ATRIBUTO HECTAREA HIJO

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable existHectarea = dt.Tables[0];

                    //Se actualiza el valor del atributo si ya tiene uno configurado, de lo contrario lo crea desde cero
                    if (existHectarea.Rows.Count > 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            UPDATE dbo.M_valor_atributo
                            SET id_valor_discreto = @id_valor_discreto
                            WHERE id = @id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id", int.Parse(existHectarea.Rows[0]["id"].ToString()));
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorHectareaInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    else
                    {
                        //padre
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_atributo)
                            VALUES
                            (@finca, 7) --ATRIBUTO PADRE HECTAREA

                            SELECT @@IDENTITY AS id_hectarea_padre

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable tabla_id_hectarea_padre = dt.Tables[0];
                        int id_hectarea_padre = int.Parse(tabla_id_hectarea_padre.Rows[0]["id_hectarea_padre"].ToString());
                        //hijo
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_registro, id_atributo, id_valor_discreto)
                            VALUES
                            (@finca, @id_registro_padre, 8, @id_valor_discreto) --ATRIBUTO HECTAREA

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_hectarea_padre);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorHectareaInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }

                    // --------------------------------------------- Organica
                    if (idValorOrganica.Rows.Count > 0)
                    {
                        idValorOrganicaInt = int.Parse(idValorOrganica.Rows[0]["id"].ToString());
                    }
                    else
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo_discreto
                            (id_atributo, valor)
                            VALUES
                            (10, @organica) --ATRIBUTO ORGANICA

                            SELECT @@IDENTITY AS id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@organica", organica);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable idAtributoInsert = dt.Tables[0];
                        idValorOrganicaInt = int.Parse(idAtributoInsert.Rows[0]["id"].ToString());
                    }

                    //Se busca si la finca ya tiene definido el atributo
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT id
                        FROM dbo.M_valor_atributo
                        WHERE id_recurso = @id
                        AND id_atributo = 10 --ATRIBUTO ORGANICA HIJO

                    "), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id", id);
                    dt = new DataSet();
                    adapter.Fill(dt);
                    DataTable existOrganica = dt.Tables[0];

                    //Se actualiza el valor del atributo si ya tiene uno configurado, de lo contrario lo crea desde cero
                    if (existOrganica.Rows.Count > 0)
                    {
                        adapter = new SqlDataAdapter(String.Format(@"

                            UPDATE dbo.M_valor_atributo
                            SET id_valor_discreto = @id_valor_discreto
                            WHERE id = @id

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@id", int.Parse(existOrganica.Rows[0]["id"].ToString()));
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorOrganicaInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }
                    else
                    {
                        //padre
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_atributo)
                            VALUES
                            (@finca, 9) --ATRIBUTO PADRE ORGANICA

                            SELECT @@IDENTITY AS id_organica_padre

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        dt = new DataSet();
                        adapter.Fill(dt);
                        DataTable tabla_id_organica_padre = dt.Tables[0];
                        int id_organica_padre = int.Parse(tabla_id_organica_padre.Rows[0]["id_organica_padre"].ToString());
                        //hijo
                        adapter = new SqlDataAdapter(String.Format(@"

                            INSERT INTO dbo.M_valor_atributo
                            (id_recurso, id_registro, id_atributo, id_valor_discreto)
                            VALUES
                            (@finca, @id_registro_padre, 10, @id_valor_discreto) --ATRIBUTO ORGANICA

                        "), conexion.getConexion());
                        adapter.SelectCommand.Parameters.AddWithValue("@finca", id);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_registro_padre", id_organica_padre);
                        adapter.SelectCommand.Parameters.AddWithValue("@id_valor_discreto", idValorOrganicaInt);
                        adapter.SelectCommand.ExecuteScalar();
                    }
                }
                else
                {
                    error["CODIGO"] = existCodigo.Rows.Count;
                    error["NOMBRE"] = existNombre.Rows.Count;
                    result["RESULTADO"] = error;
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
                Mail.SendEmail(e, host, string.Format(@"id={0}, codigo={1}, nombre={2}, municipio={3}, sector={4}, grupo={5}, estado={6}, latitud={7}, longitud={8}, hectareas={9}, organica={10}", id, codigo, nombre, municipio, sector, grupo, estado, latitud, longitud, hectareas, organica));
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
