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

public class Roles : System.Web.Services.WebService
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
    public string Perfiles()
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
	                pf.id,
	                pf.nombre
                FROM
	                {0}.perfil pf;

                SELECT
	                pf.id
                FROM
	                {0}.perfil pf
                WHERE
	                pf.nombre = 'Banasan';

                Commit Transaction;
                 
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                DataTable Perfiles = dt.Tables[0];
                DataTable Perfil_defecto = dt.Tables[1];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["Perfiles"] = JArray.Parse(JsonConvert.SerializeObject(Perfiles, Formatting.None));
                result["RESULTADO"]["Perfil_defecto"] = JArray.Parse(JsonConvert.SerializeObject(Perfil_defecto, Formatting.None));

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
    public string CrearRol(string rol, int id_perfil)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        int id_rol = 0;

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"

                BEGIN TRANSACTION;

                SELECT
	                COUNT(*)
                FROM
	                {0}.rol
                WHERE
	                nombre = @rol

                COMMIT TRANSACTION;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@rol", rol);
                DataSet dt = new DataSet();
                adapter.Fill(dt);

                // Se verifica que el nombre del nuevo rol no exista en base de datos
                int c_rol = int.Parse(dt.Tables[0].Rows[0][0].ToString());

                // Si no existe se procede al registro del nuevo rol
                if (c_rol == 0)
                {
                    
                    //Se hace el registro del nuevo rol
                    adapter = new SqlDataAdapter(String.Format(@"

                    BEGIN TRANSACTION;

                    INSERT INTO {0}.rol (nombre)
                    VALUES
                    (@rol);

                    SELECT @@IDENTITY;

                    COMMIT TRANSACTION;

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@rol", rol);
                    dt = new DataSet();
                    adapter.Fill(dt);

                    //Se obtiene el id del rol
                    id_rol = int.Parse(dt.Tables[0].Rows[0][0].ToString());

                    //Se le asigna los perfiles, los menus y los modulos con el nuevo rol
                    adapter = new SqlDataAdapter(String.Format(@"

                    BEGIN TRANSACTION;

                    INSERT INTO {0}.rol_perfil (id_rol, id_perfil)
                    VALUES
                    (@id_rol, @id_perfil);

                    INSERT INTO {0}.permisos_rol_menu (id_rol, id_menu, id_nivel_acceso, id_aplicacion)
                    SELECT
	                    @id_rol,
	                    PRM.id_menu,
	                    3,
	                    PRM.id_aplicacion
                    FROM
	                    {0}.permisos_rol_menu PRM
                    GROUP BY
	                    PRM.id_menu,
	                    PRM.id_aplicacion;

                    INSERT INTO {0}.rol_aplicacion (id_rol, id_app, id_nivel_acceso)
                    SELECT
	                    @id_rol,
	                    APP.id,
                        3
                    FROM
	                    {0}.aplicacion APP;

                    COMMIT TRANSACTION;

                    ", Abi_maestro.esquema), conexion.getConexion());
                    adapter.SelectCommand.Parameters.AddWithValue("@id_rol", id_rol);
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
                FROM {0}.rol
                WHERE
                    id = @id_rol;

                COMMIT TRANSACTION;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_rol", id_rol);
                adapter.SelectCommand.ExecuteScalar();

                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"rol: {0}, id_perfil: {1}", rol, id_perfil));
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
    public string Rol_Perfil(int id_RP)
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
                        p.id,
                        p.nombre 
                    FROM 
                        {0}.perfil p;

                    SELECT
	                    rp.id_perfil
                    FROM 
                        {0}.rol_perfil rp
                    WHERE 
                        rp.id = @id_RP

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_RP", id_RP);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Perfil = dt.Tables[0];
                DataTable R_P = dt.Tables[1];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["Perfil"] = JArray.Parse(JsonConvert.SerializeObject(Perfil, Formatting.None));
                result["RESULTADO"]["R_P"] = JArray.Parse(JsonConvert.SerializeObject(R_P, Formatting.None));

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_RP: {0}", id_RP));
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
    public string Rol_perfil_msv()
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
                        p.id,
                        p.nombre 
                    FROM 
                        {0}.perfil p;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Perfil = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["Perfil"] = JArray.Parse(JsonConvert.SerializeObject(Perfil, Formatting.None));

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
    public string upd_RP(int id_RP, int id_perfil)
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

                    UPDATE [{0}].[rol_perfil] 
                    SET 
                        [id_perfil] = @id_perfil
                    WHERE 
                        id = @id_RP;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_perfil", id_perfil);
                adapter.SelectCommand.Parameters.AddWithValue("@id_RP", id_RP);
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
                Mail.SendEmail(e, host, string.Format(@"id_RP: {0}, id_perfil: {1}", id_RP, id_perfil));
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
    public string upd_RP_msv()
    {
        int id_perfil = int.Parse(HttpContext.Current.Request.Form.Get("id_S"));
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

                int id_RP;

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_RP = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    update_RP_msv(id_RP, id_perfil);
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

    public void update_RP_msv(int id_RP, int id_perfil)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    UPDATE [{0}].[rol_perfil] 
                    SET 
                        [id_perfil] = @id_perfil
                    WHERE 
                        id = @id_RP;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_perfil", id_perfil);
                adapter.SelectCommand.Parameters.AddWithValue("@id_RP", id_RP);
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_RP: {0}, id_perfil: {1}", id_RP, id_perfil));
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

    [WebMethod(EnableSession = true)]
    public string Rol_Modulo(int id_RA)
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
                        NA.id,
                        NA.nombre 
                    FROM 
                        {0}.nivel_acceso NA;

                    SELECT
	                    ra.id_nivel_acceso
                    FROM 
                        {0}.rol_aplicacion ra
                    WHERE 
                        ra.id = @id_RA

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_RA", id_RA);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable NA = dt.Tables[0];
                DataTable R_A = dt.Tables[1];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["NA"] = JArray.Parse(JsonConvert.SerializeObject(NA, Formatting.None));
                result["RESULTADO"]["R_A"] = JArray.Parse(JsonConvert.SerializeObject(R_A, Formatting.None));

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_RA: {0}", id_RA));
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
    public string Rol_modulo_msv()
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
                        NA.id,
                        NA.nombre 
                    FROM 
                        {0}.nivel_acceso NA;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Perfil = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["NA"] = JArray.Parse(JsonConvert.SerializeObject(Perfil, Formatting.None));

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
    public string upd_RA(int id_RA, int id_NA)
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

                    UPDATE [{0}].[rol_aplicacion] 
                    SET 
                        [id_nivel_acceso] = @id_NA
                    WHERE 
                        id = @id_RA;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.Parameters.AddWithValue("@id_RA", id_RA);
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
                Mail.SendEmail(e, host, string.Format(@"id_RA: {0}, id_NA: {1}", id_RA, id_NA));
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
    public string upd_RA_msv()
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

                int id_RA;

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_RA = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    update_RA_msv(id_RA, id_NA);
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

    public void update_RA_msv(int id_RA, int id_NA)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    UPDATE [{0}].[rol_aplicacion] 
                    SET 
                        [id_nivel_acceso] = @id_NA
                    WHERE 
                        id = @id_RA;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.Parameters.AddWithValue("@id_RA", id_RA);
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_RA: {0}, id_NA: {1}", id_RA, id_NA));
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

    [WebMethod(EnableSession = true)]
    public string upd_RM_Grupo(int id_grupo, string rol, string cat, string app, int id_NA)
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

                    UPDATE {0}.permisos_rol_menu
                    SET id_nivel_acceso = @id_NA
                    WHERE
	                    id IN (
	                    SELECT
		                    PRM.id
	                    FROM
		                    {0}.acordeon ACC
	                    INNER JOIN {0}.menu_aplicacion MA ON MA.id_acordeon = ACC.id
	                    INNER JOIN {0}.aplicacion APP ON MA.id_aplicacion = APP.id
	                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
	                    INNER JOIN {0}.menu M ON MA.id_menu = M.id
	                    INNER JOIN {0}.permisos_rol_menu PRM ON PRM.id_menu = M.id
	                    AND PRM.id_aplicacion = APP.id
	                    INNER JOIN {0}.rol R ON PRM.id_rol = R.id

	                    WHERE
		                    ACC.id = @id_grupo
	                    AND R.nombre = @rol
	                    AND CAT.nombre = @cat
	                    AND APP.titulo_login = @app

	                    GROUP BY
		                    PRM.id
                    );

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_grupo", id_grupo);
                adapter.SelectCommand.Parameters.AddWithValue("@rol", rol);
                adapter.SelectCommand.Parameters.AddWithValue("@cat", cat);
                adapter.SelectCommand.Parameters.AddWithValue("@app", app);
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.ExecuteScalar();

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
                Mail.SendEmail(e, host, string.Format(@"id_grupo: {0}, rol: {1}, cat: {2}, app: {3}, id_NA: {4}", id_grupo, rol, cat, app, id_NA));
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
    public string upd_RM_Menu(int id_menu, string rol, string cat, string app, int id_NA)
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

                    UPDATE {0}.permisos_rol_menu
                    SET id_nivel_acceso = @id_NA
                    WHERE
	                    id IN (
	                    SELECT
		                    PRM.id
	                    FROM
		                    {0}.menu_aplicacion MA
	                    INNER JOIN {0}.aplicacion APP ON MA.id_aplicacion = APP.id
	                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
	                    INNER JOIN {0}.menu M ON MA.id_menu = M.id
	                    INNER JOIN {0}.permisos_rol_menu PRM ON PRM.id_menu = M.id
	                    AND PRM.id_aplicacion = APP.id
	                    INNER JOIN {0}.rol R ON PRM.id_rol = R.id

	                    WHERE
		                    M.id = @id_menu
	                    AND R.nombre = @rol
	                    AND CAT.nombre = @cat
	                    AND APP.titulo_login = @app

	                    GROUP BY
		                    PRM.id

	                    UNION ALL

	                    SELECT
		                    PRMH.id
	                    FROM
		                    {0}.menu_aplicacion MA
	                    INNER JOIN {0}.aplicacion APP ON MA.id_aplicacion = APP.id
	                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
	                    INNER JOIN {0}.menu M ON MA.id_menu = M.id
	                    INNER JOIN {0}.permisos_rol_menu PRM ON PRM.id_menu = M.id
	                    AND PRM.id_aplicacion = APP.id
	                    INNER JOIN {0}.rol R ON PRM.id_rol = R.id

	                    INNER JOIN {0}.estructura_menu EM ON EM.id_menu_padre = M.id
	                    INNER JOIN {0}.menu MH ON EM.id_menu_hijo = MH.id
	                    INNER JOIN {0}.permisos_rol_menu PRMH ON PRMH.id_menu = MH.id
	                    AND PRMH.id_rol = R.id
	                    AND PRMH.id_aplicacion = APP.id

	                    WHERE
		                    M.id = @id_menu
	                    AND R.nombre = @rol
	                    AND CAT.nombre = @cat
	                    AND APP.titulo_login = @app

	                    GROUP BY
		                    PRMH.id
                    );

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_menu", id_menu);
                adapter.SelectCommand.Parameters.AddWithValue("@rol", rol);
                adapter.SelectCommand.Parameters.AddWithValue("@cat", cat);
                adapter.SelectCommand.Parameters.AddWithValue("@app", app);
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.ExecuteScalar();

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
                Mail.SendEmail(e, host, string.Format(@"id_menu: {0}, rol: {1}, cat: {2}, app: {3}, id_NA: {4}", id_menu, rol, cat, app, id_NA));
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
    public string upd_RM_Submenu(int id_PRM, int id_NA)
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

                    UPDATE {0}.permisos_rol_menu
                    SET id_nivel_acceso = @id_NA
                    WHERE
	                    id = @id_PRM;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_PRM", id_PRM);
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.ExecuteScalar();

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
                Mail.SendEmail(e, host, string.Format(@"id_PRM: {0}, id_NA: {1}", id_PRM, id_NA));
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
    public string Rol_NA()
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
                        NA.id,
                        NA.nombre 
                    FROM 
                        {0}.nivel_acceso NA;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable Perfil = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["NA"] = JArray.Parse(JsonConvert.SerializeObject(Perfil, Formatting.None));

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
    public string upd_RM_msv_Grupo()
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

                int id_grupo;
                string rol, cat, app; 

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_grupo = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    rol = Datos.Rows[m]["Rol"].ToString();
                    cat = Datos.Rows[m]["Categoría Módulo"].ToString();
                    app = Datos.Rows[m]["Módulo"].ToString();

                    update_msv_Grupo(id_grupo, rol, cat, app, id_NA);
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

    [WebMethod(EnableSession = true)]
    public string upd_RM_msv_Menu()
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

                int id_menu;
                string rol, cat, app;

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_menu = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    rol = Datos.Rows[m]["Rol"].ToString();
                    cat = Datos.Rows[m]["Categoría Módulo"].ToString();
                    app = Datos.Rows[m]["Módulo"].ToString();

                    update_msv_Menu(id_menu, rol, cat, app, id_NA);
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

    [WebMethod(EnableSession = true)]
    public string upd_RM_msv_Submenu()
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

                int id_PRM;

                for (int m = 0; m < Datos.Rows.Count; m++)
                {
                    id_PRM = int.Parse(Datos.Rows[m]["DT_RowId"].ToString());
                    update_msv_Submenu(id_PRM, id_NA);
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

    public void update_msv_Grupo(int id_grupo, string rol, string cat, string app, int id_NA)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    UPDATE {0}.permisos_rol_menu
                    SET id_nivel_acceso = @id_NA
                    WHERE
	                    id IN (
	                    SELECT
		                    PRM.id
	                    FROM
		                    {0}.acordeon ACC
	                    INNER JOIN {0}.menu_aplicacion MA ON MA.id_acordeon = ACC.id
	                    INNER JOIN {0}.aplicacion APP ON MA.id_aplicacion = APP.id
	                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
	                    INNER JOIN {0}.menu M ON MA.id_menu = M.id
	                    INNER JOIN {0}.permisos_rol_menu PRM ON PRM.id_menu = M.id
	                    AND PRM.id_aplicacion = APP.id
	                    INNER JOIN {0}.rol R ON PRM.id_rol = R.id

	                    WHERE
		                    ACC.id = @id_grupo
	                    AND R.nombre = @rol
	                    AND CAT.nombre = @cat
	                    AND APP.titulo_login = @app

	                    GROUP BY
		                    PRM.id
                    );

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_grupo", id_grupo);
                adapter.SelectCommand.Parameters.AddWithValue("@rol", rol);
                adapter.SelectCommand.Parameters.AddWithValue("@cat", cat);
                adapter.SelectCommand.Parameters.AddWithValue("@app", app);
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_grupo: {0}, rol: {1}, cat: {2}, app: {3}, id_NA: {4}", id_grupo, rol, cat, app, id_NA));
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }

    public void update_msv_Menu(int id_menu, string rol, string cat, string app, int id_NA)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    UPDATE {0}.permisos_rol_menu
                    SET id_nivel_acceso = @id_NA
                    WHERE
	                    id IN (
	                    SELECT
		                    PRM.id
	                    FROM
		                    {0}.menu_aplicacion MA
	                    INNER JOIN {0}.aplicacion APP ON MA.id_aplicacion = APP.id
	                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
	                    INNER JOIN {0}.menu M ON MA.id_menu = M.id
	                    INNER JOIN {0}.permisos_rol_menu PRM ON PRM.id_menu = M.id
	                    AND PRM.id_aplicacion = APP.id
	                    INNER JOIN {0}.rol R ON PRM.id_rol = R.id

	                    WHERE
		                    M.id = @id_menu
	                    AND R.nombre = @rol
	                    AND CAT.nombre = @cat
	                    AND APP.titulo_login = @app

	                    GROUP BY
		                    PRM.id

	                    UNION ALL

	                    SELECT
		                    PRMH.id
	                    FROM
		                    {0}.menu_aplicacion MA
	                    INNER JOIN {0}.aplicacion APP ON MA.id_aplicacion = APP.id
	                    INNER JOIN {0}.categoria_aplicacion CAT ON APP.id_categoria = CAT.id
	                    INNER JOIN {0}.menu M ON MA.id_menu = M.id
	                    INNER JOIN {0}.permisos_rol_menu PRM ON PRM.id_menu = M.id
	                    AND PRM.id_aplicacion = APP.id
	                    INNER JOIN {0}.rol R ON PRM.id_rol = R.id

	                    INNER JOIN {0}.estructura_menu EM ON EM.id_menu_padre = M.id
	                    INNER JOIN {0}.menu MH ON EM.id_menu_hijo = MH.id
	                    INNER JOIN {0}.permisos_rol_menu PRMH ON PRMH.id_menu = MH.id
	                    AND PRMH.id_rol = R.id
	                    AND PRMH.id_aplicacion = APP.id

	                    WHERE
		                    M.id = @id_menu
	                    AND R.nombre = @rol
	                    AND CAT.nombre = @cat
	                    AND APP.titulo_login = @app

	                    GROUP BY
		                    PRMH.id
                    );

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_menu", id_menu);
                adapter.SelectCommand.Parameters.AddWithValue("@rol", rol);
                adapter.SelectCommand.Parameters.AddWithValue("@cat", cat);
                adapter.SelectCommand.Parameters.AddWithValue("@app", app);
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_menu: {0}, rol: {1}, cat: {2}, app: {3}, id_NA: {4}", id_menu, rol, cat, app, id_NA));
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }

    public void update_msv_Submenu(int id_PRM, int id_NA)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    BEGIN TRANSACTION;

                    UPDATE {0}.permisos_rol_menu
                    SET id_nivel_acceso = @id_NA
                    WHERE
	                    id = @id_PRM;

                    COMMIT TRANSACTION;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_PRM", id_PRM);
                adapter.SelectCommand.Parameters.AddWithValue("@id_NA", id_NA);
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"id_PRM: {0}, id_NA: {1}", id_PRM, id_NA));
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
    }


}
