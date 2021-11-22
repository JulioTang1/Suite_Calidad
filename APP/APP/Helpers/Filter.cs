using Android.Content;
using Android.Preferences;
using Binateq.GpsTrackFilter;
using Microsoft.Data.Sqlite;
using System;
using System.Data;
using System.Globalization;
using System.Threading.Tasks;

namespace APP.Helpers
{
    public class Filter
    {
        private Context context = global::Android.App.Application.Context;
        Intent serviceIntent;

        /*FILTRA EL RECORRIDO DE UNA VISITA PARA GUARDAR MENOS DATOS EN LOCAL*/
        public async Task Filtrar()
        {
            SqliteConnection connection = DB.GetConnection();
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            //Guarda en base de datos el error
            int idVisita = prefs.GetInt("idVisita_procesamiento", 0);

            try
            {
                connection.Open();

                //Seleccion de un recorrido finalizado
                SqliteCommand command = new SqliteCommand(@"

                    DELETE
                    FROM recorrido_visita
                    WHERE id_visita = @id_visita
                    AND procesamiento = 0;

                    UPDATE recorrido_visita
                    SET procesamiento = 1
                    WHERE id_visita = @id_visita;

                    SELECT
	                    latitud,
	                    longitud,
                        fecha                            
                    FROM recorrido_visita
                    WHERE id_visita = @id_visita
                    ORDER BY
	                    fecha;

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                var reader = command.ExecuteReader();

                //Transformacion de los datos en datatable para procesamiento
                DataTable dt = new DataTable();
                dt.Load(reader);

                //Creacion de tupla del recorrido para filtrado
                int length = (int)Math.Ceiling(decimal.Parse(dt.Rows.Count.ToString()) / decimal.Parse("1000"));
                int restante = (int)Math.Ceiling(decimal.Parse(dt.Rows.Count.ToString()) % decimal.Parse("1000")) == 0 ? 1000 : (int)Math.Ceiling(decimal.Parse(dt.Rows.Count.ToString()) % decimal.Parse("1000"));
                
                Tuple<double, double, DateTimeOffset>[] rawGpsTrack;
                for (int i = 0; i < length; i++)
                {
                    if (i != length - 1)
                    {
                        rawGpsTrack = new Tuple<double, double, DateTimeOffset>[1000];
                        for (int j = (i * 1000); j < 1000 + (i*1000); j++)
                        {
                            rawGpsTrack[j - (i * 1000)] = new Tuple<double, double, DateTimeOffset>(
                                double.Parse(dt.Rows[j]["latitud"].ToString()),
                                double.Parse(dt.Rows[j]["longitud"].ToString()),
                                DateTime.Parse(dt.Rows[j]["fecha"].ToString())
                            );
                        }
                    }
                    else
                    {
                        rawGpsTrack = new Tuple<double, double, DateTimeOffset>[restante];
                        for (int j = (i * 1000); j < restante + (i * 1000); j++)
                        {
                            rawGpsTrack[j - (i * 1000)] = new Tuple<double, double, DateTimeOffset>(
                                double.Parse(dt.Rows[j]["latitud"].ToString()),
                                double.Parse(dt.Rows[j]["longitud"].ToString()),
                                DateTime.Parse(dt.Rows[j]["fecha"].ToString())
                            );
                        }
                    }

                    //Filtrado del recorrido
                    var filter = new GpsTrackFilter();
                    filter.ZeroSpeedDrift = 3;
                    filter.OutlineSpeed = 8;
                    var gpsTrack = filter.Filter(rawGpsTrack);

                    double latitud = 0;
                    double longitud = 0;

                    //Alamacenamiento del recorrido quitando puntos iguales
                    for (int k = 0; k < gpsTrack.Count; k++)
                    {
                        if (!(latitud == gpsTrack[k].Item1 && longitud == gpsTrack[k].Item2))
                        {
                            latitud = gpsTrack[k].Item1;
                            longitud = gpsTrack[k].Item2;

                            //Almacenamiento ubicacion
                            command = new SqliteCommand(@"

                                INSERT INTO recorrido_visita (id_visita, latitud, longitud, fecha, procesamiento)
                                VALUES (@id_visita, @latitud, @longitud, @fecha, 0);

                            ", connection);
                            command.Parameters.AddWithValue("@id_visita", idVisita);
                            command.Parameters.AddWithValue("@latitud", latitud.ToString(CultureInfo.InvariantCulture));
                            command.Parameters.AddWithValue("@longitud", longitud.ToString(CultureInfo.InvariantCulture));
                            command.Parameters.AddWithValue("@fecha", gpsTrack[k].Item3.DateTime.ToString("yyyy-MM-dd H:mm:ss"));
                            command.ExecuteNonQuery();
                        }
                    }
                }

                //Eliminacion del recorrido almacenado en base de datos
                command = new SqliteCommand(@"

                    DELETE
                    FROM recorrido_visita
                    WHERE id_visita = @id_visita
                    AND procesamiento = 1;

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Cambia bandera de sincronizacion de visitas y fotos
                command = new SqliteCommand(@"

                    UPDATE visitas
                    SET estado_sync = 2
                    WHERE id = @id_visita;

                    UPDATE fotos_indicador
                    SET estado_sync = 2
                    WHERE estado_sync = 1;                 

                ", connection);
                command.Parameters.AddWithValue("@id_visita", idVisita);
                command.ExecuteNonQuery();

                //Termina servicio
                string intentFilter = prefs.GetString("intentFilter", "");
                serviceIntent = Intent.GetIntent(intentFilter);
                editor.PutInt("idVisita_procesamiento", 0);
                editor.PutString("intentFilter", "");
                editor.Commit();

                Android.App.Application.Context.StopService(serviceIntent);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error en la apertura o ejecución de comandos");

                try
                {
                    //Guarda en base de datos el error
                    int idUsuario = prefs.GetInt("idUsuario", 0);
                    await DB.ErrorService(idUsuario, 3, e.Message);

                    //Termina service de sincronizacion
                    string intentFilter = prefs.GetString("intentFilter", "");
                    serviceIntent = Intent.GetIntent(intentFilter);
                    editor.PutString("intentFilter", "");
                    editor.Commit();
                    Android.App.Application.Context.StopService(serviceIntent);
                }
                catch
                {
                    //Guarda en base de datos el error
                    int idUsuario = prefs.GetInt("idUsuario", 0);
                    await DB.ErrorService(idUsuario, 3, e.Message);
                }
            }
            finally
            {
                connection.Close();
                connection.Dispose();
            }
        }
    }
}