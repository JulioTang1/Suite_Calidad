using Android.Content;
using Android.Gms.Maps.Model;
using Android.Preferences;
using Java.Util;
using Microsoft.Data.Sqlite;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PCLStorage;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace APP.Helpers
{
    public static class DB
    {
        private static Context context = global::Android.App.Application.Context;

        /*CONEXION A BASE DE DATOS*/
        public static SqliteConnection GetConnection()
        {
            IFolder folder = FileSystem.Current.LocalStorage;
            string path = PortablePath.Combine(folder.Path.ToString(), "banasan_sql.db");
            SqliteConnection connection = new SqliteConnection("Data Source=" + path);
            return connection;
        }

        /*SE CREA TODA LA ESTRUCTURA DE LA BASE DE DATOS SI NO EXISTE*/
        public static async Task CreateDatabase()
        {
            SqliteConnection connection = GetConnection();
            try
            {            
                connection.Open();
                SqliteCommand commandCreate = new SqliteCommand(@"

                    -- ----------------------------
                    -- Table structure for departamentos
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS departamentos (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server  INTEGER,
                    nombre  TEXT
                    );

                    -- ----------------------------
                    -- Table structure for ciudades
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS ciudades (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server  INTEGER,
                    id_server_departamento  INTEGER,
                    nombre  TEXT
                    );

                    -- ----------------------------
                    -- Table structure for sectores
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS sectores (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server  INTEGER,
                    nombre  TEXT
                    );

                    -- ----------------------------
                    -- Table structure for fincas
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS fincas (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server  INTEGER,
                    id_server_ciudad  INTEGER,
                    nombre  TEXT,
                    id_server_sector  INTEGER,
                    activo  INTEGER
                    );

                    -- ----------------------------
                    -- Table structure for usuarios
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS usuarios (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server  INTEGER,
                    usuario  TEXT,
                    pass  TEXT,
                    nombres  TEXT,
                    apellido  TEXT,
                    activo  INTEGER
                    );

                    -- ----------------------------
                    -- Table structure for visitas
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS visitas (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server_finca  INTEGER,
                    id_server_usuario  INTEGER,
                    id_server_tipo_visita  INTEGER,
                    temperatura_minima  REAL,
                    temperatura  REAL,
                    temperatura_maxima  REAL,
                    humedad_relativa  REAL,
                    fecha  TEXT,
                    estado_sync  INTEGER,
                    fecha_sync  TEXT
                    );

                    -- ----------------------------
                    -- Table structure for recorrido_visita
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS recorrido_visita (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_visita  INTEGER,
                    latitud  REAL,
                    longitud  REAL,
                    fecha  TEXT,
                    procesamiento  INTEGER,
                    FOREIGN KEY (id_visita) REFERENCES visitas (id)
                    );

                    -- ----------------------------
                    -- Table structure for punto_captura
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS punto_captura (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_visita  INTEGER,
                    id_server_tipo  INTEGER,
                    fecha  TEXT,
                    FOREIGN KEY (id_visita) REFERENCES visitas (id)
                    );

                    -- ----------------------------
                    -- Table structure for planta_punto_captura
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS planta_punto_captura (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_punto_captura  INTEGER,
                    id_server_edad  INTEGER,
                    latitud  REAL,
                    longitud  REAL,
                    fecha  TEXT,
                    FOREIGN KEY (id_punto_captura) REFERENCES punto_captura (id)
                    );

                    -- ----------------------------
                    -- Table structure for indicadores_punto_captura
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS indicadores_punto_captura (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_planta_punto_captura  INTEGER,
                    id_server_indicador  INTEGER,
                    valor  REAL,
                    valor_texto  TEXT,
                    FOREIGN KEY (id_planta_punto_captura) REFERENCES planta_punto_captura (id)
                    );

                    -- ----------------------------
                    -- Table structure for fotos_indicador
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS fotos_indicador (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_lectura_indicador  INTEGER,
                    url  TEXT,
                    estado_sync  INTEGER,
                    fecha_sync  TEXT,
                    FOREIGN KEY (id_lectura_indicador) REFERENCES indicadores_punto_captura (id)
                    );

                    -- ----------------------------
                    -- Table structure for precipitaciones
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS precipitaciones (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server_finca  INTEGER,
                    id_server_usuario  INTEGER,
                    fecha  TEXT,
                    valor  REAL,
                    estado_sync  INTEGER,
                    fecha_sync  TEXT
                    );

                    -- ----------------------------
                    -- Table structure for sincronizaciones
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS sincronizaciones (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server_usuario  INTEGER,
                    fecha  TEXT
                    );

                    -- ----------------------------
                    -- Table structure for errores
                    -- ----------------------------
                    CREATE TABLE IF NOT EXISTS errores (
                    id  INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    id_server_usuario INTEGER,
                    id_server_servicio  INTEGER,
                    fecha TEXT,
                    mensaje TEXT,
                    estado_sync INTEGER,
                    fecha_sync TEXT
                    );

                ", connection);
                commandCreate.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la carga de la base de datos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*VALIDACION DE LOGIN*/
        public static async Task Login(string usuario, string contraseña)
        {
            SqliteConnection connection = DB.GetConnection();
            
            //cifrado OPC
            ClaseCifrado cifrado = new ClaseCifrado();
            contraseña = cifrado.cifrar(contraseña);
            
            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"
                
                    SELECT
                        id,
                        id_server
                    FROM usuarios
                    WHERE usuario = @usuario
                    AND pass = @contraseña
                    AND activo = 1
                
                ", connection);
                command.Parameters.AddWithValue("@usuario", usuario);
                command.Parameters.AddWithValue("@contraseña", contraseña);
                var reader = command.ExecuteReader();
                var id_server = reader.GetOrdinal("id_server");
                while (reader.Read())
                {
                    //Se enciende bandera para indicar que se esta capturando un recorrido
                    ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                    ISharedPreferencesEditor editor = prefs.Edit();
                    editor.PutInt("idUsuario", int.Parse(reader.GetString(id_server)));
                    editor.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CARGA LA LISTA DE DEPARTAMENTOS*/
        public static async Task LoadDepartamentos(ObservableCollection<Selectores> selectores, int allFincas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
                        D.id_server
                        ,D.nombre
                    FROM
                    (
	                    SELECT
		                    CASE
			                    WHEN @all_fincas = 1 THEN F.id_server
			                    ELSE
				                    CASE
					                    WHEN F.activo = 0 THEN NULL
					                    ELSE F.id_server
				                    END
		                    END AS id_server
		                    ,F.nombre
		                    ,F.activo
                            ,F.id_server_ciudad
	                    FROM fincas AS F
	                    INNER JOIN ciudades AS C
	                    ON F.id_server_ciudad = C.id_server
                    ) AS S
                    INNER JOIN ciudades AS C
                    ON S.id_server_ciudad = C.id_server
                    INNER JOIN departamentos AS D
                    ON C.id_server_departamento = D.id_server
                    WHERE S.id_server IS NOT NULL
                    GROUP BY
                        D.id_server
                        ,D.nombre

                ", connection);
                command.Parameters.AddWithValue("@all_fincas", allFincas);
                var reader = command.ExecuteReader();
                var id_server = reader.GetOrdinal("id_server");
                var nombre = reader.GetOrdinal("nombre");
                while (reader.Read())
                {
                    selectores.Add(new Selectores(int.Parse(reader.GetString(id_server)), reader.GetString(nombre)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CARGA LA LISTA DE MUNICIPIOS*/
        public static async Task LoadMunicipios(ObservableCollection<Selectores> selectores, int idDepartamento, int allFincas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
                        C.id_server
                        ,C.nombre
                    FROM
                    (
	                    SELECT
		                    CASE
			                    WHEN @all_fincas = 1 THEN F.id_server
			                    ELSE
				                    CASE
					                    WHEN F.activo = 0 THEN NULL
					                    ELSE F.id_server
				                    END
		                    END AS id_server
		                    ,F.nombre
		                    ,F.activo
                            ,F.id_server_ciudad
	                    FROM fincas AS F
	                    INNER JOIN ciudades AS C
	                    ON F.id_server_ciudad = C.id_server
                    ) AS S
                    INNER JOIN ciudades AS C
                    ON S.id_server_ciudad = C.id_server
                    WHERE S.id_server IS NOT NULL
                    AND (C.id_server_departamento = @id_departamento OR 0 = @id_departamento)
                    GROUP BY
                        C.id_server
                        ,C.nombre

                ", connection);
                command.Parameters.AddWithValue("@id_departamento", idDepartamento);
                command.Parameters.AddWithValue("@all_fincas", allFincas);
                var reader = command.ExecuteReader();
                var id_server = reader.GetOrdinal("id_server");
                var nombre = reader.GetOrdinal("nombre");
                while (reader.Read())
                {
                    selectores.Add(new Selectores(int.Parse(reader.GetString(id_server)), reader.GetString(nombre)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CARGA LA LISTA DE FINCAS*/
        public static async Task LoadFincas(ObservableCollection<Selectores> selectores, int idMunicipio, int allFincas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id_server,
	                    nombre,
	                    activo
                    FROM
                    (
	                    SELECT
		                    CASE
			                    WHEN @all_fincas = 1 THEN F.id_server
			                    ELSE
				                    CASE
					                    WHEN F.activo = 0 THEN NULL
					                    ELSE F.id_server
				                    END
		                    END AS id_server
		                    ,F.nombre
		                    ,F.activo
	                    FROM fincas AS F
	                    INNER JOIN ciudades AS C
	                    ON F.id_server_ciudad = C.id_server
	                    AND (C.id_server = @id_municipio OR 0 = @id_municipio)
                    ) AS S
                    WHERE id_server IS NOT NULL

                ", connection);
                command.Parameters.AddWithValue("@id_municipio", idMunicipio);
                command.Parameters.AddWithValue("@all_fincas", allFincas);
                var reader = command.ExecuteReader();
                var id_server = reader.GetOrdinal("id_server");
                var nombre = reader.GetOrdinal("nombre");
                var activo = reader.GetOrdinal("activo");
                while (reader.Read())
                {
                    selectores.Add(new Selectores(int.Parse(reader.GetString(id_server)), reader.GetString(nombre), int.Parse(reader.GetString(activo))));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA EL SECTOR DE UNA FINCA*/
        public static async Task LoadSector(int idFinca, ObservableCollection<string> sectorFinca)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT S.nombre AS sector
                    FROM sectores AS S
                    INNER JOIN fincas AS F
                    ON S.id_server = F.id_server_sector
                    WHERE F.id_server = @id_finca

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                var reader = command.ExecuteReader();
                var sector = reader.GetOrdinal("sector");
                while (reader.Read())
                {
                    sectorFinca.Add(reader.GetString(sector));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA LAS PRECIPITACIONES DE UNA FINCA*/
        public static async Task LoadPrecipitacion(int idFinca, ObservableCollection<string> precipitacionObj)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT valor AS precipitacion
                    FROM precipitaciones
                    WHERE id_server_finca = @id_finca
                    AND DATE(fecha) = DATE('now', '-1 day','localtime')

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                var reader = command.ExecuteReader();
                var precipitacion = reader.GetOrdinal("precipitacion");
                while (reader.Read())
                {
                    precipitacionObj.Add(reader.GetString(precipitacion));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA UNA VISITA*/
        public static async Task SaveVisit(int idFinca, float precipitacion, float temperaturaMinima, float temperatura, float temperaturaMaxima, float humedad, ObservableCollection<int> idVisita)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Se lee el id del usuario
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                int idUsuario = prefs.GetInt("idUsuario", 0);

                SqliteCommand command = new SqliteCommand(@"

				    UPDATE precipitaciones
					    SET id_server_usuario = @id_usuario,
					    fecha = DATETIME('now', '-1 day','localtime'),
					    valor = @precipitacion,
                        estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion
				    AND id_server_finca = @id_finca
				    AND DATE(fecha) = DATETIME('now', '-1 day','localtime');

                    INSERT INTO precipitaciones (id_server_finca, id_server_usuario, fecha, valor, estado_sync)
                    SELECT
	                    P.id_server_finca,
	                    P.id_server_usuario,
	                    P.fecha,
	                    P.valor,
                        2 AS estado_sync --ESTADO PENDIENTE
                    FROM
                    (
	                    SELECT
		                    @id_finca AS id_server_finca,
		                    @id_usuario AS id_server_usuario,
		                    DATETIME('now', '-1 day','localtime') AS  fecha,
		                    @precipitacion AS valor
                    ) AS P
                    LEFT JOIN precipitaciones AS PP
                    ON P.id_server_finca = PP.id_server_finca
                    AND DATETIME('now', '-1 day','localtime') = DATE(PP.fecha)
                    WHERE PP.id IS NULL;

                    INSERT INTO visitas (id_server_finca, id_server_usuario, id_server_tipo_visita, temperatura_minima, temperatura, temperatura_maxima, humedad_relativa, fecha, estado_sync)
                    VALUES (@id_finca, @id_usuario, 1, @temperatura_minima, @temperatura, @temperatura_maxima, @humedad, DATETIME('now','localtime'), 1);

                    SELECT LAST_INSERT_ROWID() AS id_visita;

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                command.Parameters.AddWithValue("@id_usuario", idUsuario);
                command.Parameters.AddWithValue("@precipitacion", precipitacion.ToString(CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@temperatura_minima", temperaturaMinima.ToString(CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@temperatura", temperatura.ToString(CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@temperatura_maxima", temperaturaMaxima.ToString(CultureInfo.InvariantCulture));
                command.Parameters.AddWithValue("@humedad", humedad.ToString(CultureInfo.InvariantCulture));
                var reader = command.ExecuteReader();
                var id_visita = reader.GetOrdinal("id_visita");
                while (reader.Read())
                {
                    idVisita.Add(int.Parse(reader.GetString(id_visita)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*ELIMINA LA VISITA EN EJECUCION Y SUS RELACIONES YA QUE SE CANCELA AL IR ATRAS*/
        public static async Task DeleteVisit(int idVisita)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Borrado de fotos guardadas en almacenamiento interno de la aplicacion
                SqliteCommand command = new SqliteCommand(@"

                    SELECT FI.url
                    FROM punto_captura AS PC
                    INNER JOIN planta_punto_captura AS PPC
                    ON PC.id = PPC.id_punto_captura
                    INNER JOIN indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta_punto_captura
                    INNER JOIN fotos_indicador AS FI
                    ON IPC.id = FI.id_lectura_indicador
                    WHERE PC.id_visita = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var url = reader.GetOrdinal("url");
                while (reader.Read())
                {
                    System.IO.File.Delete(reader.GetString(url));
                }

                //Borrado de url de fotos de indicadores
                command = new SqliteCommand(@"

                    DELETE
                    FROM fotos_indicador
                    WHERE id_lectura_indicador IN
                    (
                        SELECT IPC.id
                        FROM punto_captura AS PC
                        INNER JOIN planta_punto_captura AS PPC
                        ON PC.id = PPC.id_punto_captura
                        INNER JOIN indicadores_punto_captura AS IPC
                        ON PPC.id = IPC.id_planta_punto_captura
                        WHERE PC.id_visita = @id_visita
                    )

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Borrado de indicadores de plantas
                command = new SqliteCommand(@"

                    DELETE
                    FROM indicadores_punto_captura
                    WHERE id_planta_punto_captura IN
                    (
                        SELECT PPC.id
                        FROM punto_captura AS PC
                        INNER JOIN planta_punto_captura AS PPC
                        ON PC.id = PPC.id_punto_captura
                        WHERE PC.id_visita = @id_visita
                    )

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Borrado de plantas en el punto captura
                command = new SqliteCommand(@"

                    DELETE
                    FROM planta_punto_captura
                    WHERE id_punto_captura IN
                    (
                        SELECT id
                        FROM punto_captura
                        WHERE id_visita = @id_visita
                    )

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Borrado de puntos captura de la visita
                command = new SqliteCommand(@"

                    DELETE
                    FROM punto_captura
                    WHERE id_visita = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Borrado de puntos recorridos en la visita
                command = new SqliteCommand(@"

                    DELETE
                    FROM recorrido_visita
                    WHERE id_visita = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Borrado de visita
                command = new SqliteCommand(@"

                    DELETE
                    FROM visitas
                    WHERE id = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA UN PUNTO CAPTURA*/
        public static async Task SavePunto(int idVisita, int idServerTipo, ObservableCollection<int> idCaptura)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    INSERT INTO punto_captura (id_visita, id_server_tipo, fecha)
                    VALUES (@id_visita, @id_server_tipo, DATETIME('now','localtime'));
                    SELECT LAST_INSERT_ROWID() AS id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.Parameters.AddWithValue("@id_server_tipo", idServerTipo);
                var reader = command.ExecuteReader();
                var id_punto_captura = reader.GetOrdinal("id_punto_captura");
                while (reader.Read())
                {
                    idCaptura.Add(int.Parse(reader.GetString(id_punto_captura)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA UN PUNTO CAPTURA*/
        public static async Task SavePunto(int idVisita, int idServerTipo, ObservableCollection<int> idCaptura, int orden)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    INSERT INTO punto_captura (id_visita, id_server_tipo, fecha)
                    VALUES (@id_visita, @id_server_tipo, DATETIME('now','localtime', '" + orden + @" second'));
                    SELECT LAST_INSERT_ROWID() AS id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.Parameters.AddWithValue("@id_server_tipo", idServerTipo);
                var reader = command.ExecuteReader();
                var id_punto_captura = reader.GetOrdinal("id_punto_captura");
                while (reader.Read())
                {
                    idCaptura.Add(int.Parse(reader.GetString(id_punto_captura)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA EXITENCIA DE PUNTOS CAPTURA POR VISITA*/
        public static async Task ExistPoint(int idVisita, ObservableCollection<int> existPoint)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT COUNT(*) AS registros
                    FROM punto_captura
                    WHERE id_visita = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var registros = reader.GetOrdinal("registros");
                while (reader.Read())
                {
                    if (int.Parse(reader.GetString(registros)) == 0)
                    {
                        existPoint.Add(0);
                    }
                    else
                    {
                        existPoint.Add(1);
                    }                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA EXITENCIA DE PLANTAS REGISTRADAS POR PUNTOS CAPTURA*/
        public static async Task ExistPlanta(int idCaptura, ObservableCollection<int> existPlanta)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT COUNT(*) AS registros
                    FROM planta_punto_captura
                    WHERE id_punto_captura = @id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                var reader = command.ExecuteReader();
                var registros = reader.GetOrdinal("registros");
                while (reader.Read())
                {
                    if (int.Parse(reader.GetString(registros)) == 0)
                    {
                        existPlanta.Add(0);
                    }
                    else
                    {
                        existPlanta.Add(1);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*ELIMINA EL PUNTO DE CAPTURA Y SUS RELACIONES YA QUE SE CANCELAN AL IR ATRAS*/
        public static async Task DeletePunto(int idCaptura)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Borrado de indicadores de plantas
                SqliteCommand command = new SqliteCommand(@"

                    DELETE
                    FROM indicadores_punto_captura
                    WHERE id_planta_punto_captura IN
                    (
	                    SELECT id
	                    FROM planta_punto_captura
	                    WHERE id_punto_captura = @id_punto_captura
                    )

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                command.ExecuteNonQuery();

                //Borrado de plantas en el punto captura
                command = new SqliteCommand(@"

	                DELETE
	                FROM planta_punto_captura
	                WHERE id_punto_captura = @id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                command.ExecuteNonQuery();

                //Borrado de punto captura
                command = new SqliteCommand(@"

                    DELETE
                    FROM punto_captura
                    WHERE id = @id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                command.ExecuteNonQuery();


            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA UNA PLANTA Y SUS INDICADORES*/
        public static async Task SavePlantaIndicadores(int idCaptura, int idServerEdad, string latitud, string longitud, Indicadores[] mediciones)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Almacenamiento de planta en el punto de captura
                SqliteCommand command = new SqliteCommand(@"

                    INSERT INTO planta_punto_captura (id_punto_captura, id_server_edad, latitud, longitud, fecha)
                    VALUES (@id_punto_captura, @id_server_edad, @latitud, @longitud, DATETIME('now','localtime'));
                    SELECT LAST_INSERT_ROWID() AS id_planta_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                command.Parameters.AddWithValue("@id_server_edad", idServerEdad);
                command.Parameters.AddWithValue("@latitud", latitud);
                command.Parameters.AddWithValue("@longitud", longitud);
                var reader = command.ExecuteReader();

                var id_planta_punto_captura = reader.GetOrdinal("id_planta_punto_captura");
                while (reader.Read())
                {
                    //Almacenamiento de id con el que se guardo la planta
                    id_planta_punto_captura = int.Parse(reader.GetString(id_planta_punto_captura));
                }

                //Insercion de indicadores de la planta en el punto de captura
                for (int i = 0; i < mediciones.Length; i++)
                {
                    if(mediciones[i].id == 51 || mediciones[i].id == 52 || mediciones[i].id == 53)
                    {
                        command = new SqliteCommand(@"

                            INSERT INTO indicadores_punto_captura (id_planta_punto_captura, id_server_indicador, valor_texto)
                            VALUES (@id_planta_punto_captura, @id_server_indicador, @valor);
                            SELECT LAST_INSERT_ROWID() AS id_indicadores_punto_captura

                        ", connection);
                        command.Parameters.AddWithValue("@id_planta_punto_captura", id_planta_punto_captura);
                        command.Parameters.AddWithValue("@id_server_indicador", mediciones[i].id);
                        command.Parameters.AddWithValue("@valor", mediciones[i].valorText);
                        reader = command.ExecuteReader();
                    }
                    else
                    {
                        command = new SqliteCommand(@"

                            INSERT INTO indicadores_punto_captura (id_planta_punto_captura, id_server_indicador, valor)
                            VALUES (@id_planta_punto_captura, @id_server_indicador, @valor);
                            SELECT LAST_INSERT_ROWID() AS id_indicadores_punto_captura

                        ", connection);
                        command.Parameters.AddWithValue("@id_planta_punto_captura", id_planta_punto_captura);
                        command.Parameters.AddWithValue("@id_server_indicador", mediciones[i].id);
                        command.Parameters.AddWithValue("@valor", mediciones[i].valor.ToString(CultureInfo.InvariantCulture));
                        reader = command.ExecuteReader();
                    }

                    if(mediciones[i].id == 12 || mediciones[i].id == 13)
                    {
                        var id_indicadores_punto_captura = reader.GetOrdinal("id_indicadores_punto_captura");
                        while (reader.Read())
                        {
                            //Almacenamiento de id con el que se guardo el indicador que requiere imagen
                            id_indicadores_punto_captura = int.Parse(reader.GetString(id_indicadores_punto_captura));
                        }

                        //Insercion de url local de las fotos del indicador
                        for (int j = 0; j < mediciones[i].paths.Length; j++)
                        {
                            //Solo guarda url validas
                            if (mediciones[i].paths[j].Length > 0)
                            {
                                command = new SqliteCommand(@"

                                    INSERT INTO fotos_indicador (id_lectura_indicador, url, estado_sync)
                                    VALUES (@id_indicadores_punto_captura, @url, 1);

                                ", connection);
                                command.Parameters.AddWithValue("@id_indicadores_punto_captura", id_indicadores_punto_captura);
                                command.Parameters.AddWithValue("@url", mediciones[i].paths[j]);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA CUANTAS PLANTAS HAY REGISTRADAS EN UN PUNTO DE CAPTURA SIGATOKA*/
        public static async Task CountEdades(int idCaptura, ObservableCollection<string> plantasEdad)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    BASE.id,
	                    COALESCE(DATA.cantidad, BASE.cantidad) AS cantidad
                    FROM
                    (
	                    SELECT 1 AS id, 0 AS cantidad
	                    UNION
	                    SELECT 2 AS id, 0 AS cantidad
	                    UNION
	                    SELECT 3 AS id, 0 AS cantidad
	                    UNION
	                    SELECT 6 AS id, 0 AS cantidad
                    ) AS BASE
                    LEFT JOIN
                    (
	                    SELECT
		                    id_server_edad AS id,
		                    COUNT(*) AS cantidad
	                    FROM planta_punto_captura
	                    WHERE id_punto_captura = @id_punto_captura
	                    GROUP BY
		                    id_server_edad
                    ) AS DATA
                    ON BASE.id = DATA.id
                    ORDER BY
	                    BASE.id ASC

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                var reader = command.ExecuteReader();
                var cantidad = reader.GetOrdinal("cantidad");
                while (reader.Read())
                {
                    plantasEdad.Add(reader.GetString(cantidad));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA CUANTOS PUNTOS CAPTURAS HAY EN UNA VISITA*/
        public static async Task CountEnfermedades(int idVisita, ObservableCollection<PuntoLectura> puntoLectura)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
                        S.id,
	                    S.punto,
	                    S.fecha
                    FROM
                    (
                        SELECT
	                        id,
	                        'E. Vasculares ' || CAST((SELECT COUNT(*) FROM punto_captura AS C WHERE BASE.id >= C.id AND C.id_visita = @id_visita AND C.id_server_tipo = 2) AS TEXT) AS punto,
	                        fecha
                        FROM punto_captura AS BASE
                        WHERE id_visita = @id_visita
                        AND id_server_tipo = 2

                        UNION

                        SELECT
	                        id,
	                        'C. Culturales ' || CAST((SELECT COUNT(*) FROM punto_captura AS C WHERE BASE.id >= C.id AND C.id_visita = @id_visita AND C.id_server_tipo = 3) AS TEXT) AS punto,
	                        fecha
                        FROM punto_captura AS BASE
                        WHERE id_visita = @id_visita
                        AND id_server_tipo = 3
                    ) AS S
                    ORDER BY
	                    S.fecha DESC

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var punto = reader.GetOrdinal("punto");
                var fecha = reader.GetOrdinal("fecha");
                while (reader.Read())
                {
                    puntoLectura.Add(new PuntoLectura(int.Parse(reader.GetString(id)), reader.GetString(punto), reader.GetString(fecha)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*ELIMINA UN PUNTO DE CAPTURA*/
        public static async Task DeleteEnfermedad(int id)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Borrado de URL de fotos
                SqliteCommand command = new SqliteCommand(@"

                    DELETE
                    FROM fotos_indicador
                    WHERE id_lectura_indicador IN
                    (
                        SELECT IPC.id
                        FROM indicadores_punto_captura AS IPC
                        INNER JOIN planta_punto_captura AS PPC
                        ON IPC.id_planta_punto_captura = PPC.id
                        AND PPC.id_punto_captura = @id_punto_captura
                    )

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", id);
                command.ExecuteNonQuery();

                //Borrado de indicadores de plantas
                command = new SqliteCommand(@"

                    DELETE
                    FROM indicadores_punto_captura
                    WHERE id_planta_punto_captura IN
                    (
	                    SELECT id
	                    FROM planta_punto_captura
	                    WHERE id_punto_captura = @id_punto_captura
                    )

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", id);
                command.ExecuteNonQuery();

                //Borrado de plantas en el punto captura
                command = new SqliteCommand(@"

	                DELETE
	                FROM planta_punto_captura
	                WHERE id_punto_captura = @id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", id);
                command.ExecuteNonQuery();

                //Borrado de punto captura
                command = new SqliteCommand(@"

                    DELETE
                    FROM punto_captura
                    WHERE id = @id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", id);
                command.ExecuteNonQuery();


            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA CUANTOS RECORRIDOS HAY PARA UNA FINCA*/
        public static async Task CountRecorridos(int idFinca, string fecha, string fecha_fin, ObservableCollection<Visitas> visitas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id,
	                    'Visita ' || CAST(ROW_NUMBER() OVER(PARTITION BY id_server_finca, DATE(fecha) ORDER BY fecha) AS TEXT) AS visita,
	                    fecha
                    FROM visitas AS BASE
                    WHERE id_server_finca = @id_finca
                    AND DATE(fecha) BETWEEN @fecha AND @fecha_fin
                    AND id_server_tipo_visita = 1
                    ORDER BY
	                    fecha DESC

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@fecha_fin", fecha_fin);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var nombre = reader.GetOrdinal("visita");
                var fecha_visita = reader.GetOrdinal("fecha");
                while (reader.Read())
                {
                    visitas.Add(new Visitas(int.Parse(reader.GetString(id)), reader.GetString(nombre), reader.GetString(fecha_visita)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA PRECIPITACION Y TEMPERATURA PARA UNA VISITA*/
        public static async Task BringIndicadoresVisita(int idVisita, ObservableCollection<string> indicadores)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT valor AS dato
                    FROM precipitaciones AS P
                    INNER JOIN visitas AS V
                    ON P.id_server_finca = V.id_server_finca
                    AND DATE(P.fecha) = DATE('now', '-1 day','localtime')
                    WHERE V.id = @id_visita

                    UNION ALL

                    SELECT temperatura_minima AS dato
                    FROM visitas
                    WHERE id = @id_visita

                    UNION ALL

                    SELECT temperatura AS dato
                    FROM visitas
                    WHERE id = @id_visita

                    UNION ALL

                    SELECT temperatura_maxima AS dato
                    FROM visitas
                    WHERE id = @id_visita

                    UNION ALL

                    SELECT humedad_relativa AS dato
                    FROM visitas
                    WHERE id = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var dato = reader.GetOrdinal("dato");
                while (reader.Read())
                {
                    indicadores.Add(reader.GetString(dato));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA CUANTAS LECTURAS HAY EN UNA VISITA*/
        public static async Task CountLecturas(int idVisita, ObservableCollection<Visitas> lecturas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
                        S.id,
                        S.tipo,
                        S.fecha
                    FROM
                    (
                        SELECT
	                        PC.id,
	                        CASE
		                        WHEN PC.id_server_tipo = 2 THEN 'E. Vasculares'
		                        ELSE 'C. Culturales'
	                        END AS tipo,
	                        PC.fecha,
                            PC.fecha AS orden,
                            MIN(PPC.id) AS id_ppc
                        FROM punto_captura AS PC
                        INNER JOIN planta_punto_captura AS PPC
                        ON PC.id = PPC.id_punto_captura
                        WHERE PC.id_visita = @id_visita
                        AND PC.id_server_tipo <> 1
                        GROUP BY
	                        PC.id,
	                        CASE
		                        WHEN PC.id_server_tipo = 2 THEN 'E. Vasculares'
		                        ELSE 'C. Culturales'
	                        END,
	                        PC.fecha

                        UNION

                        SELECT
	                        PC.id,
	                        'Sigatoka - Parcela Fija' AS tipo,
	                        ROW_NUMBER() OVER(ORDER BY PC.fecha) AS fecha,
                            PC.fecha AS orden,
                            MIN(PPC.id) AS id_ppc
                        FROM punto_captura AS PC
                        LEFT JOIN planta_punto_captura AS PPC
                        ON PC.id = PPC.id_punto_captura
                        WHERE PC.id_visita = @id_visita
                        AND PC.id_server_tipo = 1
                        GROUP BY
	                        PC.id,
	                        PC.fecha
                    ) AS S
                    WHERE S.id_ppc IS NOT NULL
                    ORDER BY
	                    S.orden DESC

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var nombre = reader.GetOrdinal("tipo");
                var fecha_lectura = reader.GetOrdinal("fecha");
                while (reader.Read())
                {
                    lecturas.Add(new Visitas(int.Parse(reader.GetString(id)), reader.GetString(nombre), reader.GetString(fecha_lectura)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA INDICADORES PARA E VASCULARES Y C CULTURALES*/
        public static async Task BringIndicadores(int idLectura, ObservableCollection<Indicador> indicador)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    IPC.id,
	                    CASE
		                    WHEN IPC.id_server_indicador = 1 THEN 'HF'
		                    WHEN IPC.id_server_indicador = 2 THEN 'YLS'
		                    WHEN IPC.id_server_indicador = 4 THEN 'TH'
		                    WHEN IPC.id_server_indicador = 5 THEN 'YLI'
		                    WHEN IPC.id_server_indicador = 7 THEN 'CF'
		                    WHEN IPC.id_server_indicador = 8 THEN 'EFA'
		                    WHEN IPC.id_server_indicador = 9 THEN 'Fusarium'
		                    WHEN IPC.id_server_indicador = 10 THEN 'Moko'
		                    WHEN IPC.id_server_indicador = 11 THEN 'Erwinia'
		                    WHEN IPC.id_server_indicador = 12 THEN 'NF'
		                    WHEN IPC.id_server_indicador = 13 THEN 'FIT'
                            WHEN IPC.id_server_indicador = 48 THEN 'H2'
                            WHEN IPC.id_server_indicador = 49 THEN 'H3'
                            WHEN IPC.id_server_indicador = 50 THEN 'H4'
		                    ELSE 'RTI'
	                    END AS nombre,
	                    CASE
		                    WHEN IPC.id_server_indicador = 48 OR IPC.id_server_indicador = 49 OR IPC.id_server_indicador = 50 THEN
			                    CASE
				                    WHEN IPC.valor = 1 THEN '1+'
				                    WHEN IPC.valor = 2 THEN '1-'
				                    WHEN IPC.valor = 3 THEN '2+'
				                    WHEN IPC.valor = 4 THEN '2-'
				                    WHEN IPC.valor = 5 THEN '3+'
				                    ELSE '3-'
			                    END
		                    WHEN IPC.id_server_indicador = 9 OR IPC.id_server_indicador = 10 OR IPC.id_server_indicador = 11 THEN
			                    CASE
                                    WHEN IPC.valor = 4 THEN 'Sospechosa'
				                    WHEN IPC.valor = 1 THEN 'Ausencia'
				                    WHEN IPC.valor = 2 THEN 'Presencia tratada'
				                    ELSE 'Presencia sin tratar'
			                    END
		                    WHEN IPC.id_server_indicador = 13 OR IPC.id_server_indicador = 14 THEN
			                    CASE
                                    WHEN IPC.valor = 1 THEN 'Si'
				                    WHEN IPC.valor = 2 THEN 'No'
                                    ELSE ''
			                    END
		                    ELSE IPC.valor
	                    END AS valor,
	                    PPC.fecha,
	                    CASE
		                    WHEN IPC.id_server_indicador = 7 OR IPC.id_server_indicador = 12 OR IPC.id_server_indicador = 13 THEN 1
		                    ELSE 0
	                    END AS boton,
                        IPC.id_server_indicador AS id_indicador
                    FROM planta_punto_captura AS PPC
                    INNER JOIN indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta_punto_captura
                    WHERE PPC.id_punto_captura = @id_punto_captura

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idLectura);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var nombre = reader.GetOrdinal("nombre");
                var valor = reader.GetOrdinal("valor");
                var fecha = reader.GetOrdinal("fecha");
                var boton = reader.GetOrdinal("boton");
                var id_indicador = reader.GetOrdinal("id_indicador");
                while (reader.Read())
                {
                    indicador.Add(new Indicador(int.Parse(reader.GetString(id)), reader.GetString(nombre), reader.GetString(valor), reader.GetString(fecha), int.Parse(reader.GetString(boton)), int.Parse(reader.GetString(id_indicador))));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CARGA LA LISTA DE PLANTAS REGISTRADAS EN UNA EDAD*/
        public static async Task LoadPlantasEdad(int idCaptura, int idServerEdad, ObservableCollection<Selectores> selectores)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id,
	                    fecha AS nombre
                    FROM planta_punto_captura
                    WHERE id_punto_captura = @id_punto_captura
                    AND id_server_edad = @id_server_edad
                    ORDER BY
	                    fecha DESC

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idCaptura);
                command.Parameters.AddWithValue("@id_server_edad", idServerEdad);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var nombre = reader.GetOrdinal("nombre");
                while (reader.Read())
                {
                    selectores.Add(new Selectores(int.Parse(reader.GetString(id)), reader.GetString(nombre)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA INDICADORES PARA SIGATOKA*/
        public static async Task BringIndicadoresPlanta(int idPlanta, ObservableCollection<Indicador> indicador)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    IPC.id,
	                    CASE
		                    WHEN IPC.id_server_indicador = 1 THEN 'HF'
		                    WHEN IPC.id_server_indicador = 2 THEN 'YLS'
		                    WHEN IPC.id_server_indicador = 4 THEN 'TH'
		                    WHEN IPC.id_server_indicador = 5 THEN 'YLI'
		                    WHEN IPC.id_server_indicador = 7 THEN 'CF'
		                    WHEN IPC.id_server_indicador = 8 THEN 'EFA'
		                    WHEN IPC.id_server_indicador = 9 THEN 'Fusarium'
		                    WHEN IPC.id_server_indicador = 10 THEN 'Moko'
		                    WHEN IPC.id_server_indicador = 11 THEN 'Erwinia'
		                    WHEN IPC.id_server_indicador = 12 THEN 'NF'
		                    WHEN IPC.id_server_indicador = 13 THEN 'FIT'
                            WHEN IPC.id_server_indicador = 48 THEN 'H2'
                            WHEN IPC.id_server_indicador = 49 THEN 'H3'
                            WHEN IPC.id_server_indicador = 50 THEN 'H4'
		                    ELSE 'RTI'
	                    END AS nombre,
	                    CASE
		                    WHEN IPC.id_server_indicador = 48 OR IPC.id_server_indicador = 49 OR IPC.id_server_indicador = 50 THEN
			                    CASE
				                    WHEN IPC.valor = 1 THEN '1+'
				                    WHEN IPC.valor = 2 THEN '1-'
				                    WHEN IPC.valor = 3 THEN '2+'
				                    WHEN IPC.valor = 4 THEN '2-'
				                    WHEN IPC.valor = 5 THEN '3+'
                                    WHEN IPC.valor = 6 THEN '3-'
				                    ELSE ''
			                    END
		                    WHEN IPC.id_server_indicador = 9 OR IPC.id_server_indicador = 10 OR IPC.id_server_indicador = 11 THEN
			                    CASE
                                    WHEN IPC.valor = 4 THEN 'Sospechosa'
				                    WHEN IPC.valor = 1 THEN 'Ausencia'
				                    WHEN IPC.valor = 2 THEN 'Presencia tratada'
				                    ELSE 'Presencia sin tratar'
			                    END
		                    WHEN IPC.id_server_indicador = 13 OR IPC.id_server_indicador = 14 THEN
			                    CASE
				                    WHEN IPC.valor = 1 THEN 'Si'
                                    WHEN IPC.valor = 2 THEN 'No'
				                    ELSE ''
			                    END
		                    ELSE IPC.valor
	                    END AS valor,
	                    PPC.fecha,
	                    CASE
		                    WHEN IPC.id_server_indicador = 7 OR IPC.id_server_indicador = 12 OR IPC.id_server_indicador = 13 THEN 1
		                    ELSE 0
	                    END AS boton,
                        IPC.id_server_indicador AS id_indicador
                    FROM planta_punto_captura AS PPC
                    INNER JOIN indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta_punto_captura
                    WHERE PPC.id = @id

                ", connection);
                command.Parameters.AddWithValue("@id", idPlanta);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var nombre = reader.GetOrdinal("nombre");
                var valor = reader.GetOrdinal("valor");
                var fecha = reader.GetOrdinal("fecha");
                var boton = reader.GetOrdinal("boton");
                var id_indicador = reader.GetOrdinal("id_indicador");
                while (reader.Read())
                {
                    indicador.Add(new Indicador(int.Parse(reader.GetString(id)), reader.GetString(nombre), reader.GetString(valor), reader.GetString(fecha), int.Parse(reader.GetString(boton)), int.Parse(reader.GetString(id_indicador))));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA LAS UBICACIONES TOMADAS PERIODICAMENTE EN UNA VISITA*/
        public static async Task SaveLocation(int idVisita, string latitud, string longitud)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Almacenamiento ubicacion
                SqliteCommand command = new SqliteCommand(@"

                    INSERT INTO recorrido_visita (id_visita, latitud, longitud, fecha)
                    VALUES (@id_visita, @latitud, @longitud, DATETIME('now','localtime'));

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.Parameters.AddWithValue("@latitud", latitud);
                command.Parameters.AddWithValue("@longitud", longitud);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA LAS UBICACIONES TOMADAS PERIODICAMENTE EN UNA VISITA*/
        public static async Task BringLocation(int idVisita, ObservableCollection<ArrayList> coordenadas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                ArrayList routeList = new ArrayList();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    latitud,
	                    longitud                        
                    FROM recorrido_visita
                    WHERE id_visita = @id_visita
                    ORDER BY
	                    fecha

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var latitud = reader.GetOrdinal("latitud");
                var longitud = reader.GetOrdinal("longitud");
                while (reader.Read())
                {
                    routeList.Add(new LatLng(float.Parse(reader.GetString(latitud), CultureInfo.InvariantCulture), float.Parse(reader.GetString(longitud), CultureInfo.InvariantCulture)));
                }

                coordenadas.Add(routeList);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA LAS UBICACIONES DE PLANTAS EN UN PUNTO LECTURA ESPECIFICO*/
        public static async Task BringLocationPlanta(int idLectura, ObservableCollection<LatLng> puntosLecturas, ObservableCollection<string> tipo)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    CASE
		                    WHEN PC.id_server_tipo = 2 THEN 'E. Vasculares ' || TIME(PPC.fecha)
		                    WHEN PC.id_server_tipo = 3 THEN 'C. Culturales ' || TIME(PPC.fecha)
		                    ELSE
			                    CASE
				                    WHEN PPC.id_server_edad = 1 THEN '10 Semanas ' || TIME(PPC.fecha)
				                    WHEN PPC.id_server_edad = 2 THEN '7 Semanas ' || TIME(PPC.fecha)
				                    WHEN PPC.id_server_edad = 3 THEN '0 Semanas ' || TIME(PPC.fecha)
				                    WHEN PPC.id_server_edad = 4 THEN 'H3 ' || TIME(PPC.fecha)
				                    WHEN PPC.id_server_edad = 5 THEN 'H4 ' || TIME(PPC.fecha)
				                    ELSE 'Planta Joven ' || TIME(PPC.fecha)
			                    END
	                    END AS planta,
	                    PPC.latitud,
	                    PPC.longitud
                    FROM punto_captura AS PC
                    INNER JOIN planta_punto_captura AS PPC
                    ON PC.id = PPC.id_punto_captura
                    WHERE PC.id = @id_punto_captura
                    ORDER BY
	                    PPC.fecha ASC

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", idLectura);
                var reader = command.ExecuteReader();
                var planta = reader.GetOrdinal("planta");
                var latitud = reader.GetOrdinal("latitud");
                var longitud = reader.GetOrdinal("longitud");
                while (reader.Read())
                {
                    tipo.Add(reader.GetString(planta));
                    puntosLecturas.Add(new LatLng(float.Parse(reader.GetString(latitud), CultureInfo.InvariantCulture), float.Parse(reader.GetString(longitud), CultureInfo.InvariantCulture)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA LAS FOTOS TOMADAS PARA UN INDICADOR*/
        public static async Task BringFoto(int idIndicador, ObservableCollection<String[]> pathsObj)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                int contador = 0;
                String[] paths = { "", "", "" };

                SqliteCommand command = new SqliteCommand(@"

                    SELECT url
                    FROM fotos_indicador
                    WHERE id_lectura_indicador = @id_lectura_indicador

                ", connection);
                command.Parameters.AddWithValue("@id_lectura_indicador", idIndicador);
                var reader = command.ExecuteReader();
                var url = reader.GetOrdinal("url");
                while (reader.Read())
                {
                    paths[contador] = reader.GetString(url);
                    contador++;
                }

                pathsObj.Add(paths);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA UN REGISTRO DE CONTROL DE BIOSEGURIDAD*/
        public static async Task SaveBioseguridad(int idFinca, string latitud, string longitud, Indicadores[] mediciones, String[] ListaText)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Se lee el id del usuario
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                int idUsuario = prefs.GetInt("idUsuario", 0);

                //Almacenamiento de visita
                SqliteCommand command = new SqliteCommand(@"

                    INSERT INTO visitas (id_server_finca, id_server_usuario, id_server_tipo_visita, fecha, estado_sync)
                    VALUES (@id_finca, @id_usuario, 2, DATETIME('now','localtime'), 2);

                    SELECT LAST_INSERT_ROWID() AS id_visita;

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                command.Parameters.AddWithValue("@id_usuario", idUsuario);
                var reader = command.ExecuteReader();
                var id_visita = reader.GetOrdinal("id_visita");
                while (reader.Read())
                {
                    id_visita = int.Parse(reader.GetString(id_visita));
                }

                //Almacenamiento de punto captura
                command = new SqliteCommand(@"

                    INSERT INTO punto_captura (id_visita, id_server_tipo, fecha)
                    VALUES (@id_visita, 4, DATETIME('now','localtime'));

                    SELECT LAST_INSERT_ROWID() AS id_punto_captura;

                ", connection);
                command.Parameters.AddWithValue("@id_visita", id_visita);
                reader = command.ExecuteReader();
                var id_punto_captura = reader.GetOrdinal("id_punto_captura");
                while (reader.Read())
                {
                    id_punto_captura = int.Parse(reader.GetString(id_punto_captura));
                }

                //Almacenamiento de planta en el punto de captura
                command = new SqliteCommand(@"

                    INSERT INTO planta_punto_captura (id_punto_captura, id_server_edad, latitud, longitud, fecha)
                    VALUES (@id_punto_captura, 7, @latitud, @longitud, DATETIME('now','localtime'));

                    SELECT LAST_INSERT_ROWID() AS id_planta_punto_captura;

                ", connection);
                command.Parameters.AddWithValue("@id_punto_captura", id_punto_captura);
                command.Parameters.AddWithValue("@latitud", latitud);
                command.Parameters.AddWithValue("@longitud", longitud);
                reader = command.ExecuteReader();
                var id_planta_punto_captura = reader.GetOrdinal("id_planta_punto_captura");
                while (reader.Read())
                {
                    //Almacenamiento de id con el que se guardo la planta
                    id_planta_punto_captura = int.Parse(reader.GetString(id_planta_punto_captura));
                }

                //Insercion de indicadores de la planta en el punto de captura
                for (int i = 0; i < mediciones.Length; i++)
                {
                    command = new SqliteCommand(@"

                        INSERT INTO indicadores_punto_captura (id_planta_punto_captura, id_server_indicador, valor)
                        VALUES (@id_planta_punto_captura, @id_server_indicador, @valor);

                        SELECT LAST_INSERT_ROWID() AS id_indicadores_punto_captura;

                    ", connection);
                    command.Parameters.AddWithValue("@id_planta_punto_captura", id_planta_punto_captura);
                    command.Parameters.AddWithValue("@id_server_indicador", mediciones[i].id);
                    command.Parameters.AddWithValue("@valor", mediciones[i].valor.ToString(CultureInfo.InvariantCulture));
                    reader = command.ExecuteReader();
                    var id_indicadores_punto_captura = reader.GetOrdinal("id_indicadores_punto_captura");
                    while (reader.Read())
                    {
                        //Almacenamiento de id con el que se guardo el indicador que requiere imagen
                        id_indicadores_punto_captura = int.Parse(reader.GetString(id_indicadores_punto_captura));
                    }

                    //Insercion de url local de las fotos del indicador
                    for (int j = 0; j < mediciones[i].paths.Length; j++)
                    {
                        //Solo guarda url validas
                        if (mediciones[i].paths[j] != null)
                        {
                            if (mediciones[i].paths[j].Length > 0)
                            {
                                command = new SqliteCommand(@"

                                    INSERT INTO fotos_indicador (id_lectura_indicador, url, estado_sync)
                                    VALUES (@id_indicadores_punto_captura, @url, 2);

                                ", connection);
                                command.Parameters.AddWithValue("@id_indicadores_punto_captura", id_indicadores_punto_captura);
                                command.Parameters.AddWithValue("@url", mediciones[i].paths[j]);
                                command.ExecuteNonQuery();
                            }
                        }
                    }
                }

                int id_indicador = 45;
                //Insercion de campos de texto en el punto de captura
                for (int i = 0; i < ListaText.Length; i++)
                {
                    command = new SqliteCommand(@"

                        INSERT INTO indicadores_punto_captura (id_planta_punto_captura, id_server_indicador, valor, valor_texto)
                        VALUES (@id_planta_punto_captura, @id_server_indicador, 0, @valor);

                    ", connection);
                    command.Parameters.AddWithValue("@id_planta_punto_captura", id_planta_punto_captura);
                    command.Parameters.AddWithValue("@id_server_indicador", id_indicador + i);
                    command.Parameters.AddWithValue("@valor", ListaText[i]);
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA CUANTOS CHEQUEOS DE BIOSEGURIDAD HAY PARA UNA FINCA*/
        public static async Task CountBioseguridad(int idFinca, string fecha, string fecha_fin, ObservableCollection<Visitas> visitas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id,
	                    'Chequeo ' || CAST(CAST(ROW_NUMBER() OVER(PARTITION BY id_server_finca, DATE(fecha) ORDER BY fecha) AS TEXT) AS TEXT) AS visita,
	                    fecha
                    FROM visitas AS BASE
                    WHERE id_server_finca = @id_finca
                    AND DATE(fecha) BETWEEN @fecha AND @fecha_fin
                    AND id_server_tipo_visita = 2
                    ORDER BY
	                    fecha DESC

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                command.Parameters.AddWithValue("@fecha", fecha);
                command.Parameters.AddWithValue("@fecha_fin", fecha_fin);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var nombre = reader.GetOrdinal("visita");
                var fecha_visita = reader.GetOrdinal("fecha");
                while (reader.Read())
                {
                    visitas.Add(new Visitas(int.Parse(reader.GetString(id)), reader.GetString(nombre), reader.GetString(fecha_visita)));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CARGA INFORMACION DE UN FORMULARIO DE BIOSEGURIDAD GUARDADO*/
        public static async Task BringChequeos(int idVisita, ObservableCollection<ControlesBioseguridad> chequeos1,
                                               ObservableCollection<ControlesBioseguridad> chequeos2, ObservableCollection<ControlesBioseguridad> chequeos3,
                                               ObservableCollection<ControlesBioseguridad> chequeos4, ObservableCollection<String[]> ListaTextObservable)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                String[] ListaText = { "", "", "" };

                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    IPC.id_server_indicador	AS id,
	                    IPC.valor,
	                    IPC.valor_texto,
	                    SUM(IFNULL(FI.id, 0)) AS img
                    FROM punto_captura AS PC
                    INNER JOIN planta_punto_captura AS PPC
                    ON PC.id = PPC.id_punto_captura
                    INNER JOIN indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta_punto_captura
                    LEFT JOIN fotos_indicador AS FI
                    ON IPC.id = FI.id_lectura_indicador
                    WHERE PC.id_visita = @id_visita
                    GROUP BY
	                    IPC.id_server_indicador,
	                    IPC.valor,
	                    IPC.valor_texto
                    ORDER BY
	                    IPC.id_server_indicador ASC

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();
                var id = reader.GetOrdinal("id");
                var valor = reader.GetOrdinal("valor");
                var valor_texto = reader.GetOrdinal("valor_texto");
                var img = reader.GetOrdinal("img");
                while (reader.Read())
                {
                    //Cuestionario 1
                    if (int.Parse(reader.GetString(id)) >= 15 && int.Parse(reader.GetString(id)) <= 28)
                    {
                        chequeos1[int.Parse(reader.GetString(id)) - 15].respuesta = (int)float.Parse(reader.GetString(valor));
                        chequeos1[int.Parse(reader.GetString(id)) - 15].camara = (int)float.Parse(reader.GetString(img));
                    }
                    //Cuestionario 2
                    else if (int.Parse(reader.GetString(id)) >= 29 && int.Parse(reader.GetString(id)) <= 35)
                    {
                        chequeos2[int.Parse(reader.GetString(id)) - 29].respuesta = (int)float.Parse(reader.GetString(valor));
                        chequeos2[int.Parse(reader.GetString(id)) - 29].camara = (int)float.Parse(reader.GetString(img));
                    }
                    //Cuestionario 3
                    else if (int.Parse(reader.GetString(id)) >= 36 && int.Parse(reader.GetString(id)) <= 39)
                    {
                        chequeos3[int.Parse(reader.GetString(id)) - 36].respuesta = (int)float.Parse(reader.GetString(valor));
                        chequeos3[int.Parse(reader.GetString(id)) - 36].camara = (int)float.Parse(reader.GetString(img));
                    }
                    //Cuestionario 4
                    else if (int.Parse(reader.GetString(id)) >= 40 && int.Parse(reader.GetString(id)) <= 44)
                    {
                        chequeos4[int.Parse(reader.GetString(id)) - 40].respuesta = (int)float.Parse(reader.GetString(valor));
                        chequeos4[int.Parse(reader.GetString(id)) - 40].camara = (int)float.Parse(reader.GetString(img));
                    }
                    //Campos de texto
                    else if (int.Parse(reader.GetString(id)) >= 45 && int.Parse(reader.GetString(id)) <= 47)
                    {
                        ListaText[int.Parse(reader.GetString(id)) - 45] = reader.GetString(valor_texto);
                    }
                    else { }
                }

                ListaTextObservable.Add(ListaText);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA LAS FOTOS TOMADAS EN UN CHEQUEO DE BIOSEGURIDAD*/
        public static async Task BringFotoBioseguridad(int idVisita, int idIndicador, ObservableCollection<String[]> pathsObj)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                int contador = 0;
                String[] paths = { "", "", "" };

                SqliteCommand command = new SqliteCommand(@"

                    SELECT FI.url
                    FROM punto_captura AS PC
                    INNER JOIN planta_punto_captura AS PPC
                    ON PC.id = PPC.id_punto_captura
                    INNER JOIN indicadores_punto_captura AS IPC
                    ON PPC.id = IPC.id_planta_punto_captura
                    AND IPC.id_server_indicador = @id_server_indicador
                    INNER JOIN fotos_indicador AS FI
                    ON IPC.id = FI.id_lectura_indicador
                    WHERE PC.id_visita = @id_visita

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.Parameters.AddWithValue("@id_server_indicador", idIndicador);
                var reader = command.ExecuteReader();
                var url = reader.GetOrdinal("url");
                while (reader.Read())
                {
                    paths[contador] = reader.GetString(url);
                    contador++;
                }

                pathsObj.Add(paths);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*CONSULTA LAS PRECIPITACIONES DE UNA SEMANA*/
        public static async Task LoadPrecipitaciones(int idFinca, string[] listaFechas, ObservableCollection<float> precipitaciones)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    SELECT IFNULL(P.valor, 0) AS precipitacion
                    FROM
                    (
	                    SELECT @dia1 AS fecha
	                    UNION
	                    SELECT @dia2 AS fecha
	                    UNION
	                    SELECT @dia3 AS fecha
	                    UNION
	                    SELECT @dia4 AS fecha
	                    UNION
	                    SELECT @dia5 AS fecha
	                    UNION
	                    SELECT @dia6 AS fecha
	                    UNION	
	                    SELECT @dia7 AS fecha
                    )AS S
                    LEFT JOIN precipitaciones AS P
                    ON DATE(P.fecha) = S.fecha
                    AND P.id_server_finca = @id_finca

                ", connection);
                command.Parameters.AddWithValue("@dia1", DateTime.Parse(listaFechas[0].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia2", DateTime.Parse(listaFechas[1].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia3", DateTime.Parse(listaFechas[2].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia4", DateTime.Parse(listaFechas[3].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia5", DateTime.Parse(listaFechas[4].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia6", DateTime.Parse(listaFechas[5].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia7", DateTime.Parse(listaFechas[6].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@id_finca", idFinca);
                var reader = command.ExecuteReader();
                var precipitacion = reader.GetOrdinal("precipitacion");
                while (reader.Read())
                {
                    var readData = reader.GetFloat(precipitacion);
                    precipitaciones.Add(readData);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA LAS PRECIPITACIONES DE UNA SEMANA*/
        public static async Task SavePrecipitaciones(int idFinca, string[] listaFechas, ObservableCollection<float> precipitaciones)
        {
            SqliteConnection connection = DB.GetConnection();
            //Se lee el id del usuario
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            int idUsuario = prefs.GetInt("idUsuario", 0);
            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    -- SE ACTUALIZAN LOS DATOS QUE EXISTENTES
                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia1,
		                    valor = @precipitacion1,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion1
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia1;

                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia2,
		                    valor = @precipitacion2,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion2
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia2;

                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia3,
		                    valor = @precipitacion3,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion3
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia3;

                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia4,
		                    valor = @precipitacion4,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion4
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia4;

                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia5,
		                    valor = @precipitacion5,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion5
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia5;

                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia6,
		                    valor = @precipitacion6,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion6
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia6;

                    UPDATE precipitaciones
                    SET
		                    id_server_usuario = @id_usuario,
		                    fecha = @dia7,
		                    valor = @precipitacion7,
                            estado_sync = 2 --ESTADO PENDIENTE
                    WHERE valor <> @precipitacion7
                    AND id_server_finca = @id_finca
                    AND DATE(fecha) = @dia7;

                    -- SE INSERTAN LOS DATOS QUE NO EXISTEN
                    INSERT INTO precipitaciones
                    (id_server_finca, id_server_usuario, fecha, valor, estado_sync)
                    SELECT
                        S.id_server_finca,
                        S.id_server_usuario,
                        S.fecha,
                        S.valor,
                        2 AS estado_sync --ESTADO PENDIENTE
                    FROM
                    (
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia1 AS fecha,
                            @precipitacion1 AS valor
	                    UNION
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia2 AS fecha,
                            @precipitacion2 AS valor
	                    UNION
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia3 AS fecha,
                            @precipitacion3 AS valor
	                    UNION
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia4 AS fecha,
                            @precipitacion4 AS valor
	                    UNION
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia5 AS fecha,
                            @precipitacion5 AS valor
	                    UNION
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia6 AS fecha,
                            @precipitacion6 AS valor
	                    UNION	
	                    SELECT 
                            @id_finca AS id_server_finca,
                            @id_usuario AS id_server_usuario,
                            @dia7 AS fecha,
                            @precipitacion7 AS valor
                    )AS S
                    LEFT JOIN precipitaciones AS P
                    ON S.id_server_finca = P.id_server_finca
                    AND S.fecha = DATE(P.fecha)
                    WHERE CAST(S.valor AS REAL) <> CAST(0 AS REAL)
                    AND P.id IS NULL;

                ", connection);
                command.Parameters.AddWithValue("@id_finca", idFinca);
                command.Parameters.AddWithValue("@id_usuario", idUsuario);
                command.Parameters.AddWithValue("@dia1", DateTime.Parse(listaFechas[0].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia2", DateTime.Parse(listaFechas[1].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia3", DateTime.Parse(listaFechas[2].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia4", DateTime.Parse(listaFechas[3].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia5", DateTime.Parse(listaFechas[4].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia6", DateTime.Parse(listaFechas[5].ToString()).ToString("yyyy-MM-dd"));
                command.Parameters.AddWithValue("@dia7", DateTime.Parse(listaFechas[6].ToString()).ToString("yyyy-MM-dd"));

                decimal p1 = Convert.ToDecimal(precipitaciones[0]);
                decimal p2 = Convert.ToDecimal(precipitaciones[1]);
                decimal p3 = Convert.ToDecimal(precipitaciones[2]);
                decimal p4 = Convert.ToDecimal(precipitaciones[3]);
                decimal p5 = Convert.ToDecimal(precipitaciones[4]);
                decimal p6 = Convert.ToDecimal(precipitaciones[5]);
                decimal p7 = Convert.ToDecimal(precipitaciones[6]);

                command.Parameters.AddWithValue("@precipitacion1", SqlDbType.Decimal).Value = p1;
                command.Parameters.AddWithValue("@precipitacion2", SqlDbType.Decimal).Value = p2;
                command.Parameters.AddWithValue("@precipitacion3", SqlDbType.Decimal).Value = p3;
                command.Parameters.AddWithValue("@precipitacion4", SqlDbType.Decimal).Value = p4;
                command.Parameters.AddWithValue("@precipitacion5", SqlDbType.Decimal).Value = p5;
                command.Parameters.AddWithValue("@precipitacion6", SqlDbType.Decimal).Value = p6;
                command.Parameters.AddWithValue("@precipitacion7", SqlDbType.Decimal).Value = p7;
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /****************************************************CONSULTAS DE SINCRONIZACION ********************************************************/
        /*CONSULTA LAS PRECIPITACIONES DE UNA SEMANA*/
        public static async Task InfoUsuario(int idUsuario, ObservableCollection<string> parametrosUsuario)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                //USER
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    usuario
                    FROM usuarios
                    WHERE id_server = @id_usuario

                ", connection);
                command.Parameters.AddWithValue("@id_usuario", idUsuario);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);

                string user = table.Rows[0]["usuario"].ToString();
                parametrosUsuario.Add(user);

                //PASS
                command = new SqliteCommand(@"

                    SELECT
	                    pass
                    FROM usuarios
                    WHERE id_server = @id_usuario

                ", connection);
                command.Parameters.AddWithValue("@id_usuario", idUsuario);
                reader = command.ExecuteReader();

                table = new DataTable();
                table.Load(reader);

                string pass = table.Rows[0]["pass"].ToString();
                parametrosUsuario.Add(pass);

                //DEPARTAMENTOS
                command = new SqliteCommand(@"

                    SELECT
	                    nombre
                    FROM departamentos
                    ORDER BY id_server

                ", connection);
                reader = command.ExecuteReader();

                table = new DataTable();
                table.Load(reader);

                string departamentos = Encrypter.MD5Text((JsonConvert.SerializeObject(table, Formatting.None)));
                parametrosUsuario.Add(departamentos);

                //CIUDADES
                command = new SqliteCommand(@"

                    SELECT
	                    nombre
                    FROM ciudades
                    ORDER BY id_server

                ", connection);
                reader = command.ExecuteReader();

                table = new DataTable();
                table.Load(reader);

                string ciudades = Encrypter.MD5Text((JsonConvert.SerializeObject(table, Formatting.None)));
                parametrosUsuario.Add(ciudades);

                //SECTORES
                command = new SqliteCommand(@"

                    SELECT
	                    nombre
                    FROM sectores
                    ORDER BY id_server

                ", connection);
                reader = command.ExecuteReader();

                table = new DataTable();
                table.Load(reader);

                string sectores = Encrypter.MD5Text((JsonConvert.SerializeObject(table, Formatting.None)));
                parametrosUsuario.Add(sectores);

                //FINCAS
                command = new SqliteCommand(@"

                    SELECT
	                    nombre,
	                    activo,
	                    id_server_sector AS sector
                    FROM fincas
                    ORDER BY id_server

                ", connection);
                reader = command.ExecuteReader();

                table = new DataTable();
                table.Load(reader);

                string fincas = Encrypter.MD5Text((JsonConvert.SerializeObject(table, Formatting.None)));
                parametrosUsuario.Add(fincas);

                //FINCAS
                command = new SqliteCommand(@"

                    SELECT
	                    usuario,
	                    pass,
	                    nombres,
	                    apellido,
	                    activo
                    FROM usuarios
                    ORDER BY id_server

                ", connection);
                reader = command.ExecuteReader();

                table = new DataTable();
                table.Load(reader);

                string usuarios = Encrypter.MD5Text((JsonConvert.SerializeObject(table, Formatting.None)));
                parametrosUsuario.Add(usuarios);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA DEPARTAMENTOS*/
        public static async Task SincronizacionDepartamentos(JArray departamentos)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    DELETE FROM departamentos;

                    UPDATE sqlite_sequence
                        SET seq = 0
                    WHERE name = 'departamentos';

                ", connection);
                command.ExecuteNonQuery();

                using (var transaction = connection.BeginTransaction())
                {
                    command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO departamentos
                            (id_server, nombre)
                            VALUES ($id_server, $nombre)
                        ";

                    var id = command.CreateParameter();
                    id.ParameterName = "$id_server";
                    command.Parameters.Add(id);

                    var nombre = command.CreateParameter();
                    nombre.ParameterName = "$nombre";
                    command.Parameters.Add(nombre);

                    for (var i = 0; i < departamentos.Count; i++)
                    {
                        id.Value = int.Parse(departamentos[i]["id_server"].ToString());
                        nombre.Value = departamentos[i]["nombre"].ToString();
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA CIUDADES*/
        public static async Task SincronizacionCiudades(JArray ciudades)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    DELETE FROM ciudades;

                    UPDATE sqlite_sequence
                        SET seq = 0
                    WHERE name = 'ciudades';

                ", connection);
                command.ExecuteNonQuery();

                using (var transaction = connection.BeginTransaction())
                {
                    command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO ciudades
                            (id_server, id_server_departamento, nombre)
                            VALUES ($id_server, $id_server_departamento, $nombre)
                        ";

                    var id = command.CreateParameter();
                    id.ParameterName = "$id_server";
                    command.Parameters.Add(id);

                    var id_departamento = command.CreateParameter();
                    id_departamento.ParameterName = "$id_server_departamento";
                    command.Parameters.Add(id_departamento);

                    var nombre = command.CreateParameter();
                    nombre.ParameterName = "$nombre";
                    command.Parameters.Add(nombre);

                    for (var i = 0; i < ciudades.Count; i++)
                    {
                        id.Value = int.Parse(ciudades[i]["id_server"].ToString());
                        id_departamento.Value = int.Parse(ciudades[i]["id_server_departamento"].ToString());
                        nombre.Value = ciudades[i]["nombre"].ToString();
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA SECTORES*/
        public static async Task SincronizacionSectores(JArray sectores)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    DELETE FROM sectores;

                    UPDATE sqlite_sequence
                        SET seq = 0
                    WHERE name = 'sectores';

                ", connection);
                command.ExecuteNonQuery();

                using (var transaction = connection.BeginTransaction())
                {
                    command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO sectores
                            (id_server, nombre)
                            VALUES ($id_server, $nombre)
                        ";

                    var id = command.CreateParameter();
                    id.ParameterName = "$id_server";
                    command.Parameters.Add(id);

                    var nombre = command.CreateParameter();
                    nombre.ParameterName = "$nombre";
                    command.Parameters.Add(nombre);

                    for (var i = 0; i < sectores.Count; i++)
                    {
                        id.Value = int.Parse(sectores[i]["id_server"].ToString());
                        nombre.Value = sectores[i]["nombre"].ToString();
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA FINCAS*/
        public static async Task SincronizacionFincas(JArray fincas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    DELETE FROM fincas;

                    UPDATE sqlite_sequence
                        SET seq = 0
                    WHERE name = 'fincas';

                ", connection);
                command.ExecuteNonQuery();

                using (var transaction = connection.BeginTransaction())
                {
                    command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO fincas
                            (id_server, id_server_ciudad, nombre, id_server_sector, activo)
                            VALUES ($id_server, $id_server_ciudad, $nombre, $id_server_sector, $activo)
                        ";

                    var id = command.CreateParameter();
                    id.ParameterName = "$id_server";
                    command.Parameters.Add(id);

                    var id_ciudad = command.CreateParameter();
                    id_ciudad.ParameterName = "$id_server_ciudad";
                    command.Parameters.Add(id_ciudad);

                    var nombre = command.CreateParameter();
                    nombre.ParameterName = "$nombre";
                    command.Parameters.Add(nombre);

                    var id_sector = command.CreateParameter();
                    id_sector.ParameterName = "$id_server_sector";
                    command.Parameters.Add(id_sector);

                    var activo = command.CreateParameter();
                    activo.ParameterName = "$activo";
                    command.Parameters.Add(activo);

                    for (var i = 0; i < fincas.Count; i++)
                    {
                        id.Value = int.Parse(fincas[i]["id_server"].ToString());
                        id_ciudad.Value = fincas[i]["id_server_ciudad"].ToString() == "" ? 0 : int.Parse(fincas[i]["id_server_ciudad"].ToString());
                        nombre.Value = fincas[i]["nombre"].ToString();
                        id_sector.Value = fincas[i]["id_server_sector"].ToString() == "" ? 0 : int.Parse(fincas[i]["id_server_sector"].ToString());
                        activo.Value = int.Parse(fincas[i]["activo"].ToString());
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }

                command = new SqliteCommand(@"

                    UPDATE fincas
                    SET id_server_ciudad = NULL
                    WHERE id_server_ciudad = 0;

                ", connection);
                command.ExecuteNonQuery();

                command = new SqliteCommand(@"

                    UPDATE fincas
                    SET id_server_sector = NULL
                    WHERE id_server_sector = 0;

                ", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA USUARIOS*/
        public static async Task SincronizacionUsuarios(JArray usuarios)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    DELETE FROM usuarios;

                    UPDATE sqlite_sequence
                        SET seq = 0
                    WHERE name = 'usuarios';

                ", connection);
                command.ExecuteNonQuery();

                using (var transaction = connection.BeginTransaction())
                {
                    command = connection.CreateCommand();
                    command.CommandText =
                        @"
                            INSERT INTO usuarios
                            (id_server, usuario, pass, nombres, apellido, activo)
                            VALUES ($id_server, $usuario, $pass, $nombres, $apellido, $activo)
                        ";

                    var id = command.CreateParameter();
                    id.ParameterName = "$id_server";
                    command.Parameters.Add(id);

                    var usuario = command.CreateParameter();
                    usuario.ParameterName = "$usuario";
                    command.Parameters.Add(usuario);

                    var pass = command.CreateParameter();
                    pass.ParameterName = "$pass";
                    command.Parameters.Add(pass);

                    var nombres = command.CreateParameter();
                    nombres.ParameterName = "$nombres";
                    command.Parameters.Add(nombres);

                    var apellido = command.CreateParameter();
                    apellido.ParameterName = "$apellido";
                    command.Parameters.Add(apellido);

                    var activo = command.CreateParameter();
                    activo.ParameterName = "$activo";
                    command.Parameters.Add(activo);

                    for (var i = 0; i < usuarios.Count; i++)
                    {
                        id.Value = int.Parse(usuarios[i]["id_server"].ToString());
                        usuario.Value = usuarios[i]["usuario"].ToString();
                        pass.Value = usuarios[i]["pass"].ToString();
                        nombres.Value = usuarios[i]["nombres"].ToString();
                        apellido.Value = usuarios[i]["apellido"].ToString();
                        activo.Value = int.Parse(usuarios[i]["activo"].ToString());
                        command.ExecuteNonQuery();
                    }

                    transaction.Commit();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA PRECIPITACIONES*/
        public static async Task SincronizacionPrecipitaciones(ObservableCollection<DataTable> precipitacionesSincronizar)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Seleciona grupo de precipitaciones para sincronizar
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id_server_finca AS id_recurso,
	                    id_server_usuario AS id_usuario,
	                    fecha,
	                    valor
                    FROM precipitaciones
                    WHERE estado_sync = 2;

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                precipitacionesSincronizar.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*TERMINA LA SINCRONIZACION PRECIPITACIONES*/
        public static async Task FinSincronizacionPrecipitaciones()
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    UPDATE precipitaciones
                    SET estado_sync = 4,
                        fecha_sync = DATETIME('now','localtime')
                    WHERE estado_sync = 2;

                ", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA VISITAS*/
        public static async Task SincronizacionVisitas(ObservableCollection<DataTable> visitas)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id,
	                    id_server_finca,
	                    id_server_usuario,
	                    id_server_tipo_visita,
	                    temperatura_minima,
	                    temperatura,
	                    temperatura_maxima,
	                    humedad_relativa,
	                    fecha
                    FROM
                    visitas
                    WHERE estado_sync = 2; --PENDIENTES

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                visitas.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA RECORRIDO*/
        public static async Task SincronizacionRecorrido(ObservableCollection<DataTable> recorrido)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    R.id_visita,
	                    R.latitud,
	                    R.longitud,
	                    R.fecha
                    FROM
                    visitas AS V
                    INNER JOIN recorrido_visita AS R
                    ON V.id = R.id_visita
                    WHERE V.estado_sync = 2; --PENDIENTES

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                recorrido.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA PUNTO CAPTURA*/
        public static async Task SincronizacionPunto(ObservableCollection<DataTable> punto)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    P.id,
	                    P.id_visita,
	                    P.id_server_tipo,
	                    P.fecha
                    FROM
                    visitas AS V
                    INNER JOIN punto_captura AS P
                    ON V.id = P.id_visita
                    WHERE V.estado_sync = 2; --PENDIENTES

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                punto.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA PLANTA PUNTO CAPTURA*/
        public static async Task SincronizacionPlanta(ObservableCollection<DataTable> planta)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    PP.id,
                        PP.id_punto_captura,
	                    PP.id_server_edad,
	                    PP.latitud,
	                    PP.longitud,
                        PP.fecha
                    FROM
                    visitas AS V
                    INNER JOIN punto_captura AS P
                    ON V.id = P.id_visita
                    INNER JOIN planta_punto_captura AS PP
                    ON PP.id_punto_captura = P.id
                    WHERE V.estado_sync = 2; --PENDIENTES

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                planta.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA PLANTA PUNTO CAPTURA*/
        public static async Task SincronizacionIndicador(ObservableCollection<DataTable> indicador)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
                        I.id,
                        I.id_planta_punto_captura,
                        I.id_server_indicador,
                        I.valor,
                        I.valor_texto
                    FROM
                    visitas AS V
                    INNER JOIN punto_captura AS P
                    ON V.id = P.id_visita
                    INNER JOIN planta_punto_captura AS PP
                    ON PP.id_punto_captura = P.id
                    INNER JOIN indicadores_punto_captura AS I
                    ON PP.id = I.id_planta_punto_captura
                    WHERE V.estado_sync = 2; --PENDIENTES

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                indicador.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION FOTOS INDICADOR*/
        public static async Task SincronizacionFoto(ObservableCollection<DataTable> foto)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
                        F.id_lectura_indicador,
                        SUBSTR(F.url, 72) AS url
                    FROM
                    visitas AS V
                    INNER JOIN punto_captura AS P
                    ON V.id = P.id_visita
                    INNER JOIN planta_punto_captura AS PP
                    ON PP.id_punto_captura = P.id
                    INNER JOIN indicadores_punto_captura AS I
                    ON PP.id = I.id_planta_punto_captura
                    INNER JOIN fotos_indicador AS F
                    ON I.id = F.id_lectura_indicador
                    WHERE V.estado_sync = 2; --PENDIENTES

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                foto.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*TERMINA LA SINCRONIZACION DE VISITAS*/
        public static async Task FinSincronizacionVisitas()
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    UPDATE visitas
                    SET estado_sync = 4,
                        fecha_sync = DATETIME('now','localtime')
                    WHERE estado_sync = 2;

                ", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION FOTOS*/
        public static async Task SincronizacionFotos(ObservableCollection<DataTable> fotosSincronizar)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Seleciona imagen para sincronizar
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id,
	                    url AS fullPath,
	                    SUBSTR(url, 72) AS name
                    FROM fotos_indicador
                    WHERE estado_sync = 2
                    LIMIT 1;

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                fotosSincronizar.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*TERMINA LA SINCRONIZACION DE FOTOS*/
        public static async Task FinSincronizacionFotos(int id)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    UPDATE fotos_indicador
                    SET estado_sync = 4,
                        fecha_sync = DATETIME('now','localtime')
                    WHERE id = @id;

                ", connection);
                command.Parameters.AddWithValue("@id", id);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION MAESTROS USANDO LA INFORMACION DE USUARIOS*/
        public static async Task SincronizacionMaestros(ObservableCollection<int> maestros)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                //Seleciona imagen para sincronizar
                SqliteCommand command = new SqliteCommand(@"

                    SELECT COUNT(*) AS count
                    FROM usuarios 

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                maestros.Add(int.Parse(table.Rows[0][0].ToString()));
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*GUARDA LOS MENSAJES DE ERROR PARA LOS SERVICIOS*/
        public static async Task ErrorService(int id_server_usuario, int id_server_servicio, string mensaje)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand(@"

                    INSERT INTO errores
                    (id_server_usuario, id_server_servicio, fecha, mensaje, estado_sync, fecha_sync)
                    VALUES (@id_server_usuario, @id_server_servicio, DATETIME('now','localtime'), @mensaje, 2, DATETIME('now','localtime'))

                ", connection);
                command.Parameters.AddWithValue("@id_server_usuario", id_server_usuario);
                command.Parameters.AddWithValue("@id_server_servicio", id_server_servicio);
                command.Parameters.AddWithValue("@mensaje", mensaje);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*SINCRONIZACION TABLA ERRORES*/
        public static async Task SincronizacionErrores(ObservableCollection<DataTable> erroresSincronizar)
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();

                //Seleciona grupo de precipitaciones para sincronizar
                SqliteCommand command = new SqliteCommand(@"

                    SELECT
	                    id_server_usuario AS id_usuario,
	                    id_server_servicio AS id_servicio,
	                    fecha,
	                    mensaje
                    FROM errores
                    WHERE estado_sync = 2;

                ", connection);
                var reader = command.ExecuteReader();

                DataTable table = new DataTable();
                table.Load(reader);
                erroresSincronizar.Add(table);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }

        /*TERMINA LA SINCRONIZACION ERRORES*/
        public static async Task FinSincronizacionErrores()
        {
            SqliteConnection connection = DB.GetConnection();

            try
            {
                connection.Open();
                SqliteCommand command = new SqliteCommand(@"

                    UPDATE errores
                    SET estado_sync = 4,
                        fecha_sync = DATETIME('now','localtime')
                    WHERE estado_sync = 2;

                ", connection);
                command.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}