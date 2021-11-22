using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Globalization;
using System.Xml.Serialization;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Drawing;
using System.Collections.Specialized;
using System.Net;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using OfficeOpenXml.Drawing;
using OfficeOpenXml.Drawing.Chart;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

/// <summary>
/// Summary description for WS
/// </summary>
/// 

/* NOMBRE DEL ESQUEMA */
public static class Abi_maestro
{
    public const string esquema = "dbo";
}

// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WS : System.Web.Services.WebService
{

    /******************************************** Login ****************************************************************/

    /* Provisional */
    [WebMethod(EnableSession = true)]
    public string Nivel_acceso_menu(string rol)
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
	                    permisos.id_menu AS id_menu,
	                    permisos.id_nivel_acceso AS id_acceso
                    FROM
	                    {0}.permisos_rol_menu permisos
                    INNER JOIN {0}.rol rol ON permisos.id_rol = rol.id
                    INNER JOIN {0}.nivel_acceso N_acceso ON permisos.id_nivel_acceso = N_acceso.id
                    WHERE
	                    rol.nombre = '{1}'
                    AND N_acceso.nombre != 'Sin Acceso';

                ", Abi_maestro.esquema, rol), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0]; ;

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
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    /* Provisional */
    [WebMethod(EnableSession = true)]
    public string sidebar_dynamic(string App, string Rol)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapterSidebar = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapterSidebar = new SqlDataAdapter(String.Format(@"

                    -- conocer todos los elementos del sidebar
                    SELECT
	                    menu.id AS ID_menu,
	                    menu.ruta_interfaz AS url_vista,
	                    menu.nombre_opcion AS nombre_sidebar,
	                    menu.es_vista AS es_vista,
	                    menu.nombre_icono AS icono,
	                    menu.descripcion AS tooltip,
                        acordeon.id AS ID_header
                    FROM
	                    {0}.menu AS menu
                    INNER JOIN {0}.menu_aplicacion AS menuApp ON menuApp.id_menu = menu.id
                    INNER JOIN {0}.acordeon AS acordeon ON menuApp.id_acordeon = acordeon.id
                    INNER JOIN {0}.aplicacion AS App ON menuApp.id_aplicacion = App.id
                    INNER JOIN {0}.permisos_rol_menu AS pass_rol_menu ON pass_rol_menu.id_menu = menu.id
                    INNER JOIN {0}.rol AS rol ON pass_rol_menu.id_rol = rol.id
                    INNER JOIN {0}.nivel_acceso AS nivelPass ON pass_rol_menu.id_nivel_acceso = nivelPass.id
                    WHERE
	                    rol.nombre = '{1}'
                    AND App.nombre = '{2}'
                    AND nivelPass.nombre <> 'Sin Acceso'
                    ORDER BY
                        acordeon.orden,
	                    menuApp.orden_menu;

                    SELECT
	                    acordeon.id     AS ID_header,
                        acordeon.nombre AS header
                    FROM
	                    {0}.acordeon AS acordeon
                    ORDER BY
                        acordeon.orden;

                    -- jerarquía del menu del sidebar
                    -- estructura de 1 nivel
                    SELECT
	                    menu.id AS ID_primer_nivel
                    FROM
	                    {0}.menu AS menu
                    LEFT JOIN {0}.estructura_menu AS estMenu ON estMenu.id_menu_padre = menu.id
                    WHERE
	                    estMenu.id_menu_padre IS NULL
                    AND menu.id NOT IN (
	                    SELECT
		                    estructura.id_menu_hijo
	                    FROM
		                    {0}.estructura_menu AS estructura
                    );

                    -- estructura de 2 niveles
                    SELECT
	                    estructura.id_menu_padre AS ID_primer_nivel,
	                    estructura.id_menu_hijo AS ID_segundo_nivel
                    FROM
	                    {0}.estructura_menu AS estructura
                    WHERE
	                    estructura.id_menu_padre NOT IN (
		                    SELECT
			                    estructura.id_menu_hijo
		                    FROM
			                    {0}.estructura_menu AS estructura
	                    )
                    AND estructura.id_menu_hijo NOT IN (
	                    SELECT
		                    estructura.id_menu_padre
	                    FROM
		                    {0}.estructura_menu AS estructura
                    );

                    -- estructura de 3 niveles
                    SELECT
	                    ID_primer_nivel = (
		                    SELECT
			                    est1.id_menu_padre
		                    FROM
			                    {0}.estructura_menu AS est1
		                    WHERE
			                    est1.id_menu_padre NOT IN (
				                    SELECT
					                    estructura.id_menu_hijo
				                    FROM
					                    {0}.estructura_menu AS estructura
			                    )
		                    AND estructura.id_menu_padre = est1.id_menu_hijo
	                    ),
	                    estructura.id_menu_padre AS ID_segundo_nivel,
	                    estructura.id_menu_hijo AS ID_tercer_nivel
                    FROM
	                    {0}.estructura_menu AS estructura
                    WHERE
	                    estructura.id_menu_padre IN (
		                    SELECT
			                    estructura.id_menu_hijo
		                    FROM
			                    {0}.estructura_menu AS estructura
		                    WHERE
			                    estructura.id_menu_padre NOT IN (
				                    SELECT
					                    estructura.id_menu_hijo
				                    FROM
					                    {0}.estructura_menu AS estructura
			                    )
	                    );

                ", Abi_maestro.esquema, Rol, App), conexion.getConexion());
                DataSet dtSidebar = new DataSet();
                adapterSidebar.Fill(dtSidebar);
                DataTable tableSidebar = dtSidebar.Tables[0];
                DataTable tableHeader = dtSidebar.Tables[1];
                DataTable tableOneLevel = dtSidebar.Tables[2];
                DataTable tableTwoLevels = dtSidebar.Tables[3];
                DataTable tableThreeLevels = dtSidebar.Tables[4];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO_SIDEBAR"] = JArray.Parse(JsonConvert.SerializeObject(tableSidebar, Formatting.None));
                result["RESULTADO_HEADER"] = JArray.Parse(JsonConvert.SerializeObject(tableHeader, Formatting.None));
                result["RESULTADO_ONE_LEVEL"] = JArray.Parse(JsonConvert.SerializeObject(tableOneLevel, Formatting.None));
                result["RESULTADO_TWO_LEVELS"] = JArray.Parse(JsonConvert.SerializeObject(tableTwoLevels, Formatting.None));
                result["RESULTADO_THREE_LEVELS"] = JArray.Parse(JsonConvert.SerializeObject(tableThreeLevels, Formatting.None));
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

    /* CONSULTA SOBRE EL NIVEL DE ACCESO QUE TIENE UN ROL EN LAS VISTAS QUE ESTA AUTORIZADO A VER O EDITAR*/
    [WebMethod(EnableSession = true)]
    public string acceso_menu(int id_rol)
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
	                    permisos.id_menu AS id_menu,
	                    permisos.id_nivel_acceso AS id_acceso
                    FROM
	                    {0}.permisos_rol_menu permisos
                    INNER JOIN {0}.rol rol ON permisos.id_rol = rol.id
                    INNER JOIN {0}.nivel_acceso N_acceso ON permisos.id_nivel_acceso = N_acceso.id
                    WHERE
	                    rol.id = @id_rol
                    AND N_acceso.id <> 3;

                    Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@id_rol", id_rol);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0]; ;

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
                Mail.SendEmail(e, host, String.Format(@"id_rol: {0}", id_rol));
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

    /* CONSULTA SOBRE LAS VISTAS QUE PUEDE VER EL USUARIO CON UN ROL PARA GENERAR EL SIDEBAR */
    [WebMethod(EnableSession = true)]
    public string sidebar_dinamico(string id_app, string id_rol)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapterSidebar = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapterSidebar = new SqlDataAdapter(String.Format(@"

                    Begin Transaction;

                    -- conocer todos los elementos del sidebar
                    SELECT
	                    menu.id AS ID_menu,
	                    menu.ruta_interfaz AS url_vista,
	                    menu.nombre_opcion AS nombre_sidebar,
	                    menu.es_vista AS es_vista,
	                    menu.nombre_icono AS icono,
	                    menu.descripcion AS tooltip,
                        acordeon.id AS ID_header
                    FROM
	                    {0}.menu AS menu
                    INNER JOIN {0}.menu_aplicacion AS menuApp ON menuApp.id_menu = menu.id
                    INNER JOIN {0}.acordeon AS acordeon ON menuApp.id_acordeon = acordeon.id
                    INNER JOIN {0}.aplicacion AS App ON menuApp.id_aplicacion = App.id
                    INNER JOIN {0}.permisos_rol_menu AS pass_rol_menu ON pass_rol_menu.id_menu = menu.id
                    INNER JOIN {0}.rol AS rol ON pass_rol_menu.id_rol = rol.id
                    INNER JOIN {0}.nivel_acceso AS nivelPass ON pass_rol_menu.id_nivel_acceso = nivelPass.id
                    WHERE
	                    rol.id = @id_rol
                    AND App.id = @id_app
                    AND nivelPass.nombre <> 'Sin Acceso'
                    ORDER BY
                        acordeon.orden,
	                    menuApp.orden_menu;

                    SELECT
	                    acordeon.id     AS ID_header,
                        acordeon.nombre AS header
                    FROM
	                    {0}.acordeon AS acordeon
                    ORDER BY
                        acordeon.orden;

                    -- jerarquía del menu del sidebar
                    -- estructura de 1 nivel
                    SELECT
	                    menu.id AS ID_primer_nivel
                    FROM
	                    {0}.menu AS menu
                    LEFT JOIN {0}.estructura_menu AS estMenu ON estMenu.id_menu_padre = menu.id
                    WHERE
	                    estMenu.id_menu_padre IS NULL
                    AND menu.id NOT IN (
	                    SELECT
		                    estructura.id_menu_hijo
	                    FROM
		                    {0}.estructura_menu AS estructura
                    );

                    -- estructura de 2 niveles
                    SELECT
	                    estructura.id_menu_padre AS ID_primer_nivel,
	                    estructura.id_menu_hijo AS ID_segundo_nivel
                    FROM
	                    {0}.estructura_menu AS estructura
                    WHERE
	                    estructura.id_menu_padre NOT IN (
		                    SELECT
			                    estructura.id_menu_hijo
		                    FROM
			                    {0}.estructura_menu AS estructura
	                    )
                    AND estructura.id_menu_hijo NOT IN (
	                    SELECT
		                    estructura.id_menu_padre
	                    FROM
		                    {0}.estructura_menu AS estructura
                    );

                    Commit Transaction;

                ", Abi_maestro.esquema), conexion.getConexion());
                adapterSidebar.SelectCommand.Parameters.AddWithValue("@id_rol", id_rol);
                adapterSidebar.SelectCommand.Parameters.AddWithValue("@id_app", id_app);
                DataSet dtSidebar = new DataSet();
                adapterSidebar.Fill(dtSidebar);
                DataTable tableSidebar = dtSidebar.Tables[0];
                DataTable tableHeader = dtSidebar.Tables[1];
                DataTable tableOneLevel = dtSidebar.Tables[2];
                DataTable tableTwoLevels = dtSidebar.Tables[3];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = new JObject();
                result["RESULTADO"]["SIDEBAR"]    = JArray.Parse(JsonConvert.SerializeObject(tableSidebar, Formatting.None));
                result["RESULTADO"]["HEADER"]     = JArray.Parse(JsonConvert.SerializeObject(tableHeader, Formatting.None));
                result["RESULTADO"]["ONE_LEVEL"]  = JArray.Parse(JsonConvert.SerializeObject(tableOneLevel, Formatting.None));
                result["RESULTADO"]["TWO_LEVELS"] = JArray.Parse(JsonConvert.SerializeObject(tableTwoLevels, Formatting.None));
                conexion.closeConexion();

            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, String.Format(@"id_rol: {0}, id_app: {1}", id_rol, id_app));
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

    /*******************************************************************************************************************/
    /********************************************Generar reportes en excel**********************************************/
    /*******************************************************************************************************************/
    [WebMethod(EnableSession = true)]
    public string excelState(int id)
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
	                    *
                    FROM
	                    {0}.D_Registro_Excel
                    WHERE
	                    id = {1}

                ", Abi_maestro.esquema, id), conexion.getConexion());
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

    /********************************************REPORTE BD*************************************************************/

    //Los siguientes tres metodos se encargan de mostrar las graficas//
    /*******************************************************************************************************/
    //Se crea la clase lineChart de la forma en la que la grafica -- name y data
    public class lineChart
    {
        public float?[] data { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public int? yAxis { get; set; }
        public Tooltip tooltip { get; set; }
    }

    public class Tooltip
    {
        public string valueSuffix { get; set; }
    }

    //Se crea la clase pieChart --name & y
    public class pieChart
    {
        public string name { get; set; }
        public float y { get; set; }
    }

    //MODIFICAR LA SIGUIENTE LINEA CON EL ESQUEMA DONDE SE ENCUENTRA 'graphicConfig' !!!!
    private gQueryStrings dataG = new gQueryStrings(Abi_maestro.esquema);

    [WebMethod(EnableSession = true)]
    public string data_graph_column_names(string nombre)
    {
        /*Construcción de la consulta*/
        string query = this.dataG.gColumnsName(nombre);

        JObject result = new JObject();
        JObject resultado = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(query, conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 180;
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                //Se trae el nombre del esquema
                string esquema = this.dataG.gQueryEsquema(nombre);

                //Trae el id para la configuracion
                int id = this.dataG.gQueryId(nombre);

                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                    SELECT D.nombre, D.EJEX, D.EJEY, D.SERIE, D.SHOW, D.append, G.groupBy, titulo, subtitulo, T.type, D.mark,
                    D.tituloEjeX, D.tituloEjeY, D.legendUser, D.detalle, D.[values], ISNULL(D.unidades, '') AS unidades, 
                    ISNULL(D.Unidades_2, '') AS unidades_2, ISNULL(D.tituloEjeY_2, '') AS tituloEjeY_2,
                    D.tickIntervalEjeY, D.tickIntervalEjeY2, D.maxEjeY, D.maxEjeY2, D.minEjeY, D.minEjeY2
                    FROM {0}.dashboardConfig AS D
                    INNER JOIN {0}.groupBy AS G
                    ON D.id_groupBy = G.id
                    INNER JOIN {0}.type AS T
                    ON D.id_type = T.id
                    WHERE D.id_graphicConfig = {1}
                    AND D.append = 1
                    ORDER BY D.orden
                ", esquema, id), conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 180;
                dt = new DataSet();
                adapter.Fill(dt);
                DataTable table1 = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                resultado["COLUMNS"] = JArray.Parse(JsonConvert.SerializeObject(table, Formatting.None));
                resultado["HIGHCHARTS"] = JArray.Parse(JsonConvert.SerializeObject(table1, Formatting.None));   //Configuracion highcharts
                result["RESULTADO"] = resultado;
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"nombre grafica: {0}", nombre));
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
    public string data_graph_column_filters(string col_name, string cols, string col, string nombre, string replace)
    {
        string queryFilter = this.dataG.gColumnsFilter(col_name, cols, col, nombre, replace);

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(queryFilter, conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 180;
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTableCollection tables = dt.Tables;
                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(tables[0], Formatting.None));

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
    public string number_threads()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE CREA EL REGISTRO Y SE RETORNA EL ID, 
                //SE ELIMINAN REGISTROS QUE SUPEREN LOS -60 MIN A PARTIR DEL MOMENTO ACTUAL
                adapter = new SqlDataAdapter(string.Format(@"

                    --DETECTA EL NUMERO DE HILOS ENE EJECUCION PARA ESE USUARIO
                    DECLARE @numero_activos INT 

                    SELECT @numero_activos = COUNT(*)
                    FROM {0}.graphicHistoric
                    WHERE token = '{1}'
                    AND activo = 1
                    GROUP BY token

                    SELECT ISNULL(@numero_activos, 0) AS hilos

                ",Abi_maestro.esquema, Session["token"].ToString()), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(table, Formatting.None));
                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"number_threads"));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "Error en la conexion";
            conexion.closeConexion();
        }
        
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    [WebMethod(EnableSession = true)]
    public string data_graph(int n_graph, int n_col, string nombre, string replace, float key)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE CREA EL REGISTRO Y SE RETORNA EL ID, 
                //SE ELIMINAN REGISTROS QUE SUPEREN LOS -60 MIN A PARTIR DEL MOMENTO ACTUAL
                adapter = new SqlDataAdapter(string.Format(@"

                    --  ELIMINA
                    DELETE FROM {0}.graphicHistoric
                    WHERE fecha <= DATEADD(N, -60, GETDATE())

                    --CREA
                    INSERT INTO {0}.graphicHistoric (id_graphicConfig, id_uniqueGraph, fecha, activo, error, abortar, token)
                    SELECT
	                    GC.id AS id_graphicConfig,
	                    '{2}' AS id_uniqueGraph,
	                    GETDATE() AS fecha,
                        1 AS abortar,
                        0 AS error,
                        0 AS abortar,
                        '{3}' AS token
                    FROM {0}.graphicConfig AS GC
                    WHERE GC.nombre = '{1}'

                    -- RETORNA ID
                    SELECT @@IDENTITY AS id_historic;
                ", Abi_maestro.esquema, nombre, key.ToString(), Session["token"].ToString()), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable id_historic_table = dt.Tables[0];

                int id_historic = int.Parse(id_historic_table.Rows[0]["id_historic"].ToString());

                //SE CONSULTA EL ESTADO Y EL ERROR
                adapter = new SqlDataAdapter(string.Format(@"

                    SELECT
	                    id,
	                    id_uniqueGraph,
	                    fecha,
	                    json,
	                    activo,
	                    error,
                        abortar
                    FROM {0}.graphicHistoric
                    WHERE id = {1}

                ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                dt = new DataSet();
                adapter.Fill(dt);
                DataTable historic = dt.Tables[0];

                string host = HttpContext.Current.Request.Url.Host;
                NameValueCollection nvc = HttpContext.Current.Request.Form;
                Task tasks = Task.Factory.StartNew(() => intermedio(n_graph, n_col, nombre, replace, nvc, key, id_historic, host));

                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(historic, Formatting.None));
                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, string.Format(@"Nombre grafica: {0}, Usuario: {1}, token: {2}, navegador: {3}, Replace: {4}
                ", nombre, Session["User"], Session["token"], Session["navegador"], replace));
                conexion.closeConexion();
            }
        }
        else
        {
            result["ESTADO"] = "FALSE";
            result["MENSAJE"] = "Error en la conexion";
            conexion.closeConexion();
        }


        
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    /* CONTORLA EL HILO QUE CONSTRUYE LA GRAFICA */
    public void intermedio(int n_graph, int n_col, string nombre, string replace, NameValueCollection nvc, float key,
        int id_historic, string host)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                Thread control = new Thread(() => period_graph(n_graph, n_col, nombre, replace, nvc, key, id_historic, host));
                control.Start();

                DataSet dt = new DataSet();
                DataTable cancelar_dt = new DataTable();

                while (control.ThreadState.ToString() == "Running")
                {
                    Thread.Sleep(1000);

                    //Validacion de cancelacion del reporte
                    adapter = new SqlDataAdapter(String.Format(@"

                        SELECT abortar
                        FROM {0}.graphicHistoric
                        WHERE id = {1}

                    ",Abi_maestro.esquema, id_historic), conexion.getConexion());
                    dt = new DataSet();
                    adapter.Fill(dt);
                    cancelar_dt = new DataTable();
                    cancelar_dt = dt.Tables[0];

                    string cancelar = cancelar_dt.Rows[0]["abortar"].ToString();

                    //Cancelacion del reporte si la bandera esta activa
                    if (cancelar == "True")
                    {
                        //Se detiene la proyección del país si se está ejecutando
                        adapter = new SqlDataAdapter(String.Format(@"
                            DECLARE @query NVARCHAR(255)
                            DECLARE @consulta_activa INT

                            SELECT @consulta_activa = sesion
                            FROM {0}.graphicHistoric
                            WHERE id = {1}
	
                            IF @consulta_activa IS NOT NULL
                            BEGIN
	                            SET @query = 'KILL ' + CAST(@consulta_activa AS NVARCHAR(4))
	                            EXEC (@query)
                            END

                        ",Abi_maestro.esquema, id_historic), conexion.getConexion());
                        adapter.SelectCommand.ExecuteScalar();

                        //Se detiene el hilo que está realizando el reporte
                        control.Abort();
                    }
                }

                //SE APAGAN BANDERAS DE ACTIVO
                adapter = new SqlDataAdapter(string.Format(@"

                    UPDATE {0}.graphicHistoric
                    SET activo = 0, Sesion = NULL
                    WHERE id = {1}

                ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                adapter.SelectCommand.ExecuteScalar();

                conexion.closeConexion();
            }
            catch (Exception e)
            {
                /*La siguiente condicion evita enviar un mensaje derivado de cancelar un hilo que mata una sesion en sql, 
                 * algunas veces esta ya no se esta ejecutando, por lo tanto, no es necesario enviar correo.
                 Los mensajes que se muestran abajo son los posibles fragmentos de ese correo que no se debe enviar*/
                if ((!e.Message.ToString().Contains("Process ID") && !e.Message.ToString().Contains("is not an active process ID"))
                && (!e.Message.ToString().Contains("El ID de proceso") && !e.Message.ToString().Contains("no es un ID de proceso activo"))
                && !e.Message.ToString().Contains("no es un id. de proceso activo."))
                {
                    //SE ACTIVA LA VARIABLE DE ERROR Y SE APAGA ACTIVO
                    adapter = new SqlDataAdapter(string.Format(@"

                        UPDATE {0}.graphicHistoric
                        SET activo = 0, error = 1
                        WHERE id = {1}

                    ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    Mail.SendEmail(e, host, string.Format(@"Nombre grafica: {0}", nombre));
                }
                conexion.closeConexion();
            }
        }
        else
        {
            conexion.closeConexion();
        }
        conexion.closeConexion();
    }

    public void period_graph(int n_graph, int n_col, string nombre, string replace, NameValueCollection nvc, float key,
        int id_historic, string host)
    {
        //variables
        string[] ejex;
        string[] ejey;
        string[] ejey2_title;
        string[] serie;
        string[] showString;
        string[] groupMath;
        string[] type;
        string[] type2;
        string[] pareto;
        string[] unidades;
        string[] unidades2;
        string[] nombreType;
        int i, j, l;
        int[] show = new int[0];
        int dim = 0;
        string[] ejeXOut = new string[0];
        string tempTable = "";
        int tamaSeries = 100;
        JObject resultado = new JObject();

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //Se trae el nombre del esquema
                string esquema = this.dataG.gQueryEsquema(nombre);

                //Se trae el nombre de la vista
                string vista = this.dataG.gQueryVista(nombre);

                //Trae el id para la configuracion
                int id = this.dataG.gQueryId(nombre);

                adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                    SELECT D.nombre, D.EJEX, D.EJEY, D.SERIE, D.SHOW, D.append, G.groupBy, titulo, subtitulo, T.type, D.mark, 
                    D.tituloEjeX, D.tituloEjeY, D.legendUser, D.pareto, D.colors, ISNULL(T2.type, 0) AS type2,
                    ISNULL(D.unidades, '') AS unidades, ISNULL(D.unidades_2, '') AS unidades_2, D.nombreType,D.tituloEjeY_2
                    FROM {0}.dashboardConfig AS D
                    INNER JOIN {0}.groupBy AS G
                    ON D.id_groupBy = G.id
                    INNER JOIN {0}.type AS T
                    ON D.id_type = T.id
                    LEFT JOIN {0}.type AS T2
                    ON D.id_type_2 = T2.id
                    WHERE D.id_graphicConfig = {1}
                    ORDER BY D.orden
                ", esquema, id), conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 600;
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                //En esta consulta se traen los nombres de las columnas
                string query = this.dataG.gColumnsName(nombre);

                adapter = new SqlDataAdapter(query, conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 600;
                dt = new DataSet();
                adapter.Fill(dt);
                DataTable table0 = dt.Tables[0];

                //Dim array ejex & ejey & serie & show & groupMath & type
                ejex = new string[table.Rows.Count];
                ejey = new string[table.Rows.Count];
                ejey2_title = new string[table.Rows.Count];
                serie = new string[table.Rows.Count];
                showString = new string[table.Rows.Count];
                groupMath = new string[table.Rows.Count];
                type = new string[table.Rows.Count];
                type2 = new string[table.Rows.Count];
                pareto = new string[table.Rows.Count];
                unidades = new string[table.Rows.Count];
                unidades2 = new string[table.Rows.Count];
                nombreType = new string[table.Rows.Count];

                /*Procesamiento de colores de las graficas*/
                string colorsString = table.Rows[n_graph]["colors"].ToString();
                string[] colors;
                if (colorsString.Contains("'"))
                {
                    colors = Regex.Split(table.Rows[n_graph]["colors"].ToString(), "',");
                    for (int h = 0; h < colors.Count(); h++)
                    {
                        colors[h] = colors[h].Substring(colors[h].IndexOf("'") + 1);
                    }
                }
                else
                {
                    colors = Regex.Split(table.Rows[n_graph]["colors"].ToString(), "\",");
                    for (int h = 0; h < colors.Count(); h++)
                    {
                        colors[h] = colors[h].Substring(colors[h].IndexOf("\"") + 1);
                    }
                }
                colors[colors.Count() - 1] = colors[colors.Count() - 1].Substring(0, colors[colors.Count() - 1].IndexOf("\""));
                resultado["COLORS"] = JArray.Parse(JsonConvert.SerializeObject(colors, Formatting.None));

                //Se extraen los nombres completos de las columnas ejex, ejey y  serie
                j = 0;
                foreach (DataRow c in table.Rows)
                {
                    ejex[j] = c["EJEX"].ToString();
                    ejey[j] = c["EJEY"].ToString();
                    ejey2_title[j] = c["tituloEjeY_2"].ToString();
                    serie[j] = c["SERIE"].ToString();
                    showString[j] = c["SHOW"].ToString();
                    groupMath[j] = c["groupBy"].ToString();
                    type[j] = c["type"].ToString();
                    type2[j] = c["type2"].ToString();
                    pareto[j] = c["pareto"].ToString();
                    unidades[j] = c["unidades"].ToString();
                    unidades2[j] = c["unidades_2"].ToString();
                    nombreType[j] = c["nombreType"].ToString();
                    j++;
                }

                //Se trae el WHERE de las consultas
                string queryWhere = this.dataG.gColumnsData(n_col, nombre, ejey[n_graph], replace, nvc);

                if (!(type[n_graph] == "pie" || type[n_graph] == "pieA"))
                {
                    DataTable table1 = null;
                    DataTable dimTable = null;
                    if (this.dataG.view != "")
                    {
                        //En esta consulta se traen las primeras 20 series que cumplen con el where
                        adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                            UPDATE {0}.graphicHistoric
                            SET sesion = @@SPID
                            WHERE id = {6}

                            DECLARE @cols NVARCHAR (MAX)
  
                            SELECT TOP {5}
                            @cols = COALESCE (@cols + ',[' + [{3}] + ']','[' + [{3}] + ']')
  
                            FROM
                            [{1}].[{2}]
                            --WHERE
                            {4}
                                GROUP BY [{1}].[{2}].[{3}]
                                ORDER BY [{1}].[{2}].[{3}]
  
                            SELECT @cols
  
                            SELECT COUNT(DISTINCT [{3}]) AS dim
                            FROM
                            [{1}].[{2}]
                            --WHERE
                            {4}
                        ", Abi_maestro.esquema, esquema, vista, serie[n_graph], queryWhere, tamaSeries, id_historic), conexion.getConexion());
                        adapter.SelectCommand.CommandTimeout = 600;
                        dt = new DataSet();
                        adapter.Fill(dt);
                        table1 = dt.Tables[0];
                        dimTable = dt.Tables[1];
                    }
                    else
                    {
                        //Se arma la tabla temporal
                        tempTable = this.dataG.queryBuilder(nombre);
                        //En esta consulta se traen las primeras 20 series que cumplen con el where
                        adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                            UPDATE {0}.graphicHistoric
                            SET sesion = @@SPID
                            WHERE id = {6}

                            --Tabla temporal
                            {3}

                            --Consulta creada por el usuario
                            {4}

                            DECLARE @cols NVARCHAR (MAX)
  
                            SELECT TOP {5}
                            @cols = COALESCE (@cols + ',[' + [{1}] + ']','[' + [{1}] + ']')
  
                            FROM
                            #consulta AS consulta
                            --WHERE
                            {2}
                                GROUP BY consulta.[{1}]
                                ORDER BY consulta.[{1}]
  
                            SELECT @cols
  
                            SELECT COUNT(DISTINCT [{1}]) AS dim
                            FROM
                            #consulta AS consulta
                            --WHERE
                            {2}

                            DROP TABLE #consulta;
                        ", Abi_maestro.esquema, serie[n_graph], queryWhere, tempTable, this.dataG.consulta, tamaSeries, id_historic),
                        conexion.getConexion());
                        adapter.SelectCommand.CommandTimeout = 600;
                        dt = new DataSet();
                        adapter.Fill(dt);
                        table1 = dt.Tables[0];
                        dimTable = dt.Tables[1];
                    }

                    //SE LIMPIA LA SESION
                    adapter = new SqlDataAdapter(string.Format(@"

	                    UPDATE {0}.graphicHistoric
	                    SET sesion = NULL
	                    WHERE id = {1}

                    ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    dim = Int32.Parse(dimTable.Rows[0][0].ToString());
                    //No hay datos si no cumple esta condicion
                    if (dim > 0)
                    {
                        //Series a pivotear
                        string pivotSeries = table1.Rows[0][0].ToString();

                        //Se trae el HAVING del pivot
                        string queryHaving = this.dataG.gColumnsDataEjey(n_col, nombre, ejey[n_graph], replace, nvc);

                        //Se reemplaza eje y por groupMath (ejey): ejemplo SUM([%#%EJEY%#%Columna])
                        queryHaving = queryHaving.Replace("[" + ejey[n_graph] + "]", groupMath[n_graph] + "([" + ejey[n_graph] + "])");

                        DataTable table2 = null;

                        if (this.dataG.view != "")
                        {
                            if (pareto[n_graph] == "True")
                            {
                                string paretoSeries = pivotSeries.Replace("[", "ISNULL([");
                                paretoSeries = paretoSeries.Replace(",", "+");
                                paretoSeries = paretoSeries.Replace("]", "], 0)");
                                paretoSeries = string.Concat(", ", paretoSeries, " AS Pareto");
                                //En esta consulta se traen las series que cumplen con el where
                                adapter = new SqlDataAdapter(string.Format(@"
                                    UPDATE {0}.graphicHistoric
                                    SET sesion = @@SPID
                                    WHERE id = {12}

                                    --------------------------------------------------------------------
                                    BEGIN
                                    SELECT [{3}], [{10}], {9} --eje x, show y pivotSeries
                                    FROM
                                    (
                                        SELECT *{11}
                                        FROM
                                            (
                                                SELECT
                                                    --EJEX EJEY Y SERIES
                                                    [{3}],
                                                    [{5}],
                                                    --AQUI SE PONE LA FUNCION CON LA CUAL SE AGRUPARA
                                                    {8}([{4}]) AS [{4}],
                                                    -- SHOW
                                                    [{10}] AS [%#%SHOW%#%]
  
                                                FROM
                                                    [{1}].[{2}] --db.schema.view
                                                --WHERE
                                                {6}
                                                --CONDICIONES
  
                                                --AQUI SE PONE EL AGRUPAMIENTO SIEMPRE POR EJEX Y SERIE
                                                GROUP BY [{3}], [{5}], [{10}]--SHOW
                              
                                                --HAVING
                                                    {7}
  
                                            )
                                        AS SOURCE
                                        PIVOT (
                                                --MAX NO INFLUYE EN NADA
                                                MAX (SOURCE.[{4}]) FOR SOURCE.[{5}]
                                        IN ({9})
                                            ) AS PV1
                                    )  AS mainPareto
                                    ORDER BY
                                    mainPareto.Pareto DESC
                                    END
                                ", Abi_maestro.esquema, esquema, vista, ejex[n_graph], ejey[n_graph], serie[n_graph], queryWhere, queryHaving,
                                groupMath[n_graph], pivotSeries, showString[n_graph], paretoSeries, id_historic),
                                conexion.getConexion());
                                adapter.SelectCommand.CommandTimeout = 600;
                                dt = new DataSet();
                                adapter.Fill(dt);
                                table2 = dt.Tables[0];
                            }
                            else
                            {
                                //En esta consulta se traen las series que cumplen con el where
                                adapter = new SqlDataAdapter(string.Format(@"
                                    UPDATE {0}.graphicHistoric
                                    SET sesion = @@SPID
                                    WHERE id = {11}

                                    --------------------------------------------------------------------
                                    BEGIN
                                    SELECT * FROM
                                        (
                                            SELECT
                                                --EJEX EJEY Y SERIES
                                                [{3}],
                                                [{5}],
                                                --AQUI SE PONE LA FUNCION CON LA CUAL SE AGRUPARA
                                                {8}([{4}]) AS [{4}],
                                                -- SHOW
                                                [{10}] AS [%#%SHOW%#%]
  
                                            FROM
                                                [{1}].[{2}] --db.schema.view
                                            --WHERE
                                            {3}
                                            --CONDICIONES
  
                                            --AQUI SE PONE EL AGRUPAMIENTO SIEMPRE POR EJEX Y SERIE
                                            GROUP BY [{3}], [{5}], [{10}]--SHOW
                              
                                            --HAVING
                                                {7}
  
                                        )
                                    AS SOURCE
                                    PIVOT (
                                            --MAX NO INFLUYE EN NADA
                                            MAX (SOURCE.[{4}]) FOR SOURCE.[{5}]
                                    IN ({9})
                                        ) AS PV1
                                    ORDER BY
                                    [{3}]
                                        END
                                ", Abi_maestro.esquema, esquema, vista, ejex[n_graph], ejey[n_graph], serie[n_graph], queryWhere, queryHaving,
                                groupMath[n_graph], pivotSeries, showString[n_graph], id_historic), conexion.getConexion());
                                adapter.SelectCommand.CommandTimeout = 600;
                                dt = new DataSet();
                                adapter.Fill(dt);
                                table2 = dt.Tables[0];
                            }
                        }
                        else
                        {
                            if (pareto[n_graph] == "True")
                            {
                                string paretoSeries = pivotSeries.Replace("[", "ISNULL([");
                                paretoSeries = paretoSeries.Replace(",", "+");
                                paretoSeries = paretoSeries.Replace("]", "], 0)");
                                paretoSeries = string.Concat(", ", paretoSeries, " AS Pareto");
                                //En esta consulta se traen las series que cumplen con el where para pareto
                                adapter = new SqlDataAdapter(string.Format(@"
                                    UPDATE {0}.graphicHistoric
                                    SET sesion = @@SPID
                                    WHERE id = {12}

                                    --------------------------------------------------------------------
                                    --Tabla temporal
                                    {9}

                                    --Consulta creada por el usuario
                                    {10}

                                    BEGIN
                                    SELECT [{1}], [{8}], {7} --eje x, show y series
                                    FROM
                                    (
                                        SELECT *{11}
                                        FROM
                                            (
                                                SELECT
                                                    --EJEX EJEY Y SERIES
                                                    [{1}],
                                                    [{3}],
                                                    --AQUI SE PONE LA FUNCION CON LA CUAL SE AGRUPARA
                                                    {6}([{2}]) AS [{2}],
                                                    -- SHOW
                                                    [{8}] AS [%#%SHOW%#%]
  
                                                FROM
                                                    #consulta AS consulta --db.schema.view
                                                --WHERE
                                                {4}
                                                --CONDICIONES
  
                                                --AQUI SE PONE EL AGRUPAMIENTO SIEMPRE POR EJEX Y SERIE
                                                GROUP BY [{1}], [{3}], [{8}]--SHOW
                              
                                                --HAVING
                                                    {5}
  
                                            )
                                        AS SOURCE
                                        PIVOT (
                                                --MAX NO INFLUYE EN NADA
                                                MAX (SOURCE.[{2}]) FOR SOURCE.[{3}]
                                        IN ({7})
                                            ) AS PV1
                                    )  AS mainPareto
                                    ORDER BY
                                    mainPareto.Pareto DESC
                                    END
                                ", Abi_maestro.esquema, ejex[n_graph], ejey[n_graph], serie[n_graph], queryWhere, queryHaving, groupMath[n_graph],
                                pivotSeries, showString[n_graph], tempTable, this.dataG.consulta, paretoSeries, id_historic),
                                conexion.getConexion());
                                adapter.SelectCommand.CommandTimeout = 600;
                                dt = new DataSet();
                                adapter.Fill(dt);
                                table2 = dt.Tables[0];
                            }
                            else
                            {
                                //En esta consulta se traen las series que cumplen con el where
                                adapter = new SqlDataAdapter(string.Format(@"
                                    UPDATE {0}.graphicHistoric
                                    SET sesion = @@SPID
                                    WHERE id = {11}

                                    --------------------------------------------------------------------
                                    --Tabla temporal
                                    {9}

                                    --Consulta creada por el usuario
                                    {10}

                                    BEGIN
                                    SELECT * FROM
                                        (
                                            SELECT
                                                --EJEX EJEY Y SERIES
                                                [{1}],
                                                [{3}],
                                                --AQUI SE PONE LA FUNCION CON LA CUAL SE AGRUPARA
                                                {6}([{2}]) AS [{2}],
                                                -- SHOW
                                                [{8}] AS [%#%SHOW%#%]
  
                                            FROM
                                                #consulta AS consulta --db.schema.view
                                            --WHERE
                                            {4}
                                            --CONDICIONES
  
                                            --AQUI SE PONE EL AGRUPAMIENTO SIEMPRE POR EJEX Y SERIE
                                            GROUP BY [{1}], [{3}], [{8}]--SHOW
                              
                                            --HAVING
                                                {5}
  
                                        )
                                    AS SOURCE
                                    PIVOT (
                                            --MAX NO INFLUYE EN NADA
                                            MAX (SOURCE.[{2}]) FOR SOURCE.[{3}]
                                    IN ({7})
                                        ) AS PV1
                                    ORDER BY
                                    [{1}]
                                        END
                                ", Abi_maestro.esquema, ejex[n_graph], ejey[n_graph], serie[n_graph], queryWhere, queryHaving, groupMath[n_graph],
                                pivotSeries, showString[n_graph], tempTable, this.dataG.consulta, id_historic), conexion.getConexion());
                                adapter.SelectCommand.CommandTimeout = 600;
                                dt = new DataSet();
                                adapter.Fill(dt);
                                table2 = dt.Tables[0];
                            }
                        }

                        //SE LIMPIA LA SESION
                        adapter = new SqlDataAdapter(string.Format(@"

	                        UPDATE {0}.graphicHistoric
	                        SET sesion = NULL
	                        WHERE id = {1}

                        ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                        adapter.SelectCommand.ExecuteScalar();

                        //Se consulta el type 2
                        DataTable table3 = null;
                        string seriesType2 = "";
                        if (type2[n_graph] != "0")
                        {
                            adapter = new SqlDataAdapter(string.Format(@"
                                --------------------------------------------------------------------
                                DECLARE @series NVARCHAR(255)
                                SELECT @series = COALESCE(@series + CONCAT('%#%', [{0}], '%#%'), CONCAT('%#%', [{0}], '%#%'))
                                FROM #consulta
                                WHERE [{1}] = '{2}'
                                GROUP BY [{0}]

                                SELECT @series 
                            ", serie[n_graph], nombreType[n_graph], ejey2_title[n_graph]), conexion.getConexion());
                            adapter.SelectCommand.CommandTimeout = 600;
                            dt = new DataSet();
                            adapter.Fill(dt);
                            table3 = dt.Tables[0];

                            seriesType2 = table3.Rows[0][0].ToString();
                        }

                        //Elimina las columnas que no contienen informacion
                        if (table2.Rows.Count > 0)
                        {
                            foreach (var column in table2.Columns.Cast<DataColumn>().ToArray())
                            {
                                if (table2.AsEnumerable().All(dr => dr.IsNull(column)))
                                    table2.Columns.Remove(column);
                            }
                        }

                        //SI LA TABLA NO TIENE DATOS PARA DIBUJAR NO ENTRA
                        if (table2.Columns.Count - 2 > 0 && table2.Rows.Count > 0)
                        {
                            //Se arma el objeto que recibe highcharts
                            //Se crea un vector de la clase lineChart que contendra todos los objetos
                            lineChart[] vecLineChart = new lineChart[table2.Columns.Count - 2];

                            //Arreglo string que contendra el eje x
                            ejeXOut = new string[table2.Rows.Count];

                            //Se llenan el vector con los objetos que contienen los nombres y el vector data
                            //recorre la serie
                            i = 0;
                            int a = 0;
                            if (type2[n_graph] == "0")
                            {
                                foreach (DataColumn dc in table2.Columns)
                                {

                                    if ((i == 0))              //Extraigo el eje x 1 sola vez
                                    {
                                        j = 0;
                                        foreach (DataRow dtRow in table2.Rows)
                                        {
                                            ejeXOut[j] = dtRow[dc].ToString();
                                            j++;
                                        }
                                    }
                                    if ((i == 1))                //Extraigo el show 1 sola vez
                                    {
                                        j = 0;
                                        l = 0;
                                        foreach (DataRow dtRow in table2.Rows)
                                        {
                                            if (Int32.Parse(dtRow[dc].ToString()) == 1)
                                            {
                                                l++;                                            //Se averigua que dimension exacta debe tener el arreglo
                                            }
                                            j++;
                                        }
                                        //Se define el arreglo que contendra el show(encargado de mostrar puntos en el ejex)
                                        show = new int[l];
                                        j = 0;
                                        l = 0;
                                        foreach (DataRow dtRow in table2.Rows)
                                        {
                                            if (Int32.Parse(dtRow[dc].ToString()) == 1)
                                            {
                                                show[l] = j;
                                                l++;
                                            }
                                            j++;
                                        }
                                    }
                                    else if ((i != 0) && (i != 1))                              //Extraigo las series
                                    {
                                        lineChart objLineChart = new lineChart();               //Se instancia un objeto lineChart
                                        objLineChart.data = new float?[table2.Rows.Count];      //Se define el tamaño del vector interno data
                                        objLineChart.name = table2.Columns[i].ToString();       //Aqui se deben poner los nombnres del JSON
                                        Tooltip tootlip = new Tooltip();
                                        tootlip.valueSuffix = unidades[n_graph];
                                        objLineChart.tooltip = tootlip;                         //Unidades del tooltip ej: mm, $, etc
                                        objLineChart.yAxis = 0;
                                        j = 0;
                                        foreach (DataRow dtRow in table2.Rows)                  //Se recorre el vector interno
                                        {
                                            if (!(dtRow[dc].ToString() == ""))                  //IsNull
                                            {
                                                objLineChart.data[j] = float.Parse(dtRow[dc].ToString());
                                            }
                                            j++;
                                        }
                                        vecLineChart[i - 2] = objLineChart;
                                    }
                                    i++;
                                }
                            }
                            else
                            {
                                //Series normales
                                foreach (DataColumn dc in table2.Columns)
                                {

                                    if ((i == 0))              //Extraigo el eje x 1 sola vez
                                    {
                                        j = 0;
                                        foreach (DataRow dtRow in table2.Rows)
                                        {
                                            ejeXOut[j] = dtRow[dc].ToString();
                                            j++;
                                        }
                                    }
                                    if ((i == 1))                //Extraigo el show 1 sola vez
                                    {
                                        j = 0;
                                        l = 0;
                                        foreach (DataRow dtRow in table2.Rows)
                                        {
                                            if (Int32.Parse(dtRow[dc].ToString()) == 1)
                                            {
                                                l++;                                            //Se averigua que dimension exacta debe tener el arreglo
                                            }
                                            j++;
                                        }
                                        //Se define el arreglo que contendra el show(encargado de mostrar puntos en el ejex)
                                        show = new int[l];
                                        j = 0;
                                        l = 0;
                                        foreach (DataRow dtRow in table2.Rows)
                                        {
                                            if (Int32.Parse(dtRow[dc].ToString()) == 1)
                                            {
                                                show[l] = j;
                                                l++;
                                            }
                                            j++;
                                        }
                                    }
                                    else if ((i != 0) && (i != 1) //Extraigo las series
                                    && seriesType2.IndexOf("%#%" + table2.Columns[i].ToString() + "%#%") == -1)
                                    {
                                        lineChart objLineChart = new lineChart();               //Se instancia un objeto lineChart
                                        objLineChart.data = new float?[table2.Rows.Count];      //Se define el tamaño del vector interno data
                                        objLineChart.name = table2.Columns[i].ToString();       //Aqui se deben poner los nombnres del JSON
                                        Tooltip tootlip = new Tooltip();
                                        tootlip.valueSuffix = unidades[n_graph];
                                        objLineChart.tooltip = tootlip;                         //Unidades del tooltip ej: mm, $, etc
                                        objLineChart.yAxis = 0;
                                        j = 0;
                                        foreach (DataRow dtRow in table2.Rows)                  //Se recorre el vector interno
                                        {
                                            if (!(dtRow[dc].ToString() == ""))                  //IsNull
                                            {
                                                objLineChart.data[j] = float.Parse(dtRow[dc].ToString());
                                            }
                                            j++;
                                        }
                                        vecLineChart[a] = objLineChart;
                                        a++;
                                    }
                                    i++;
                                }
                                //Series tipo2
                                i = 0;
                                foreach (DataColumn dc in table2.Columns)
                                {
                                    if ((i != 0) && (i != 1) //Extraigo las series
                                    && seriesType2.IndexOf("%#%" + table2.Columns[i].ToString() + "%#%") != -1)
                                    {
                                        lineChart objLineChart = new lineChart();               //Se instancia un objeto lineChart
                                        objLineChart.data = new float?[table2.Rows.Count];      //Se define el tamaño del vector interno data
                                        objLineChart.name = table2.Columns[i].ToString();       //Aqui se deben poner los nombnres del JSON
                                        objLineChart.type = type2[n_graph];                     //Type2
                                        Tooltip tootlip = new Tooltip();
                                        tootlip.valueSuffix = unidades2[n_graph];
                                        objLineChart.tooltip = tootlip;                         //Unidades del tooltip ej: mm, $, etc
                                        objLineChart.yAxis = 1;
                                        j = 0;
                                        foreach (DataRow dtRow in table2.Rows)                  //Se recorre el vector interno
                                        {
                                            if (!(dtRow[dc].ToString() == ""))                  //IsNull
                                            {
                                                objLineChart.data[j] = float.Parse(dtRow[dc].ToString());
                                            }
                                            j++;
                                        }
                                        vecLineChart[a] = objLineChart;
                                        a++;
                                    }
                                    i++;
                                }
                            }

                            resultado["EJEX"] = JArray.Parse(JsonConvert.SerializeObject(ejeXOut, Formatting.None));         //ejeX
                            resultado["SHOW"] = JArray.Parse(JsonConvert.SerializeObject(show, Formatting.None));            //show
                            resultado["DATA"] = JArray.Parse(JsonConvert.SerializeObject(vecLineChart, Formatting.None));    //Jarray HighCharts
                        }
                        else
                        {
                            dim = 0;
                        }
                    }
                }
                else
                {
                    DataTable table1 = null;
                    DataTable dimTable = null;
                    if (this.dataG.view != "")
                    {
                        //En esta consulta se traen las primeras 20 series que cumplen con el where
                        adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                            UPDATE {0}.graphicHistoric
                            SET sesion = @@SPID
                            WHERE id = {8}

                            SELECT TOP {7} [{5}] AS name, {4}([{3}]) AS y--SERIE, groupMath (ejey)
                            FROM
                                [{1}].[{2}] --db.schema.view
                                --WHERE
                            {6}--CONDICIONES
                            --AQUI SE PONE EL AGRUPAMIENTO SIEMPRE POR SERIE
                            GROUP BY
                                [{5}]
  
                            SELECT COUNT(DISTINCT [{5}]) AS dim
                            FROM
                            [{1}].[{2}] --db.schema.view
                            --WHERE
                            {6}--CONDICIONES
                        ", Abi_maestro.esquema, esquema, vista, ejey[n_graph], groupMath[n_graph], serie[n_graph], queryWhere, tamaSeries, id_historic),
                        conexion.getConexion());
                        adapter.SelectCommand.CommandTimeout = 600;
                        dt = new DataSet();
                        adapter.Fill(dt);
                        table1 = dt.Tables[0];
                        dimTable = dt.Tables[1];
                    }
                    else
                    {
                        //Se arma la tabla temporal
                        tempTable = this.dataG.queryBuilder(nombre);
                        //En esta consulta se traen las primeras 20 series que cumplen con el where
                        adapter = new SqlDataAdapter(string.Format(CultureInfo.InvariantCulture, @"
                            UPDATE {0}.graphicHistoric
                            SET sesion = @@SPID
                            WHERE id = {8}

                            --Tabla temporal
                            {5}

                            --Consulta creada por el usuario
                            {6}

                            SELECT TOP {7} [{3}] AS name, {2}([{1}]) AS y--SERIE, groupMath (ejey)
                            FROM
                                #consulta AS consulta --db.schema.view
                                --WHERE
                            {4}--CONDICIONES
                            --AQUI SE PONE EL AGRUPAMIENTO SIEMPRE POR SERIE
                            GROUP BY
                                [{3}]
  
                            SELECT COUNT(DISTINCT [{3}]) AS dim
                            FROM
                            #consulta AS consulta --db.schema.view
                            --WHERE
                            {4}--CONDICIONES
                        ", Abi_maestro.esquema, ejey[n_graph], groupMath[n_graph], serie[n_graph], queryWhere, tempTable, this.dataG.consulta, tamaSeries, id_historic),
                        conexion.getConexion());
                        adapter.SelectCommand.CommandTimeout = 600;
                        dt = new DataSet();
                        adapter.Fill(dt);
                        table1 = dt.Tables[0];
                        dimTable = dt.Tables[1];
                    }

                    //SE LIMPIA LA SESION
                    adapter = new SqlDataAdapter(string.Format(@"

	                    UPDATE {0}.graphicHistoric
	                    SET sesion = NULL
	                    WHERE id = {1}

                    ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    dim = Int32.Parse(dimTable.Rows[0][0].ToString());
                    //No hay datos si no cumple esta condicion
                    if (dim > 0)
                    {
                        //Se arma el objeto que recibe highcharts
                        //Se crea un vector de la clase pieChart que contendra todos los objetos
                        pieChart[] vecPieChart = new pieChart[table1.Rows.Count];

                        i = 0;
                        foreach (DataRow dtRow in table1.Rows)
                        {
                            pieChart objPieChart = new pieChart();                      //objeto pieChart
                            objPieChart.name = dtRow["name"].ToString();
                            if (dtRow["y"].ToString() == "")
                            {
                                objPieChart.y = 0;
                            }
                            else
                            {
                                objPieChart.y = float.Parse(dtRow["y"].ToString());
                            }
                            vecPieChart[i] = objPieChart;
                            i++;
                        }
                        resultado["DATA"] = JArray.Parse(JsonConvert.SerializeObject(vecPieChart, Formatting.None));    //Jarray HighCharts
                    }
                }
                resultado["DIM"] = dim;              //Dimensión total de las series
                //SE ACTUALIZA EL ACTIVO Y SE GUARDA EL JSON
                adapter = new SqlDataAdapter(string.Format(@"

                    UPDATE {0}.graphicHistoric
                    SET activo = 0, json = @json
                    WHERE id = {1}

                ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@json", resultado.ToString());
                adapter.SelectCommand.ExecuteScalar();
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                if (e.Message.ToString() != "Subproceso anulado." && e.Message.ToString() != "No se puede continuar la ejecución porque la sesión está en estado de eliminación.\r\nError grave en el comando actual. Los resultados, si los hay, se deben descartar.")
                {
                    //SE ACTIVA LA VARIABLE DE ERROR Y SE APAGA ACTIVO
                    adapter = new SqlDataAdapter(string.Format(@"

                        UPDATE {0}.graphicHistoric
                        SET activo = 0, error = 1, abortar = 1
                        WHERE id = {1}

                    ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();

                    Mail.SendEmail(e, host, string.Format(@"Nombre grafica: {0}", nombre));
                }
                conexion.closeConexion();
            }
        }
    }

    [WebMethod(EnableSession = true)]
    public string period_graph(int id_historic)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE CONSULTA EL ESTADO Y EL ERROR
                adapter = new SqlDataAdapter(string.Format(@"

                    SELECT
	                    id,
	                    id_uniqueGraph,
	                    fecha,
	                    json,
	                    activo,
	                    error
                    FROM {0}.graphicHistoric
                    WHERE id = {1}

                ", Abi_maestro.esquema, id_historic), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable historic = dt.Tables[0];

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(historic, Formatting.None));
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
    public string abort_graph(int id_historic)
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                //SE ENCIENDE LA BANDERA QUE INDICA QUE EL HILO DEBE SER ANULADO
                adapter = new SqlDataAdapter(string.Format(@"

                    UPDATE {0}.graphicHistoric
                    SET abortar = 1
                    WHERE id = {1}

                ", Abi_maestro.esquema, id_historic), conexion.getConexion());
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
    /*******************************************************************************************************/

    /****************************************************************************/
    /******************************* DATATABLES *********************************/
    /****************************************************************************/


    /*data, el cual devuelve los datos paginados, ordenados, filtrados*/
    /*data_column_names, el cual devuelve las columnas a visualizar*/
    /*data_column_filters, el cual llena los selectores de los filtros*/
    /*la instacia de dtQueryStrings es la que construye cada consulta desde la tabla de configuración ubicada en dicho esquema*/

    private dtQueryStrings dataQ = new dtQueryStrings(Abi_maestro.esquema);

    [WebMethod(EnableSession = true)]
    public string data_column_names(string nombre)
    {
        /*Construcción de la consulta*/
        string query = this.dataQ.qColumnsName(nombre);

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(query, conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 90;
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
    public string data_column_filters(string col_name, string cols, string col, string nombre, string replace)
    {
        string queryFilter = this.dataQ.qColumnsFilter(col_name, cols, col, nombre, replace);

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(queryFilter, conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 90;
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTableCollection tables = dt.Tables;
                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(tables[0], Formatting.None));

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
    public string data(int draw, int start, int length, int n_col, string nombre, string replace)
    {
        string queryData = this.dataQ.qColumnsData(draw, start, length, n_col, nombre, replace);
        JObject result = new JObject();
        int recordsTotal;
        int recordsFiltered;
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
                DataTable table = dt.Tables[0];
                recordsTotal = (int)dt.Tables[1].Rows[0][0];
                recordsFiltered = (int)dt.Tables[2].Rows[0][0];
                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = JArray.Parse(JsonConvert.SerializeObject(table, Formatting.None));
                result["draw"] = draw;
                result["recordsTotal"] = recordsTotal;
                result["recordsFiltered"] = recordsFiltered;
                conexion.closeConexion();
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
                string host = HttpContext.Current.Request.Url.Host;
                Mail.SendEmail(e, host, String.Format(@"nombre: {0}", nombre));
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
    public string aditional_data(string nombre)
    {
        string queryAditionalData = this.dataQ.qAditionalData(nombre);
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(queryAditionalData, conexion.getConexion());
                adapter.SelectCommand.CommandTimeout = 90;
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
    public string excelDataTables()
    {
        int draw = int.Parse(HttpContext.Current.Request.Form.Get("draw"));
        int start = int.Parse(HttpContext.Current.Request.Form.Get("start"));
        int length = int.Parse(HttpContext.Current.Request.Form.Get("length"));
        int n_col = int.Parse(HttpContext.Current.Request.Form.Get("n_col"));
        string nombre = HttpContext.Current.Request.Form.Get("nombre");
        string name_pag = HttpContext.Current.Request.Form.Get("name_pag");
        string replace = HttpContext.Current.Request.Form.Get("replace");
        string columns = "";
        for (int i = 0; i < n_col; i++)
        {
            columns += String.Format(",'{0}'", HttpContext.Current.Request.Form.Get("columns[" + i.ToString() + "][data]"));
        }

        string queryData = this.dataQ.qColumnsDataExcel(draw, start, length, n_col, nombre, replace);

        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    
                    INSERT INTO {0}.D_Registro_Excel
                        (Fecha, Nombre, Estado, Fallo)
                    VALUES
                    (GETDATE(), 'excelDataTables', 1, 0);

                    SELECT IDENT_CURRENT('{0}.D_Registro_Excel') AS id

                ", Abi_maestro.esquema), conexion.getConexion());
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                int id_table = int.Parse(table.Rows[0][0].ToString());
                string host = HttpContext.Current.Request.Url.Host;

                // Se  hace el llamado sincrono a los dos metodos en APS
                Thread hilo1 = new Thread(() => this.dataExcel(id_table, host, queryData, start, length, name_pag, nombre, columns));
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
            result["MENSAJE"] = "ERROR DE LA CONEXIÓN";
            conexion.closeConexion();
        }
        Context.Response.Output.Write(result);
        Context.Response.End();
        return result.ToString();
    }

    public void dataExcel(int id_table, string host, string queryData, int start, int length, string name_pag, string nombre, string columns)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(queryData, conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@inicio", start);
                adapter.SelectCommand.Parameters.AddWithValue("@tam", length);
                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable excelDatos = dt.Tables[0];

                string queryTypes_Columns = this.Types_Columns(nombre, columns);
                adapter = new SqlDataAdapter(queryTypes_Columns, conexion.getConexion());
                DataSet dt_TC = new DataSet();
                adapter.Fill(dt_TC);
                DataTable Types_Columns = dt_TC.Tables[0];

                //Se pone la ruta base sobre donde se guardara el excel y la ruta del logo
                string pathBase = Server.MapPath("~/RegistroTablas/");
                string NombreArchivo = string.Format("{0}_{1}.xlsx", name_pag, DateTime.Now.ToString("yyyy-MM-dd_HH.mm.ss"));

                FileInfo destFile = new FileInfo(pathBase + NombreArchivo);


                using (ExcelPackage registros = new ExcelPackage())
                {
                    Array.ForEach(Directory.GetFiles(pathBase), File.Delete);


                    //modificar cuando sea otra empresa
                    registros.Workbook.Properties.Title = "Central Cervecera de Colombia S.A.S.";
                    registros.Workbook.Properties.Author = "OptiPlant Consultores";
                    registros.Workbook.Properties.Comments = "Registros de menus";
                    registros.Workbook.Properties.Company = "OptiPlant Consultores";
                    registros.Workbook.Properties.AppVersion = "1.0";
                    registros.Workbook.Properties.LastModifiedBy = "OptiPlant Consultores";
                    registros.Workbook.Properties.Modified = DateTime.Now;

                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Registro de inversión y mantenimientos
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
                    // Se agregra la hoja de inversión al libro de registros
                    /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

                    //string por si no hay coincidencias en las consultas
                    string no_match = "No se encontraron resultados";
                    int aux = 0;

                    //Se verifica que la consulta contenga información
                    if (excelDatos.Rows.Count == 0)
                    {
                        aux = 1;
                    }

                    //Se crea la hoja
                    ExcelWorksheet reg_Excel = registros.Workbook.Worksheets.Add(name_pag);

                    reg_Excel.View.ShowGridLines = false;
                    reg_Excel.Cells.Style.Font.SetFromFont(new Font("Arial", 9, FontStyle.Regular));

                    //Se define el aalto de todas las filas y columnas
                    reg_Excel.DefaultRowHeight = 15;
                    reg_Excel.Row(1).Height = 45;

                    reg_Excel.Cells["A1"].Value = "#";

                    string TypeData;
                    string data_;

                    for (int i = 1; i < excelDatos.Columns.Count; i++)
                    {
                        reg_Excel.Cells[1, (i + 1)].Value = excelDatos.Columns[i].ToString();

                        TypeData = Types_Columns.Rows[i]["type"].ToString().ToLower();
                        switch (TypeData)
                        {
                            case "date":
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "dd/MM/yyyy";
                                break;

                            case "datetime":
                            case "datetime2":
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "dd/MM/yyyy HH:mm:ss";
                                break;

                            case "time":
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "HH:mm:ss";
                                break;

                            case "int":
                            case "decimal":
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "#,##0";
                                break;

                            case "bit":
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "#";
                                break;

                            case "float":
                            case "real":
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "#,##0.00";
                                break;

                            default:
                                reg_Excel.Cells[1, (i + 1), (excelDatos.Rows.Count + 1), (i + 1)].Style.Numberformat.Format = "@";
                                break;
                        }
                    }

                    //Se define el estilo, tipo y tamaño de la letra, se centra y se le agrega un fondo al encabezado
                    reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Font.SetFromFont(new Font("Arial", 9, FontStyle.Bold));
                    reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                    reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                    reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                    reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.WrapText = true;

                    //Comparación por si no encuentra resultados
                    if (aux == 0)
                    {
                        reg_Excel.Cells[1, 1, (1 + excelDatos.Rows.Count), excelDatos.Columns.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[1, 1, (1 + excelDatos.Rows.Count), excelDatos.Columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[1, 1, (1 + excelDatos.Rows.Count), excelDatos.Columns.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[1, 1, (1 + excelDatos.Rows.Count), excelDatos.Columns.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        reg_Excel.Cells[2, 1, (1 + excelDatos.Rows.Count), excelDatos.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        reg_Excel.Cells[2, 1, (1 + excelDatos.Rows.Count), excelDatos.Columns.Count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                        reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].AutoFilter = true;
                        float auxfloat = 0;
                        for (int j = 0; j < excelDatos.Rows.Count; j++)
                        {
                            reg_Excel.Cells[(j + 2), 1].Value = (j + 1);
                            for (int i = 1; i < excelDatos.Columns.Count; i++)
                            {
                                TypeData = Types_Columns.Rows[i]["type"].ToString().ToLower();
                                data_ = excelDatos.Rows[j][i].ToString();

                                if(data_ != "")
                                {
                                    switch (TypeData)
                                    {
                                        case "date":
                                        case "datetime":
                                        case "datetime2":
                                            reg_Excel.Cells[(j + 2), (i + 1)].Value = DateTime.Parse(data_);
                                            break;

                                        case "time":
                                            reg_Excel.Cells[(j + 2), (i + 1)].Value = data_;
                                            break;

                                        case "int":
                                        case "bit":
                                            reg_Excel.Cells[(j + 2), (i + 1)].Value = int.Parse(data_);
                                            break;

                                        case "float":
                                        case "decimal":
                                        case "real":
                                            reg_Excel.Cells[(j + 2), (i + 1)].Value = Math.Round( float.Parse(data_),2);
                                            break;

                                        default:
                                            reg_Excel.Cells[(j + 2), (i + 1)].Value = data_;
                                            break;
                                    }
                                }
                                else
                                {
                                    reg_Excel.Cells[(j + 2), (i + 1)].Value = data_;
                                }
                            }

                            if (j % 2 == 0)
                            {
                                reg_Excel.Cells[(j + 2), 1, (j + 2), excelDatos.Columns.Count].Style.Fill.PatternType = ExcelFillStyle.Solid;
                                reg_Excel.Cells[(j + 2), 1, (j + 2), excelDatos.Columns.Count].Style.Fill.BackgroundColor.SetColor(Color.FromArgb(191, 191, 191));
                            }
                        }

                        reg_Excel.View.FreezePanes(2, 1);
                    }
                    else
                    {

                        reg_Excel.Cells["A2"].Value = no_match;
                        //reg_Excel.Cells[2, 1, 2, excelDatos.Columns.Count].Merge = true;

                        reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Border.Top.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[1, 1, 1, excelDatos.Columns.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;

                        reg_Excel.Cells[2, 1].Style.Border.Left.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[2, excelDatos.Columns.Count].Style.Border.Right.Style = ExcelBorderStyle.Thin;
                        reg_Excel.Cells[2, 1, 2, excelDatos.Columns.Count].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                        reg_Excel.Cells[2, 1, 2, excelDatos.Columns.Count].Style.HorizontalAlignment = ExcelHorizontalAlignment.CenterContinuous;
                        reg_Excel.Cells[2, 1, 2, excelDatos.Columns.Count].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                    }

                    //Se define el ancho de las columnas
                    reg_Excel.Cells.AutoFitColumns();

                    //reg_Excel.Cells.auto

                    reg_Excel.View.ZoomScale = 100;

                    registros.SaveAs(destFile);

                    adapter = new SqlDataAdapter(String.Format(@"
                    
                        UPDATE {0}.D_Registro_Excel
                            SET Nombre = '{1}', Estado = 0
                        WHERE
                            id = {2}

                    ", Abi_maestro.esquema, NombreArchivo, id_table), conexion.getConexion());
                    adapter.SelectCommand.ExecuteScalar();
                    conexion.closeConexion();
                }
            }
            catch (Exception e)
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    
                    UPDATE {0}.D_Registro_Excel
                        SET Fallo = 1
                    WHERE
                        id = {1}

                ",Abi_maestro.esquema, id_table), conexion.getConexion());
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

    public string Types_Columns(string nombre, string columns)
    {
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();

        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {
                adapter = new SqlDataAdapter(String.Format(@"
                    Begin Transaction;

                    SELECT
                        id,
                        esquema,
                        vista
                    FROM
                        {0}.dataTableConfig
                    WHERE
                        nombre = @nombre;

                    Commit Transaction;
                ", Abi_maestro.esquema), conexion.getConexion());
                adapter.SelectCommand.Parameters.AddWithValue("@nombre", nombre);

                DataSet dt = new DataSet();
                adapter.Fill(dt);
                DataTable table = dt.Tables[0];

                string id = table.Rows[0]["id"].ToString();
                string schema = table.Rows[0]["esquema"].ToString();
                string view = table.Rows[0]["vista"].ToString();

                string query;

                if (view != "")
                {
                    
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
                            AND COLUMN_NAME IN ('DT_RowId'{2})

                    ", schema, view, columns);

                    conexion.closeConexion();
                }
                else
                {
                    //SE EXTRAEN LAS COLUMNAS Y EL TYPE DE LA TABLA dataTableColumnsName
                    query = String.Format(@"
                        -- Por el momento solo se necesita el nombre de las columnas

                        SELECT data, type
                        FROM {0}.dataTableColumnsName
                        WHERE id_dataTableConfig = {1} AND data IN ('DT_RowId'{2})
                        ORDER BY orden ASC

                    ", schema, id, columns);

                    conexion.closeConexion();
                }
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

    /****************************************************************************/
    /******************************* subir archivos *****************************/
    /****************************************************************************/
    /* CAPTURA DE ARCHIVO DEL CLIENTE AL SERVIDOR, GUARDO LA RUTA EN BD Y EL ARCHIVO EN SERVER*/
    [WebMethod(EnableSession = true)]
    public string load_files()
    {
        JObject result = new JObject();
        ConexionSQL conexion = new ConexionSQL();
        SqlDataAdapter adapter = new SqlDataAdapter();
        string pathBase;
        string name;
        DateTime FechaGen = DateTime.Now;
        if ((conexion.openConexion()) == "TRUE")
        {
            try
            {

                //int id_usuario = int.Parse(HttpContext.Current.Request.Form["id_usuario"]);
                string carpeta = HttpContext.Current.Request.Form["carpeta"].ToString();
                string ext = HttpContext.Current.Request.Form["ext"].ToString();

                name = HttpContext.Current.Request.Form["nombre"].ToString();

                string path = string.Format(@"~/{0}/{1}.{2}", carpeta, name, ext);
                HttpPostedFile file = HttpContext.Current.Request.Files.Get(0);
                //SE MAPEA LA RUTA DONDE SE GUARDARA EL ARCHIVO
                pathBase = HttpContext.Current.Server.MapPath(path);
                //SE BORRA EL ARCHIVO SI YA EXISTE
                if (File.Exists(pathBase))
                {
                    File.Delete(pathBase);
                }

                file.SaveAs(pathBase);

                conexion.closeConexion();

                result["ESTADO"] = "TRUE";
                result["MENSAJE"] = "Consulta Correcta.";
                result["RESULTADO"] = pathBase;
            }
            catch (Exception e)
            {
                result["ESTADO"] = "FALSE";
                result["MENSAJE"] = "ERROR";
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
            result["MENSAJE"] = "Error en la conexion:" + conexion.openConexion();
            conexion.closeConexion();
        }

        Context.Response.Output.Write(result);
        Context.Response.End();

        return result.ToString();
    }
}