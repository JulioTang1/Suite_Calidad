using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
//Regex
using System.Text.RegularExpressions;
using System.Web;

/// <summary>
/// Summary description for dtQueryStrings
/// </summary>
public class dtQueryStrings
{
    private string id { get; set; }
    private string schema { get; set; }
    private string view { get; set; }
    private string nombre { get; set; }
    private string consulta { get; set; }
    private string replace { get; set; }
    private string tableConfig { get; set; }
    private ConexionSQL conexion = new ConexionSQL();


    public dtQueryStrings(string schema)
    {
        this.tableConfig = schema+".dataTableConfig";
        //
        // TODO: Add constructor logic here
        //
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
                            {0}
                        WHERE
                            nombre = '{1}';
                ", this.tableConfig, this.nombre), conexion.getConexion());

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

                if (table1.Rows.Count > 0 && this.replace != null) {
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

    public string qColumnsName(string nombre)
    {
        this.nombre = nombre;
        this.qConfigData();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                if(this.view != "")
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

    public string qAditionalData(string nombre)
    {
        this.nombre = nombre;
        this.qConfigData();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE EXTRAE INFORMACION ADICIONAL DE LAS TABLAS 
                //(AQUI PUEDE IR TODO LO QUE SE QUIERA TRAER EN UN FUTURO)
                string query;
                query = String.Format(@"

                    SELECT detalle
                    FROM {0}.dataTableConfig
                    WHERE id = {1}

                ", this.schema, this.id);

                conexion.closeConexion();
                return query;
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

                string data,type,longitud,tempTable = @"
                    CREATE TABLE #consulta (";
                for (int i = 0; i < table.Rows.Count; i++){
                    data = table.Rows[i]["data"].ToString();
                    type = table.Rows[i]["type"].ToString();
                    longitud = table.Rows[i]["longitud"].ToString();
                    if (i == table.Rows.Count-1)
                    {
                        //ultima fila
                        if (longitud != "")
                        {
                            //longitud 
                            tempTable = string.Concat(tempTable, @"
                            [", data, @"] ", type, @" (", longitud, @")");
                        }
                        else{
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
                            if ( (type == "char") || (type == "varchar") || (type == "nvarchar") || (type == "text") || (type == "nchar") || (type == "ntext"))
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

    public string qColumnsData(int draw, int start, int length, int n_col, string nombre, string replace)
    {
        this.nombre = nombre;
        this.replace = replace;
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
        /* Organizar estas lineas en una función y devolver un objeto con estas características */
        /* la función recibe un NameValueCollection, organiza los datos de este formulario y los pasa */
        /* a otra función que construye las consultas */
        /*---------------------------------------------------------------------------------------*/

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
            if(columns[order.column].data == "")
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
        if(this.view != "")
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
		                        WHERE
			                        Row BETWEEN (@start + 1)
		                        AND (@start + @length) 
 		
                        END

                        SELECT @recordsTotal = COUNT (*) FROM [{2}].[{3}] --db.schema.view
                        SELECT @recordsTotal AS recordsTotal

                        SELECT @recordsFiltered = COUNT(*)
		                FROM
			                [{2}].[{3}] --db.schema.view
		                {4} --WHERE
                        SELECT @recordsFiltered AS recordsFiltered

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
		                        WHERE
			                        Row BETWEEN (@start + 1)
		                        AND (@start + @length) 
 		
                        END

                        SELECT @recordsTotal = COUNT (*) FROM #consulta --consulta
                        SELECT @recordsTotal AS recordsTotal

                        SELECT @recordsFiltered = COUNT(*)
		                FROM
			                #consulta AS consulta --Consulta
		                {3} --WHERE
                        SELECT @recordsFiltered AS recordsFiltered

                        DROP TABLE #consulta;

                ", queryOrder, queryCols, this.consulta, queryWhere, tempTable);

            return query;
        }
    }

    //mio
    public string qColumnsDataExcel(int draw, int start, int length, int n_col, string nombre, string replace)
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
        dictAjaxDataTables.Remove("name_pag");

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
}