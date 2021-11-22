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

public class Perfiles : System.Web.Services.WebService
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
        dictAjaxDataTables.Remove("id_S");

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
    public string Perfil_recurso()
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

                SELECT
		                RP.id,
	                RP.nombre
                FROM
	                {0}.M_recurso RP
                INNER JOIN {0}.M_categoria_recurso CAT ON RP.id_M_categoria_recurso = CAT.id
                WHERE
	                CAT.id = 6
                ORDER BY
	                RP.nombre;

                SELECT
	                R.id,
	                R.nombre,
	                R.id AS [id_Plantas]
                FROM
	                {0}.M_recurso R
                WHERE
	                R.id_M_categoria_recurso = 6
                ORDER BY
	                R.nombre;

                SELECT
	                TAB.id,
	                TAB.nombre
                FROM
                (
	                SELECT
		                NA.id,
		                NA.nombre
	                FROM
		                {0}.nivel_acceso NA
	                WHERE
		                id = 3

	                UNION ALL

	                SELECT
		                NA.id,
		                NA.nombre
	                FROM
		                {0}.nivel_acceso NA
	                WHERE
		                id <> 3
                ) TAB;

                SELECT
	                NA.id
                FROM
	                {0}.nivel_acceso NA
                WHERE
	                NA.nombre = 'Sin Acceso'

                Commit Transaction;
                 
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                DataTable Plantas = dt.Tables[0];
                DataTable Recursos = dt.Tables[1];
                DataTable NA = dt.Tables[2];
                DataTable NA_defecto = dt.Tables[3];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["Plantas"] = JArray.Parse(JsonConvert.SerializeObject(Plantas, Formatting.None));
                result["RESULTADO"]["Recursos"] = JArray.Parse(JsonConvert.SerializeObject(Recursos, Formatting.None));
                result["RESULTADO"]["NA"] = JArray.Parse(JsonConvert.SerializeObject(NA, Formatting.None));
                result["RESULTADO"]["NA_defecto"] = JArray.Parse(JsonConvert.SerializeObject(NA_defecto, Formatting.None));

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
    public string CrearPerfil(string perfil, string recursos, string nivel_acceso)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        int id_perfil = 0;

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                BEGIN TRANSACTION;

                SELECT
	                COUNT(*)
                FROM
	                {0}.perfil
                WHERE
	                nombre = @perfil

                COMMIT TRANSACTION;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@perfil", perfil);
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                // Se verifica que el nombre del nuevo perfil no exista en base de datos
                int c_perfil = int.Parse(dt.Tables[0].Rows[0][0].ToString());

                // Si no existe se procede al registro del nuevo perfil
                if (c_perfil == 0)
                {
                    // Se pone los caracteres a eliminar del arreglo
                    char[] charsToTrim = { '[', ']' };

                    //Se eliminan en ambos string, tanto de recursos como en los niveles de acceso
                    recursos = recursos.Trim(charsToTrim);
                    nivel_acceso = nivel_acceso.Trim(charsToTrim);

                    // se crean dos arreglos con los ids de los recursos y de los niveles de acceso
                    string[] recurso = recursos.Split(',');
                    string[] N_A = nivel_acceso.Split(',');

                    int cont = recurso.Length;

                    //Se crea el query para hacer el registro cuando exista el nuevo rol
                    string query = @"VALUES ";

                    for (var i = 0; i < cont - 1; i++)
                    {
                        query = string.Format(@"{0}(@id_perfil, {1}, {2}),", query, recurso[i], N_A[i]);
                    }
                    query = string.Format(@"{0}(@id_perfil, {1}, {2});", query, recurso[cont - 1], N_A[cont - 1]);

                    //Se hace el registro del nuevo rol
                    adapter = new SqlDataAdapter(String.Format(@"

                    BEGIN TRANSACTION;

                    INSERT INTO {0}.perfil (nombre)
                    VALUES
                    (@perfil);

                    SELECT @@IDENTITY;

                    COMMIT TRANSACTION;

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@perfil", perfil);
                    dt = new DataSet();
                    adapter.Fill(dt);

                    //Se obtiene el id del perfil
                    id_perfil = int.Parse(dt.Tables[0].Rows[0][0].ToString());

                    //Se le asigna los perfiles de cada aplicacion de cada rol
                    adapter = new SqlDataAdapter(String.Format(@"

                    BEGIN TRANSACTION;

                    INSERT INTO {0}.permisos_perfil_recurso (id_perfil, id_M_recurso, id_nivel_acceso)
                    {1}

                    COMMIT TRANSACTION;

                    ", Abi_maestro.esquema, query), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_perfil", id_perfil);
                    adapter.SelectCommand.ExecuteScalar();

                    result["RESULTADO"] = 1;
                }
                else
                {
                    result["RESULTADO"] = 0;
                }

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";

                adapter = new SqlDataAdapter(String.Format(@"

                BEGIN TRANSACTION;

                DELETE
                FROM {0}.perfil
                WHERE
                    id = @id_perfil;

                COMMIT TRANSACTION;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_perfil", id_perfil);
                adapter.SelectCommand.ExecuteScalar();

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
    public string Perfil_NA(int id_PPR)
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

                    SELECT
	                    NA.id,
	                    NA.nombre
                    FROM
	                    {0}.nivel_acceso NA;

                    SELECT
	                    PPR.id_nivel_acceso AS [id]
                    FROM
	                    {0}.permisos_perfil_recurso PPR
                    WHERE
	                    PPR.id = @id_PPR;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_PPR", id_PPR);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable NA = dt.Tables[0];
                DataTable PPR = dt.Tables[1];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["NA"] = JArray.Parse(JsonConvert.SerializeObject(NA, Formatting.None));
                result["RESULTADO"]["PPR"] = JArray.Parse(JsonConvert.SerializeObject(PPR, Formatting.None));

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_PPR: {0}", id_PPR));
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
    public string Perfil_msv()
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

                    SELECT
	                    NA.id,
	                    NA.nombre
                    FROM
	                    {0}.nivel_acceso NA;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable NA = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["NA"] = JArray.Parse(JsonConvert.SerializeObject(NA, Formatting.None));

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

    [WebMethod(EnableSession = true)]
    public string upd_P(int id_PPR, int id_NA)
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

                    UPDATE [{0}].[permisos_perfil_recurso] 
                    SET 
                        [id_nivel_acceso] = @id_NA
                    WHERE 
                        id = @id_PPR;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.Parameters.AddWithValue("@id_PPR", id_PPR);
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
                Mail.SendEmail(e, host, string.Format(@"id_PPR: {0}, id_NA: {1}", id_PPR, id_NA));
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
    public string upd_P_msv()
    {
        int id_NA = int.Parse(HttpContext.Current.Request.Form.Get("id_S"));
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

                int id_PPR;

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_PPR = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    update_RP_msv(id_PPR, id_NA);
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

    public void update_RP_msv(int id_PPR, int id_NA)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    UPDATE [{0}].[permisos_perfil_recurso] 
                    SET 
                        [id_nivel_acceso] = @id_NA
                    WHERE 
                        id = @id_PPR;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.Parameters.AddWithValue("@id_PPR", id_PPR);
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_PPR: {0}, id_NA: {1}", id_PPR, id_NA));
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }

    /**********************************************************************************/
    /**********************************************************************************/
    /**********************************************************************************/
}
