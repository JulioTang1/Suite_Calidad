using System.Threading.Tasks;
using Android.Content;
using Android.Preferences;
using APP.Activities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Net.NetworkInformation;

namespace APP.Helpers
{
    public class Sincronizacion 
    {
        private Context context = global::Android.App.Application.Context;
        private String conexionUrl = "https://indicadores.banasan.com.co/apiServices/Sincronizacion.asmx";
        public CookieContainer cc = new CookieContainer();

        //variables
        ObservableCollection<DataTable> tabla;
        ObservableCollection<string> parametrosUsuario;
        Intent serviceIntent;
        private string token;

        public async Task SincronizarMaestros()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            try
            {
                Ping myPing = new Ping();
                PingReply reply = myPing.Send("indicadores.banasan.com.co");
                if (reply != null)
                {
                    if (reply.RoundtripTime < 2000)
                    {
                        await InicioMaestros();
                    }
                }

                //Termina service de sincronizacion
                string intentSincronizacion = prefs.GetString("intentSincronizacionMaestros", "");
                serviceIntent = Intent.GetIntent(intentSincronizacion);
                editor.PutString("intentSincronizacionMaestros", "");
                editor.Commit();
                Android.App.Application.Context.StopService(serviceIntent);
            }
            catch (Exception ex)
            {
                try
                {
                    //Guarda en base de datos el error
                    int idUsuario = prefs.GetInt("idUsuario", 0);
                    await DB.ErrorService(idUsuario, 1, ex.Message);

                    //Termina service de sincronizacion
                    string intentSincronizacion = prefs.GetString("intentSincronizacionMaestros", "");
                    serviceIntent = Intent.GetIntent(intentSincronizacion);
                    editor.PutString("intentSincronizacionMaestros", "");
                    editor.Commit();
                    Android.App.Application.Context.StopService(serviceIntent);
                }
                catch (Exception e)
                {
                    //Guarda en base de datos el error
                    int idUsuario = prefs.GetInt("idUsuario", 0);
                    await DB.ErrorService(idUsuario, 1, e.Message);
                }
            }
        }

        public async Task InicioMaestros()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();

            //TOKEN INICIAL
            JObject autenticar = await this.Autenticar();
            if (autenticar["ESTADO"].ToString() == "FALSE")
            {
                return;
            }

            //SE GUARDA EL TOKEN
            token = autenticar["RESULTADO"].ToString();

            //SINCRONIZACION DE ERRORES
            tabla = new ObservableCollection<DataTable>();
            await DB.SincronizacionErrores(tabla);
            if (tabla[0].Rows.Count > 0)
            {
                JObject errores = await this.SincronizacionErrores(tabla[0], token);
                if (errores["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.FinSincronizacionErrores();
            }

            //SINCRONIZACION DE USUARIOS
            JObject usuarios = await this.SincronizacionMaestros("usuarios", token);
            if (usuarios["ESTADO"].ToString() == "FALSE")
            {
                return;
            }
            await DB.SincronizacionUsuarios((JArray)usuarios["RESULTADO"]);
            

            //SINCRONIZACION DE DEPARTAMENTOS
            JObject departamentos = await this.SincronizacionMaestros("departamentos", token);
            if (departamentos["ESTADO"].ToString() == "FALSE")
            {
                return;
            }
            await DB.SincronizacionDepartamentos((JArray)departamentos["RESULTADO"]);
            

            //SINCRONIZACION DE MUNICIPIOS
            JObject ciudades = await this.SincronizacionMaestros("ciudades", token);
            if (ciudades["ESTADO"].ToString() == "FALSE")
            {
                return;
            }
            await DB.SincronizacionCiudades((JArray)ciudades["RESULTADO"]);
            

            //SINCRONIZACION DE SECTORES
            JObject sectores = await this.SincronizacionMaestros("sectores", token);
            if (sectores["ESTADO"].ToString() == "FALSE")
            {
                return;
            }
            await DB.SincronizacionSectores((JArray)sectores["RESULTADO"]);

            //SINCRONIZACION DE FINCAS
            JObject fincas = await this.SincronizacionMaestros("fincas", token);
            if (fincas["ESTADO"].ToString() == "FALSE")
            {
                return;
            }
            await DB.SincronizacionFincas((JArray)fincas["RESULTADO"]);
        }

        public async Task Sincronizar()
        {
            //VERIFICA QUE NO SE ENCUENTRE EN UN RECORRIDO
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();
            try
            {
                string intentGPS = prefs.GetString("intentGPS", "");
                if (intentGPS == "")
                {
                    Ping myPing = new Ping();
                    PingReply reply = myPing.Send("indicadores.banasan.com.co");
                    if (reply != null)
                    {
                        if (reply.RoundtripTime < 2000)
                        {
                            await Inicio();
                        }
                    }
                }

                //Termina service de sincronizacion
                string intentSincronizacion = prefs.GetString("intentSincronizacion", "");
                serviceIntent = Intent.GetIntent(intentSincronizacion);
                editor.PutString("intentSincronizacion", "");
                editor.Commit();
                Android.App.Application.Context.StopService(serviceIntent);
            }
            catch (Exception ex)
            {
                try
                {
                    //Guarda en base de datos el error
                    int idUsuario = prefs.GetInt("idUsuario", 0);
                    await DB.ErrorService(idUsuario, 1, ex.Message);

                    //Termina service de sincronizacion
                    string intentSincronizacion = prefs.GetString("intentSincronizacion", "");
                    serviceIntent = Intent.GetIntent(intentSincronizacion);
                    editor.PutString("intentSincronizacion", "");
                    editor.Commit();
                    Android.App.Application.Context.StopService(serviceIntent);
                }
                catch (Exception e)
                {
                    //Guarda en base de datos el error
                    int idUsuario = prefs.GetInt("idUsuario", 0);
                    await DB.ErrorService(idUsuario, 1, e.Message);
                }
            }
        }

        public async Task Inicio()
        {
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
            ISharedPreferencesEditor editor = prefs.Edit();

            //TOKEN INICIAL
            JObject autenticar = await this.Autenticar();
            if (autenticar["ESTADO"].ToString() == "FALSE")
            {
                return;
            }

            //AUTENTICACION DEL USUARIO Y REGISTRO DE LA FECHA INICIO DE LA SINCRONIZACION
            JObject validacionUsuario = await this.ValidacionUsuario(autenticar["RESULTADO"].ToString());
            if (validacionUsuario["ESTADO"].ToString() == "FALSE")
            {
                return;
            }

            //SE GUARDA EL TOKEN
            token = validacionUsuario["RESULTADO"]["TOKEN"].ToString();

            //SINCRONIZACION DE ERRORES
            tabla = new ObservableCollection<DataTable>();
            await DB.SincronizacionErrores(tabla);
            if (tabla[0].Rows.Count > 0)
            {
                JObject errores = await this.SincronizacionErrores(tabla[0], token);
                if (errores["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.FinSincronizacionErrores();
            }

            //SINCRONIZACION DE DEPARTAMENTOS
            if (bool.Parse(validacionUsuario["RESULTADO"]["DEPARTAMENTOS"].ToString()))
            {
                JObject departamentos = await this.SincronizacionMaestros("departamentos", token);
                if (departamentos["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.SincronizacionDepartamentos((JArray) departamentos["RESULTADO"]);
            }

            //SINCRONIZACION DE MUNICIPIOS
            if (bool.Parse(validacionUsuario["RESULTADO"]["CIUDADES"].ToString()))
            {
                JObject ciudades = await this.SincronizacionMaestros("ciudades", token);
                if (ciudades["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.SincronizacionCiudades((JArray) ciudades["RESULTADO"]);
            }

            //SINCRONIZACION DE SECTORES
            if (bool.Parse(validacionUsuario["RESULTADO"]["SECTORES"].ToString()))
            {
                JObject sectores = await this.SincronizacionMaestros("sectores", token);
                if (sectores["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.SincronizacionSectores((JArray) sectores["RESULTADO"]);
            }

            //SINCRONIZACION DE FINCAS
            if (bool.Parse(validacionUsuario["RESULTADO"]["FINCAS"].ToString()))
            {
                JObject fincas = await this.SincronizacionMaestros("fincas", token);
                if (fincas["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.SincronizacionFincas((JArray) fincas["RESULTADO"]);
            }

            //SINCRONIZACION DE USUARIOS
            if (bool.Parse(validacionUsuario["RESULTADO"]["USUARIOS"].ToString()))
            {
                JObject usuarios = await this.SincronizacionMaestros("usuarios", token);
                if (usuarios["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.SincronizacionUsuarios((JArray) usuarios["RESULTADO"]);
            }

            //SE GUARDA LA FECHA FIN DE SINCRONIZACION
            JObject fechaFinPrecipitacion = await FechaFinSincronizacion(validacionUsuario["RESULTADO"]["SINCRONIZACION"].ToString(), token);
            if (fechaFinPrecipitacion["ESTADO"].ToString() == "FALSE")
            {
                return;
            }

            //Se sale de la aplicacion si no tiene sesion
            if (validacionUsuario["ESTADO"].ToString() == "FALSE_USER")
            {
                //Se limpia el id usuario
                editor.PutInt("idUsuario", 0);

                editor.PutInt("idFinca", 0);
                editor.PutInt("idDepartamento", 0);
                editor.PutString("nombreDepartamento", "");
                editor.PutString("nombreMunicipio", "");
                editor.PutInt("idCaptura", 0);
                editor.PutInt("idMunicipio", 0);
                editor.PutInt("idVisita", 0);
                editor.PutString("Actividad", "");
                editor.PutString("nombreFinca", "");
                editor.PutInt("idVisita_procesamiento", 0);

                string intentGPS = prefs.GetString("intentGPS", "");
                if (intentGPS != "")
                {
                    serviceIntent = Intent.GetIntent(intentGPS);
                    editor.PutString("intentGPS", "");
                    Android.App.Application.Context.StopService(serviceIntent);
                }
                editor.Commit();

                Intent intent = new Intent(context, typeof(LoginActivity));
                intent.AddFlags(Android.Content.ActivityFlags.ClearTop | Android.Content.ActivityFlags.SingleTop | Android.Content.ActivityFlags.ClearTask | Android.Content.ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);

                return;
            }

            //SINCRONIZACION DE PRECIPITACIONES
            tabla = new ObservableCollection<DataTable>();
            await DB.SincronizacionPrecipitaciones(tabla);
            if (tabla[0].Rows.Count > 0)
            {
                JObject precipitaciones = await this.SincronizacionPrecipitaciones(tabla[0], token);
                if (precipitaciones["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.FinSincronizacionPrecipitaciones();
            }

            //SE SINCRONIZAN LAS VISITAS
            tabla = new ObservableCollection<DataTable>();
            await DB.SincronizacionVisitas(tabla);
            if (tabla[0].Rows.Count > 0)
            {
                JObject infoVisita = new JObject();
                //VISITAS
                infoVisita["VISITA"] = JArray.Parse(JsonConvert.SerializeObject(tabla[0], Formatting.None));

                //RECORRIDO
                tabla = new ObservableCollection<DataTable>();
                await DB.SincronizacionRecorrido(tabla);
                infoVisita["RECORRIDO"] = JArray.Parse(JsonConvert.SerializeObject(tabla[0], Formatting.None));

                //PUNTO CAPTURA
                tabla = new ObservableCollection<DataTable>();
                await DB.SincronizacionPunto(tabla);
                infoVisita["PUNTO"] = JArray.Parse(JsonConvert.SerializeObject(tabla[0], Formatting.None));

                //PLANTA PUNTO CAPTURA
                tabla = new ObservableCollection<DataTable>();
                await DB.SincronizacionPlanta(tabla);
                infoVisita["PLANTA"] = JArray.Parse(JsonConvert.SerializeObject(tabla[0], Formatting.None));

                //INDICADORES PUNTO CAPTURA
                tabla = new ObservableCollection<DataTable>();
                await DB.SincronizacionIndicador(tabla);
                infoVisita["INDICADOR"] = JArray.Parse(JsonConvert.SerializeObject(tabla[0], Formatting.None));

                //FOTOS INDICADOR
                tabla = new ObservableCollection<DataTable>();
                await DB.SincronizacionFoto(tabla);
                infoVisita["FOTO"] = JArray.Parse(JsonConvert.SerializeObject(tabla[0], Formatting.None));

                JObject visitas = await this.SincronizacionVisitas(infoVisita, token);
                if (visitas["ESTADO"].ToString() == "FALSE")
                {
                    return;
                }
                await DB.FinSincronizacionVisitas();
            }

            //SINCRONIZACION DE FOTOS
            int continuar = 1;
            do
            {
                tabla = new ObservableCollection<DataTable>();
                await DB.SincronizacionFotos(tabla);
                if (tabla[0].Rows.Count > 0)
                {
                    JObject fotos = await this.SincronizacionFotos(int.Parse(tabla[0].Rows[0]["id"].ToString()), tabla[0].Rows[0]["fullPath"].ToString(), tabla[0].Rows[0]["name"].ToString(), token);
                    if (fotos["ESTADO"].ToString() == "FALSE")
                    {
                        return;
                    }
                    await DB.FinSincronizacionFotos(int.Parse(tabla[0].Rows[0]["id"].ToString()));
                }
                else
                {
                    continuar = 0;
                }
            } while (continuar == 1);

            //SE GUARDA LA FECHA FIN DE SINCRONIZACION
            fechaFinPrecipitacion = new JObject();
            fechaFinPrecipitacion = await FechaFinSincronizacion(validacionUsuario["RESULTADO"]["SINCRONIZACION"].ToString(), token);
            if (fechaFinPrecipitacion["ESTADO"].ToString() == "FALSE")
            {
                return;
            }
        }

        //TOKEN INICIAL
        private async Task<JObject> Autenticar()
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/Autenticar");
                request.Timeout = 2000000;
                request.CookieContainer = cc;
                request.ReadWriteTimeout = 2000000;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                //Write params in POST
                var posData = "consumerKey=69AA045B-C986-404E-8A7E-80AF212FBAF7";
                posData += "&consumerSecret=BC0F0487-C4E2-448A-959C-59A48AFEB912";
                posData += "&version=210826";
                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //AUTENTICACION DEL USUARIO
        private async Task<JObject> ValidacionUsuario(string token)
        {
            try
            {
                //Se lee el id del usuario
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(context);
                int idUsuario = prefs.GetInt("idUsuario", 0);

                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/ValidacionUsuario");
                request.Timeout = 2000000;
                request.ReadWriteTimeout = 2000000;
                request.CookieContainer = cc;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                parametrosUsuario = new ObservableCollection<string>();
                await DB.InfoUsuario(idUsuario, parametrosUsuario);

                //Write params in POST
                string posData = "";
                posData = "user=" + parametrosUsuario[0];
                posData += "&pass=" + Encrypter.MD5Text(parametrosUsuario[1]);
                posData += "&departamentos=" + parametrosUsuario[2];
                posData += "&ciudades=" + parametrosUsuario[3];
                posData += "&sectores=" + parametrosUsuario[4];
                posData += "&fincas=" + parametrosUsuario[5];
                posData += "&usuarios=" + parametrosUsuario[6];
                posData += "&token=" + token;

                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //SINCRONIZACION DE MAESTROS
        private async Task<JObject> SincronizacionMaestros(string tabla, string token)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/SincronizacionMaestros");
                request.Timeout = 2000000;
                request.ReadWriteTimeout = 2000000;
                request.CookieContainer = cc;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                //Write params in POST
                string posData = "";
                posData = "tabla=" + tabla;
                posData += "&token=" + token;

                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //SINCRONIZACION DE PRECIPITACIONES
        private async Task<JObject> SincronizacionPrecipitaciones(DataTable tabla, string token)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/SincronizacionPrecipitaciones");
                request.Timeout = 2000000;
                request.ReadWriteTimeout = 2000000;
                request.CookieContainer = cc;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                //Write params in POST
                string posData = "";
                posData = "tabla=" + JArray.Parse(JsonConvert.SerializeObject(tabla, Formatting.None)).ToString();
                posData += "&token=" + token;

                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //SINCRONIZACION DE VISITAS
        private async Task<JObject> SincronizacionVisitas(JObject json, string token)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/SincronizacionVisitas");
                request.Timeout = 2000000;
                request.ReadWriteTimeout = 2000000;
                request.CookieContainer = cc;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                //Write params in POST
                string posData = "";
                posData = "json=" + json.ToString();
                posData += "&token=" + token;

                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //SINCRONIZACION DE FOTOS
        public async Task<JObject> SincronizacionFotos(int id, String fullPath, string name, string token)
        {
            try
            {
                //Para la validación de la sesión del usuario del lado server
                HttpClientHandler clientHandler = new HttpClientHandler
                {
                    CookieContainer = cc,
                    UseCookies = true
                };

                //Se deshabilita la verificación de dominio, esto debido a que el cliente es app y no es web
                clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => {
                    return true;
                };

                HttpClient httpClient = new HttpClient(clientHandler);
                MultipartFormDataContent form = new MultipartFormDataContent();

                //Variables POST
                form.Add(new StringContent(token), "token");
                form.Add(new StringContent("Fotos"), "carpeta");
                form.Add(new StringContent(name.Substring(0, name.IndexOf("."))), "nombre");
                form.Add(new StringContent(name.Substring(name.IndexOf(".") + 1)), "ext");

                //SI LA FOTOS NO ESTA ALMACENADA SE DA POR SUBIDA
                try
                {
                    var upfilebytes = File.ReadAllBytes(fullPath);

                    HttpContent httpFile = new ByteArrayContent(upfilebytes);
                    form.Add(httpFile, "file", name);
                    HttpResponseMessage response = await httpClient.PostAsync(conexionUrl + "/SincronizacionFotos", form);

                    httpClient.Dispose();
                    string responseFromServer = response.Content.ReadAsStringAsync().Result;

                    return JObject.Parse(responseFromServer);
                }
                catch
                {
                    JObject response = new JObject();
                    response["ESTADO"] = "TRUE";
                    return JObject.Parse(response.ToString());
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //SINCRONIZACION DE FOTOS
        public async Task<JObject> FechaFinSincronizacion(string id, string token)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/FechaFinSincronizacion");
                request.Timeout = 2000000;
                request.ReadWriteTimeout = 2000000;
                request.CookieContainer = cc;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                //Write params in POST
                string posData = "";
                posData = "id=" + id;
                posData += "&token=" + token;

                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        //SINCRONIZACION DE ERRORES
        private async Task<JObject> SincronizacionErrores(DataTable tabla, string token)
        {
            try
            {
                ServicePointManager.ServerCertificateValidationCallback += (o, cert, chain, errors) => true;

                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(conexionUrl + "/SincronizacionErrores");
                request.Timeout = 2000000;
                request.ReadWriteTimeout = 2000000;
                request.CookieContainer = cc;
                request.Method = "POST";
                request.ContentType = "application/x-www-form-urlencoded";
                Stream dataStream = request.GetRequestStream();

                //Write params in POST
                string posData = "";
                posData = "tabla=" + JArray.Parse(JsonConvert.SerializeObject(tabla, Formatting.None)).ToString();
                posData += "&token=" + token;

                byte[] byteArray = Encoding.UTF8.GetBytes(posData);
                dataStream.Write(byteArray, 0, byteArray.Length);

                dataStream.Close();

                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                dataStream = response.GetResponseStream();
                StreamReader reader = new StreamReader(dataStream);

                string responseFromServer = "";
                responseFromServer = reader.ReadToEnd();
                reader.Close();
                dataStream.Close();
                response.Close();

                return JObject.Parse(responseFromServer);
            }
            catch (Exception e)
            {
                return null;
            }
        }
    }
}