using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Drawing;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using System.Web.Services;
using System.IO;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System.Threading;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;
using System.Security.Cryptography;
using System.Text;

public class Estado_Usuario : System.Web.Services.WebService
{
    private string id { get; set; }
    private string schema { get; set; }
    private string view { get; set; }
    private string nombre { get; set; }
    private string consulta { get; set; }
    private string replace { get; set; }
    private ConexionSQL conexion = new ConexionSQL();

    public string qColumnsName(string nombre)
    {
        this.nombre = nombre;
        this.qConfigData();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if (this.view != "")
                {
                    string query;
                    query = String.Format(@"
                        -- Por el momento solo se necesita el nombre de las columnas

                        SELECT
                            COLUMN_NAME AS data, --se envia de esta forma para construccion automatica en dataTable
                            DATA_TYPE AS type
                        FROM
                            INFORMATION_SCHEMA.COLUMNS
                        WHERE
                            TABLE_SCHEMA = '{0}'     --nombre schema
                            AND TABLE_NAME = '{1}'   --nombre de la vista

                    ", this.schema, this.view);

                    conexion.closeConexion();
                    return query;
                }
                else
                {
                    //SE EXTRAEN LAS COLUMNAS Y EL TYPE DE LA TABLA dataTableColumnsName
                    string query;
                    query = String.Format(@"
                        -- Por el momento solo se necesita el nombre de las columnas

                        SELECT data, type
                        FROM {0}.dataTableColumnsName
                        WHERE id_dataTableConfig = {1}
                        ORDER BY orden ASC

                    ", this.schema, this.id);

                    conexion.closeConexion();
                    return query;
                }
            }
            catch (Exception e)
            {
                conexion.closeConexion();
                return "ERROR";
            }
        }
        else
        {
            conexion.closeConexion();
            return "Error en la conexion:";
        }
    }

    public string queryBuilder(string nombre)
    {
        this.nombre = nombre;
        this.qConfigData();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE EXTRAEN LAS COLUMNAS Y EL TYPE DE LA TABLA dataTableColumnsName
                adapter = new SqlDataAdapter(string.Format(@"
                    SELECT data, type, longitud
                    FROM {0}.dataTableColumnsName
                    WHERE id_dataTableConfig = {1}
                    ORDER BY orden ASC
                ", this.schema, this.id), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                string data, type, longitud, tempTable = @"
                    CREATE TABLE #consulta (";
                for (int i = 0; i < table.Rows.Count; i++)
                {
                    data = table.Rows[i]["data"].ToString();
                    type = table.Rows[i]["type"].ToString();
                    longitud = table.Rows[i]["longitud"].ToString();
                    if (i == table.Rows.Count - 1)
                    {
                        //ultima fila
                        if (longitud != "")
                        {
                            //longitud 
                            tempTable = string.Concat(tempTable, @"
                            [", data, @"] ", type, @" (", longitud, @")");
                        }
                        else
                        {
                            //longitud ausente
                            tempTable = string.Concat(tempTable, @"
                            [", data, @"] ", type);
                        }
                    }
                    else
                    {
                        //filas normales
                        if (longitud != "")
                        {
                            tempTable = string.Concat(tempTable, @"
                            [", data, @"] ", type, @" (", longitud, @"),");
                        }
                        else
                        {
                            tempTable = string.Concat(tempTable, @"
                            [", data, @"] ", type, @",");
                        }
                    }
                }
                tempTable = string.Concat(tempTable, ");");

                conexion.closeConexion();
                return tempTable;
            }
            catch (Exception e)
            {
                conexion.closeConexion();
                return "ERROR";
            }
        }
        else
        {
            conexion.closeConexion();
            return "Error en la conexion:";
        }
    }

    public string qColumnsFilter(string col_name, string cols, string col, string nombre, string replace)
    {
        this.nombre = nombre;
        this.replace = replace;
        string querySelect = "";
        string exp = "**/";
        this.qConfigData();
        /*------------------------*/
        string queryColumns = qColumnsName(nombre);
        SqlDataAdapter adapter = new SqlDataAdapter();
        //SE EXTRAEN LAS COLUMNAS Y EL TYPE
        adapter = new SqlDataAdapter(string.Format(queryColumns), conexion.getConexion());
        DataSet dt = new DataSet();
        adapter.Fill(dt);
        DataTable columnsTable = dt.Tables[0];
        /*------------------------*/

        /* Creación consulta base*/
        if (this.view != "")
        {
            querySelect = string.Concat(@"
                                    IF (
	                                    SELECT
		                                    COUNT (X.[{0}]) AS VALOR
	                                    FROM
		                                    (
			                                    SELECT
		                                            [{0}]
	                                            FROM "
                                                   , this.schema, ".", this.view, @"
                                                  {1} --WHERE
                                                  GROUP BY[{0}]
		                                    ) AS X
                                    ) < 5000
                                    BEGIN
	                                    SELECT
		                                    [{0}]
	                                    FROM "
                                           , this.schema, ".", this.view, @"
                                          {1} --WHERE
                                          GROUP BY[{0}]
                                          ORDER BY[{0}] ASC
                                        END
                                    ELSE
                                    BEGIN
	                                    SELECT
		                                    ('DENIED') AS [{0}]
                                    END");
        }
        else
        {
            //Se arma la tabla temporal
            string tempTable = queryBuilder(this.nombre);
            querySelect = string.Concat(tempTable, this.consulta,
                @"
                                    IF (
	                                    SELECT
		                                    COUNT (X.[{0}]) AS VALOR
	                                    FROM
		                                    (
			                                    SELECT
		                                            [{0}]
	                                            FROM 
                                                    #consulta
                                                  {1} --WHERE
                                                  GROUP BY[{0}]
		                                    ) AS X
                                    ) < 5000
                                    BEGIN
	                                    SELECT
		                                    [{0}]
	                                    FROM 
                                            #consulta
                                          {1} --WHERE
                                          GROUP BY[{0}]
                                          ORDER BY[{0}] ASC
                                        END
                                    ELSE
                                    BEGIN
	                                    SELECT
		                                    ('DENIED') AS [{0}]
                                    END");
        }
        /*-----------------------*/
        /* Creación filtros, puede ser una función compartida (sobrecargada) con el método principal */
        string queryWhere = "";
        /*-----------------------*/

        /*----------- Construcción Consulta -----------*/
        JArray cols_name = JArray.Parse(col_name);
        string queryFilter = "";
        List<string> singleQuery = new List<string>(new string[cols_name.Count()]);
        /* filtros de consulta */
        if (cols != "")
        {
            Column[] columns = JsonConvert.DeserializeObject<Column[]>(cols);
            //JArray columns = JArray.Parse(cols);
            int colsSearch = columns.Where(c => c.search.value != "").Count();
            if (colsSearch > 0)
            {
                queryWhere = "WHERE ";
                for (int i = 0; i < cols_name.Count(); i++)
                {
                    //string[] searchValue = columns[i].search.value.Split('\\,');
                    string[] searchValue = Regex.Split(columns[i].search.value, "#,");
                    if (searchValue.Where(sV => sV != "").Count() != 0)
                    {
                        if (searchValue.Where(sV => sV.Contains(exp)).Count() > 0)
                        {
                            if (searchValue.Where(sV => sV != "" && !sV.Contains(exp)).Count() > 0)
                            {
                                singleQuery[i] = string.Concat("[", columns[i].data, "]", " IN ('", string.Join("','", searchValue.Where(sV => sV != "" && !sV.Contains(exp))), "') AND ");
                            }
                            string[] searchCustom = searchValue.Where(sV => sV.Contains(exp)).ToArray();
                            searchCustom = searchCustom.Select(sC => sC.Replace("**/", " ")).ToArray();
                            //COLLATE SE USA PARA QUYE LOS ACENTOS O TILDES NO AFECTEN (NO FUNCIONA CON FECHAS)
                            //SOLO SI ES CHAR, VARCHAR, NVARCHAR O TEXT USE COLLAT
                            string type = columnsTable.Select("data =" + @"'" + columns[i].data + @"'")[0][1].ToString();
                            if ((type == "char") || (type == "varchar") || (type == "nvarchar") || (type == "text") || (type == "nchar") || (type == "ntext"))
                            {
                                searchCustom = searchCustom.Select(sC => sC.Replace("data", "[" + columns[i].data + "] COLLATE SQL_Latin1_General_Cp1_CI_AI")).ToArray();
                            }
                            else
                            {
                                searchCustom = searchCustom.Select(sC => sC.Replace("data", "[" + columns[i].data + "]")).ToArray();
                            }
                            singleQuery[i] = string.Concat(singleQuery[i], "(", string.Join("", searchCustom), ")");
                        }
                        else
                        {
                            singleQuery[i] = string.Concat("[", columns[i].data, "]", " IN ('", string.Join("','", searchValue.Where(sV => sV != "")), "')");
                        }
                    }

                }

            }
        }

        /*Construir consulta total, a partir de cada consulta creada anteriormente en la lista SingleQuery, esta lista puede se una arreglo de strings en otro lenguaje*/
        //foreach (var name in cols_name)
        //{

        string whereText = string.Join(" AND ",
                                    singleQuery.Where(s => s != null && !s.Contains(string.Concat("[", col, "]")))
                            );

        if (whereText == "")
        {
            queryWhere = "";
            queryFilter = string.Concat(queryFilter, "\n",
                            String.Format(querySelect, col, queryWhere));

        }
        else
        {
            queryWhere = string.Concat("WHERE ", whereText);
            queryFilter = string.Concat(queryFilter, "\n",
                            String.Format(querySelect, col, queryWhere));
        }


        //}

        /*-------------------------------------------------*/
        return queryFilter;
    }

    private void qConfigData()
    {
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                        SELECT
                            id,
                            esquema,
                            vista,
                            consulta
                        FROM
                            dbo.dataTableConfig
                        WHERE
                            nombre = @nombre;
                ", this.nombre), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@nombre", this.nombre);

                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                this.id = table.Rows[0]["id"].ToString();
                this.schema = table.Rows[0]["esquema"].ToString();
                this.view = table.Rows[0]["vista"].ToString();
                this.consulta = table.Rows[0]["consulta"].ToString();

                //Se consulta si hay valores que deben ser replazados en consulta
                adapter = new SqlDataAdapter(String.Format(@"
                    SELECT replace
                    FROM {0}.dataTableReplace
                    WHERE id_dataTableConfig = {1}
                    ORDER BY orden ASC
                ", schema, this.id), conexion.getConexion());

                dt = new DataSet();
                adapter.Fill(dt);
                DataTable table1 = dt.Tables[0];

                if (table1.Rows.Count > 0 && this.replace != null)
                {
                    JObject replaceObj = JObject.Parse(this.replace);
                    string replace, key;
                    for (int i = 0; i < table1.Rows.Count; i++)
                    {
                        key = @"{" + i.ToString() + @"}";
                        replace = table1.Rows[i]["replace"].ToString();
                        replace = replaceObj[replace].ToString();
                        this.consulta = this.consulta.Replace(key, replace);
                    }
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }

    public string qColumnsDataReg(int draw, int start, int length, int n_col, string nombre, string replace)
    {
        this.nombre = nombre;
        this.replace = replace;
        this.qConfigData();
        /* Organizar estas lineas en una función y devolver un objeto con estas características */
        /* la función recibe un NameValueCollection, organiza los datos de este formulario y los pasa */
        /* a otra función que construye las consultas */
        /*---------------------------------------------------------------------------------------*/
        /*------------------------*/
        string queryColumns = qColumnsName(nombre);
        SqlDataAdapter adapter = new SqlDataAdapter();
        //SE EXTRAEN LAS COLUMNAS Y EL TYPE
        adapter = new SqlDataAdapter(string.Format(queryColumns), conexion.getConexion());
        DataSet dt = new DataSet();
        adapter.Fill(dt);
        DataTable columnsTable = dt.Tables[0];
        /*------------------------*/

        List<Order> orders = new List<Order>();
        Search search = new Search();
        List<Column> columns = new List<Column>(n_col);

        //Para recuperar datos que envia el ajax de datatable por una petición POST
        NameValueCollection nvc = HttpContext.Current.Request.Form;
        var dictAjaxDataTables = nvc.AllKeys.ToDictionary(k => k, k => nvc.GetValues(k));

        //Como estas variables ya se obtiene a través de los parametros se quitan
        dictAjaxDataTables.Remove("draw");
        dictAjaxDataTables.Remove("start");
        dictAjaxDataTables.Remove("length");
        dictAjaxDataTables.Remove("n_col");
        dictAjaxDataTables.Remove("nombre");
        dictAjaxDataTables.Remove("replace");
        dictAjaxDataTables.Remove("id_rol");
        dictAjaxDataTables.Remove("id_estado");

        //Obtener cada campo, Columns, Order, Search y guardarlo en un Jobject
        search.value = dictAjaxDataTables["search[value]"][0];
        search.regex = dictAjaxDataTables["search[regex]"][0];
        dictAjaxDataTables.Remove("search[value]");
        dictAjaxDataTables.Remove("search[regex]");

        //Guardar cada columna en una lista
        for (int j = 0; j < n_col * 6; j += 6)
        {
            Column column = new Column();
            column.data = dictAjaxDataTables.Values.ElementAt(j)[0];
            column.name = dictAjaxDataTables.Values.ElementAt(j + 1)[0];
            column.searchable = Convert.ToBoolean(dictAjaxDataTables.Values.ElementAt(j + 2)[0]);
            column.orderable = Convert.ToBoolean(dictAjaxDataTables.Values.ElementAt(j + 3)[0]);
            column.search = new Search(dictAjaxDataTables.Values.ElementAt(j + 4)[0],
                dictAjaxDataTables.Values.ElementAt(j + 5)[0]);

            columns.Add(column);
        }

        for (int i = n_col * 6; i < dictAjaxDataTables.Count(); i += 2)
        {
            Order order = new Order();
            order.column = int.Parse(dictAjaxDataTables.Values.ElementAt(i)[0]);
            order.dir = dictAjaxDataTables.Values.ElementAt(i + 1)[0];

            orders.Add(order);
        }
        /* Llamado a funcion que organiza las consultas */
        /* recibe un objeto que contiene los demas objetos */
        /*-----------------------------------------------------------------------------------------*/
        /* Funcion para construir consultas */
        /* Variables */
        string queryOrder = "";
        string[] colsName = { };
        string queryCols = "";
        string queryWhere = "";
        string exp = "**/";

        /* Columnas de consulta */
        queryCols = string.Concat("[", string.Join("], [", columns.Where(c => c.data != "").Select(c => c.data)), "]");

        /* Ordenar por columna */
        foreach (Order order in orders)
        {
            if (columns[order.column].data == "")
            {
                queryOrder = String.Format(@"[{0}] {1}", columns[(order.column) + 1].data, order.dir.ToUpper());
            }
            else
            {
                queryOrder = String.Format(@"[{0}] {1}", columns[order.column].data, order.dir.ToUpper());
            }
        }//Se puede construir algo mejor?

        /* Busqueda, se puede usar una funcion generica que reciba los parametros de los WHERE */
        /* La busqueda llega en el atributo search.value de la columna la que se le esta haciendo la busqueda*/

        IEnumerable<Column> colsSearch = columns.Where(c => c.search.value != "");
        if (colsSearch.Count() > 0)
        {
            queryWhere = "WHERE ";
            List<string> singleQuery = new List<string>();
            foreach (Column column in colsSearch)
            {
                //string[] searchValue = column.search.value.Split('\\,');
                string[] searchValue = Regex.Split(column.search.value, "#,");
                if (searchValue.Where(sV => sV.Contains(exp)).Count() > 0)
                {
                    if (searchValue.Where(sV => !sV.Contains(exp)).Count() > 0)
                    {
                        singleQuery.Add(string.Concat("[", column.data, "]", " IN ('", string.Join("','", searchValue.Where(sV => !sV.Contains(exp))), "')"));
                    }
                    string[] searchCustom = searchValue.Where(sV => sV.Contains(exp)).ToArray();
                    searchCustom = searchCustom.Select(sC => sC.Replace("**/", " ")).ToArray();
                    //COLLATE SE USA PARA QUYE LOS ACENTOS O TILDES NO AFECTEN (NO FUNCIONA CON FECHAS)
                    //SOLO SI ES CHAR, VARCHAR, NVARCHAR O TEXT USE COLLAT
                    string type = columnsTable.Select("data =" + @"'" + column.data + @"'")[0][1].ToString();
                    if ((type == "char") || (type == "varchar") || (type == "nvarchar") || (type == "text") || (type == "nchar") || (type == "ntext"))
                    {
                        searchCustom = searchCustom.Select(sC => sC.Replace("data", "[" + column.data + "] COLLATE SQL_Latin1_General_Cp1_CI_AI")).ToArray();
                    }
                    else
                    {
                        searchCustom = searchCustom.Select(sC => sC.Replace("data", "[" + column.data + "]")).ToArray();
                    }
                    singleQuery.Add(string.Concat("(", string.Join("", searchCustom), ")"));
                }
                else
                {
                    singleQuery.Add(string.Concat("[", column.data, "]", " IN ('", string.Join("','", searchValue), "')"));
                }
            }
            queryWhere = string.Concat(queryWhere, string.Join(" AND ", singleQuery));
        }
        /*------------------------------------------------------------------------------------------*/
        if (this.view != "")
        {
            string query = String.Format(@"
                        DECLARE
	                        @length INT,
	                        @recordsTotal INT,
	                        @recordsFiltered INT,
	                        @start INT

                        SET @start = @inicio
                        SET @length = @tam

                        BEGIN
	                    WITH Data AS					--Este ORDER BY es dinamico
		                    (SELECT ROW_NUMBER () OVER (ORDER BY {0}) AS Row,
						                    --Realizar consulta campos, esta linea es dinámica, construir en C#
						                    DT_RowId, {1}
		                    FROM
			                    [{2}].[{3}] --db.schema.view
                            {4} --WHERE
                            )
							-- estos nombres tambien deben ser dinámicos
		                    SELECT DT_RowId, {1} --Linea dinámica
		                    FROM 
                                Data
                        END
            ", queryOrder, queryCols, this.schema, this.view, queryWhere);

            return query;
        }
        else
        {
            //Se arma la tabla temporal
            string tempTable = queryBuilder(this.nombre);
            string query = String.Format(@"
                        --Tabla temporal
                        {4}

                        --Consulta creada por el usuario
                        {2}

                        DECLARE
	                        @length INT,
	                        @recordsTotal INT,
	                        @recordsFiltered INT,
	                        @start INT

                        SET @start = @inicio
                        SET @length = @tam

                        BEGIN

	                        WITH Data AS					--Este ORDER BY es dinamico
		                        (SELECT ROW_NUMBER () OVER (ORDER BY {0}) AS Row,
						                        --Realizar consulta campos, esta linea es dinámica, construir en C#
						                        DT_RowId, {1}
		                        FROM
			                        #consulta --consulta
                                {3} --WHERE
                                )
								-- estos nombres tambien deben ser dinámicos
		                        SELECT DT_RowId, {1} --Linea dinámica
		                        FROM 
                                    Data
                        END
                        DROP TABLE #consulta;

                ", queryOrder, queryCols, this.consulta, queryWhere, tempTable);

            return query;
        }
    }

    /**********************************************************************************/
    /**********************************************************************************/
    /**********************************************************************************/

    [WebMethod(EnableSession = true)]
    public string Rol_Estado(int id_EU)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                ClaseCifrado cifrado = new ClaseCifrado();

                //Se consultan las unidades de medida
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;
                    SELECT 
                        r.id,
                        r.nombre 
                    FROM 
                        {0}.rol r;

                    SELECT 
                        eo.id, 
                        eo.estado 
                    FROM 
                        {0}.estado_opcion eo;

                    SELECT
	                    eu.id_estado_opcion, 
                        eu.id_rol
                    FROM 
                        {0}.estado_usuario eu
                    WHERE 
                        eu.id = @id_EU

                    SELECT
	                    ISNULL(U.app_habilitada, 0) AS app_habilitada,
                        U.contrasena_app
                    FROM dbo.estado_usuario AS EU
                    INNER JOIN dbo.usuario AS U
                    ON EU.id_usuario = U.id
                    WHERE EU.id = @id_EU

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_EU", id_EU);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Roles = dt.Tables[0];
                DataTable Estado = dt.Tables[1];
                DataTable E_R = dt.Tables[2];
                DataTable app_habilitada = dt.Tables[3];

                if (app_habilitada.Rows.Count != 0)
                {
                    try
                    {
                        app_habilitada.Rows[0]["contrasena_app"] = cifrado.descifrar(app_habilitada.Rows[0]["contrasena_app"].ToString());
                    }
                    catch
                    {
                        app_habilitada.Rows[0]["contrasena_app"] = null;
                    }
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["Roles"] = JArray.Parse(JsonConvert.SerializeObject(Roles, Formatting.None));
                result["RESULTADO"]["Estado"] = JArray.Parse(JsonConvert.SerializeObject(Estado, Formatting.None));
                result["RESULTADO"]["E_R"] = JArray.Parse(JsonConvert.SerializeObject(E_R, Formatting.None));
                result["RESULTADO"]["app_habilitada"] = JArray.Parse(JsonConvert.SerializeObject(app_habilitada, Formatting.None));

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_EU: {0}", id_EU));
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

    [WebMethod(EnableSession = true)]
    public string Rol_Estado_msv()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se consultan las unidades de medida
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;
                    SELECT 
                        r.id,
                        r.nombre 
                    FROM 
                        {0}.rol r;

                    SELECT 
                        eo.id, 
                        eo.estado 
                    FROM 
                        {0}.estado_opcion eo;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Roles = dt.Tables[0];
                DataTable Estado = dt.Tables[1];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["Roles"] = JArray.Parse(JsonConvert.SerializeObject(Roles, Formatting.None));
                result["RESULTADO"]["Estado"] = JArray.Parse(JsonConvert.SerializeObject(Estado, Formatting.None));

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

    [WebMethod(EnableSession = true)]
    public string upd_EU(int id_EU, int id_rol, int id_estado, int app_habilitada, string contrasena_app)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se encripta la contraseña si es que viene
                if(contrasena_app != "")
                {                    
                    ClaseCifrado cifrado = new ClaseCifrado();
                    contrasena_app = cifrado.cifrar(contrasena_app);
                }

                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    DECLARE @U_ INT
                    SET @U_ = (SELECT U.id FROM {0}.usuario U WHERE U.usuario = @user_admin)

                    INSERT INTO {0}.T_cambio_estado_usuario (
	                    id_usuario_admin, 
	                    id_usuario, 
	                    id_estado_anterior,
	                    id_estado_actual,
	                    id_rol_anterior,
	                    id_rol_actual,
	                    fecha_cambio
                    )
                    SELECT
	                    @U_,
	                    EU.id_usuario,
	                    EU.id_estado_opcion,
	                    @id_estado,
	                    EU.id_rol,
	                    @id_rol,
	                    GETDATE()
                    FROM
	                    {0}.estado_usuario EU
                    WHERE 
                        EU.id = @id_EU;
                    
                    UPDATE [{0}].[estado_usuario] 
                    SET 
                        [id_estado_opcion] = @id_estado,
                        [id_rol] = @id_rol
                    WHERE 
                        id = @id_EU;

                    UPDATE U
                    SET
	                    app_habilitada = @app_habilitada
                    FROM dbo.usuario AS U
                    INNER JOIN dbo.estado_usuario AS EU
                    ON U.id = EU.id_usuario
                    WHERE  EU.id = @id_EU;

                    IF @contrasena_app <> ''   
                        BEGIN
                            UPDATE U
                            SET
	                            contrasena_app = @contrasena_app
                            FROM dbo.usuario AS U
                            INNER JOIN dbo.estado_usuario AS EU
                            ON U.id = EU.id_usuario
                            WHERE  EU.id = @id_EU;
                        END

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@user_admin", Session["User"]);
                adapter.SelectCommand.Parameters.AddWithValue("@id_EU", id_EU);
                adapter.SelectCommand.Parameters.AddWithValue("@id_rol", id_rol);
                adapter.SelectCommand.Parameters.AddWithValue("@id_estado", id_estado);
                adapter.SelectCommand.Parameters.AddWithValue("@app_habilitada", app_habilitada);
                adapter.SelectCommand.Parameters.AddWithValue("@contrasena_app", contrasena_app);
                adapter.SelectCommand.ExecuteScalar();

                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;
                    SELECT
	                    u.nombre_usuario AS nombre,
	                    u.correo AS email,
	                    est_opc.estado AS estado,
                        ROL.nombre AS rol
                    FROM
	                    {0}.usuario u
                    INNER JOIN {0}.estado_usuario est_u ON est_u.id_usuario = u.id
                    INNER JOIN {0}.estado_opcion est_opc ON est_u.id_estado_opcion = est_opc.id
                    INNER JOIN {0}.rol ROL ON est_u.id_rol = ROL.id
                    WHERE
	                    est_u.id = @id_EU
                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_EU", id_EU);
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                string nombre =     dt.Tables[0].Rows[0]["nombre"].ToString();
                string email =      dt.Tables[0].Rows[0]["email"].ToString();
                string estado =     dt.Tables[0].Rows[0]["estado"].ToString();
                string rol =        dt.Tables[0].Rows[0]["rol"].ToString();

                if (email != "")
                {
                    Mail.SendEmailState(nombre, email, estado, rol);
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = "";

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_EU: {0}, id_rol: {1}, id_estado: {2}", id_EU, id_rol, id_estado));
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

    /**********************************************************************************/

    [WebMethod(EnableSession = true)]
    public string excel_info()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                
                Begin Transaction;

                INSERT INTO {0}.D_Registro_Excel (Fecha, Nombre, Estado, Fallo)
                VALUES
                (GETDATE(), 'excelDataTables', 1, 0);

                SELECT @@IDENTITY AS id

                Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                int id_table = int.Parse(table.Rows[0][0].ToString());
                string host = HttpContext.Current.Request.Url.Host;

                Thread hilo1 = new Thread(() => this.dataExcel(id_table, host));
                hilo1.Start();

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
            result["MENSAJE"] = "ERROR";
            conexion.closeConexion();
        }
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    public void dataExcel(int id_table, string host)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                Begin Transaction;

                SET DATEFORMAT ymd

                DECLARE @F_ NVARCHAR(10)
                SET @F_ = CONVERT(NVARCHAR(10), GETDATE(), 120)

                SELECT
	                U.correo AS [Email],	                
	                U.nombres AS [Nombre],
	                U.apellidos AS [Apellido],
	                U.nombre_usuario AS [Nombre Completo],
	                CAST(MAX(TUS.fecha_sesion) AS NVARCHAR(10)) AS [Fecha último ingreso]
                FROM
	                {0}.usuario U
                INNER JOIN {0}.T_usuario_suite TUS ON TUS.id_usuario = U.id
                INNER JOIN {0}.estado_usuario EU ON EU.id_usuario = U.id

                WHERE
	                EU.id_estado_opcion = 1

                GROUP BY
	                U.correo,
	                U.nombres,
	                U.apellidos,
	                U.nombre_usuario

                HAVING
	                MAX(TUS.fecha_sesion) < DATEADD(MM, -3, @F_)

                Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable info = dt.Tables[0];

                //Se pone la ruta base sobre donde se guardara el excel 
                string pathBase = Server.MapPath("~/Registro_InfoUser/");
                string NombreArchivo = string.Format("Reporte_usuarios_{0}.xlsx", DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss"));

                FileInfo destFile = new FileInfo(pathBase + NombreArchivo);

                GestionReportes.ClearFolder(pathBase, 1);

                using (ExcelPackage registros = new ExcelPackage())
                {
                    //modificar cuando sea otra empresa
                    registros.Workbook.Properties.Title = "Central Cervecera de Colombia S.A.S.";
                    registros.Workbook.Properties.Author = "OptiPlant Consultores";
                    registros.Workbook.Properties.Comments = "Registro de inicio de sesión de usuarios";
                    registros.Workbook.Properties.Company = "OptiPlant Consultores";
                    registros.Workbook.Properties.AppVersion = "1.0";
                    registros.Workbook.Properties.LastModifiedBy = "OptiPlant Consultores";
                    registros.Workbook.Properties.Modified = DateTime.Now;

                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    //Se agregra la hoja de inversión al libro de registros
                    ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    /************************************************************************************************************************************/

                    //Se crea la hoja de usuarios inactivos
                    ExcelWorksheet Usuarios = registros.Workbook.Worksheets.Add("Usuarios");

                    Usuarios.View.ShowGridLines = false;
                    Usuarios.Cells.Style.Font.SetFromFont(new Font("Arial", 9, FontStyle.Regular));

                    // Se define el aalto de todas las filas y columnas
                    Usuarios.DefaultRowHeight = 15;
                    Usuarios.Row(1).Height = 45;

                    for (int i = 0; i < info.Columns.Count; i++)
                    {
                        Usuarios.Cells[1, (i + 1)].Value = info.Columns[i].ToString();
                    }

                    //Se define el estilo, tipo y tamaño de la letra, se centra y se le agrega un fondo al encabezado
                    Usuarios.Cells[1, 1, 1, info.Columns.Count].Style.Font.SetFromFont(new Font("Arial", 9, FontStyle.Bold));
                    Usuarios.Cells[1, 1, 1, info.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    Usuarios.Cells[1, 1, 1, info.Columns.Count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    Usuarios.Cells[1, 1, 1, info.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    Usuarios.Cells[1, 1, 1, info.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                    Usuarios.Cells[1, 1, 1, info.Columns.Count].Style.WrapText = true;

                    /************************************************************************************************************************************/

                    if (info.Rows.Count > 0)
                    {
                        Usuarios.Cells[1, 1, (info.Rows.Count + 1), info.Columns.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        Usuarios.Cells[1, 1, (info.Rows.Count + 1), info.Columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        Usuarios.Cells[1, 1, (info.Rows.Count + 1), info.Columns.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        Usuarios.Cells[1, 1, (info.Rows.Count + 1), info.Columns.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        Usuarios.Cells[2, 1, (info.Rows.Count + 1), info.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        Usuarios.Cells[2, 1, (info.Rows.Count + 1), info.Columns.Count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        //Formato de fecha
                        Usuarios.Cells[2, 6, (info.Rows.Count + 1), 8].Style.Numberformat.Format = "dd/MM/yyyy";

                        for (int j = 0; j < info.Rows.Count; j++)
                        {
                            for (int i = 0; i < info.Columns.Count; i++)
                            {
                                if (i == 5)
                                {
                                    Usuarios.Cells[(j + 2), (i + 1)].Value = DateTime.Parse(info.Rows[j][i].ToString());
                                }
                                else
                                {
                                    Usuarios.Cells[(j + 2), (i + 1)].Value = info.Rows[j][i];
                                }
                            }
                            if (j % 2 != 0)
                            {
                                Usuarios.Cells[(j + 2), 1, (j + 2), info.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                Usuarios.Cells[(j + 2), 1, (j + 2), info.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                            }
                        }
                    }

                    /************************************************************************************************************************************/

                    Usuarios.Cells[1, 1, 1, info.Columns.Count].AutoFilter = true;
                    Usuarios.View.FreezePanes(2, 1);
                    Usuarios.Cells.AutoFitColumns(13);
                    Usuarios.View.ZoomScale = 100;

                    registros.SaveAs(destFile);

                    adapter = new SqlDataAdapter(String.Format(@"
                        
                    Begin Transaction;
                    
                    UPDATE {0}.D_Registro_Excel
                    SET Nombre = '{1}', Estado = 0
                    WHERE
                        id = {2}

                    Commit Transaction;
                    ", Abi_maestro.esquema, NombreArchivo, id_table), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();
                    conexion.closeConexion();
                }
            }
            catch (Exception e)
            {
                adapter = new SqlDataAdapter(String.Format(@"
                Begin Transaction;

                UPDATE {0}.D_Registro_Excel
                SET Fallo = 1
                WHERE
                    id = {1}
                Commit Transaction;
                ", Abi_maestro.esquema, id_table), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();
                Mail.SendEmail(e, host, "");
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }

    /**********************************************************************************/

    [WebMethod(EnableSession = true)]
    public string updateStateUser()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                Begin Transaction;

                SET DATEFORMAT ymd

                DECLARE @F_ NVARCHAR(10)
                SET @F_ = CONVERT(NVARCHAR(10), GETDATE(), 120)

                DECLARE @U_ INT
                SET @U_ = (SELECT U.id FROM {0}.usuario U WHERE U.usuario = @user_admin)

                INSERT INTO {0}.T_cambio_estado_usuario (
	                id_usuario_admin, 
	                id_usuario, 
	                id_estado_anterior,
	                id_estado_actual,
	                id_rol_anterior,
	                id_rol_actual,
	                fecha_cambio
                )
                SELECT
	                @U_,
	                U.id,
	                1,
	                3,
	                EU.id_rol,
	                EU.id_rol,
	                GETDATE()
                FROM
	                {0}.usuario U
                INNER JOIN {0}.T_usuario_suite TUS ON TUS.id_usuario = U.id
                INNER JOIN {0}.estado_usuario EU ON EU.id_usuario = U.id

                WHERE
	                EU.id_estado_opcion = 1

                GROUP BY
	                U.id,
	                EU.id_rol

                HAVING
	                MAX(TUS.fecha_sesion) < DATEADD(MM, -3, @F_);

                UPDATE EST
                SET EST.id_estado_opcion = 3
                FROM
	                {0}.estado_usuario EST
                INNER JOIN {0}.usuario U ON EST.id_usuario = U.id
                WHERE
	                U.id IN (
                    SELECT
	                    U.id
                    FROM
	                    {0}.usuario U
                    INNER JOIN {0}.T_usuario_suite TUS ON TUS.id_usuario = U.id
                    INNER JOIN {0}.estado_usuario EU ON EU.id_usuario = U.id

                    WHERE
	                    EU.id_estado_opcion = 1

                    GROUP BY
	                    U.id

                    HAVING
                        MAX(TUS.fecha_sesion) < DATEADD(MM, -3, @F_)
                );

                Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@user_admin", Session["User"]);
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

    /**********************************************************************************/

    [WebMethod(EnableSession = true)]
    public string upd_EU_msv()
    {
        int id_rol = int.Parse(HttpContext.Current.Request.Form.Get("id_rol"));
        int id_estado = int.Parse(HttpContext.Current.Request.Form.Get("id_estado"));
        int draw = int.Parse(HttpContext.Current.Request.Form.Get("draw"));
        int start = int.Parse(HttpContext.Current.Request.Form.Get("start"));
        int length = int.Parse(HttpContext.Current.Request.Form.Get("length"));
        int n_col = int.Parse(HttpContext.Current.Request.Form.Get("n_col"));
        string nombre = HttpContext.Current.Request.Form.Get("nombre");
        string replace = HttpContext.Current.Request.Form.Get("replace");

        string queryData = this.qColumnsDataReg(draw, start, length, n_col, nombre, replace);

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(queryData, conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@inicio", start);
                adapter.SelectCommand.Parameters.AddWithValue("@tam", length);
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Datos = dt.Tables[0];

                int id_EU;

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_EU = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    update_user_msv(id_EU, id_rol, id_estado);
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = "";
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
            result["MENSAJE"] = "ERROR";
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    public void update_user_msv(int id_EU, int id_rol, int id_estado)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    DECLARE @U_ INT
                    SET @U_ = (SELECT U.id FROM {0}.usuario U WHERE U.usuario = @user_admin)

                    INSERT INTO {0}.T_cambio_estado_usuario (
	                    id_usuario_admin, 
	                    id_usuario, 
	                    id_estado_anterior,
	                    id_estado_actual,
	                    id_rol_anterior,
	                    id_rol_actual,
	                    fecha_cambio
                    )
                    SELECT
	                    @U_,
	                    EU.id_usuario,
	                    EU.id_estado_opcion,
	                    @id_estado,
	                    EU.id_rol,
	                    @id_rol,
	                    GETDATE()
                    FROM
	                    {0}.estado_usuario EU
                    WHERE 
                        EU.id = @id_EU;
                    
                    UPDATE [{0}].[estado_usuario] 
                    SET 
                        [id_estado_opcion] = @id_estado,
                        [id_rol] = @id_rol
                    WHERE 
                        id = @id_EU;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@user_admin", Session["User"]);
                adapter.SelectCommand.Parameters.AddWithValue("@id_EU", id_EU);
                adapter.SelectCommand.Parameters.AddWithValue("@id_rol", id_rol);
                adapter.SelectCommand.Parameters.AddWithValue("@id_estado", id_estado);
                adapter.SelectCommand.ExecuteScalar();

                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;
                    SELECT
	                    u.nombre_usuario AS nombre,
	                    u.correo AS email,
	                    est_opc.estado AS estado,
                        ROL.nombre AS rol
                    FROM
	                    {0}.usuario u
                    INNER JOIN {0}.estado_usuario est_u ON est_u.id_usuario = u.id
                    INNER JOIN {0}.estado_opcion est_opc ON est_u.id_estado_opcion = est_opc.id
                    INNER JOIN {0}.rol ROL ON est_u.id_rol = ROL.id
                    WHERE
	                    est_u.id = @id_EU
                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_EU", id_EU);
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                string nombre = dt.Tables[0].Rows[0]["nombre"].ToString();
                string email = dt.Tables[0].Rows[0]["email"].ToString();
                string estado = dt.Tables[0].Rows[0]["estado"].ToString();
                string rol = dt.Tables[0].Rows[0]["rol"].ToString();

                if (email != "")
                {
                    Mail.SendEmailState(nombre, email, estado, rol);
                }

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_EU: {0}, id_rol: {1}, id_estado: {2}", id_EU, id_rol, id_estado));
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }

}


