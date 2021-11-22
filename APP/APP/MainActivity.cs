using Android;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Gms.Common;
using Android.OS;
using Android.Preferences;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Widget;
using APP.Activities;
using APP.Helpers;
using AppDemo.LocalLogic.Componentes;
using System;

namespace APP
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = false, Icon = "@mipmap/IconoApp", ScreenOrientation = ScreenOrientation.Portrait)]
    public class MainActivity : AppCompatActivity
    {
        Button sincronizar;
        Button consultarVisitas;
        Button nuevoRecorrido;
        Button precipitaciones;
        Button consultarBioseguridad;
        Button bioseguridad;
        ImageView salir;
        Intent sincronizacion;
        Intent serviceIntent;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            Xamarin.Forms.Forms.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            sincronizar = (Button)FindViewById(Resource.Id.sincronizar);
            sincronizar.Click += Sincronizar_Click;
            consultarVisitas = (Button)FindViewById(Resource.Id.consultarVisitas);
            consultarVisitas.Click += ConsultarVisitas_Click;
            nuevoRecorrido = (Button)FindViewById(Resource.Id.nuevoRecorrido);
            nuevoRecorrido.Click += NuevoRecorrido_Click;
            precipitaciones = (Button)FindViewById(Resource.Id.precipitaciones);
            precipitaciones.Click += Precipitaciones_Click;
            consultarBioseguridad = (Button)FindViewById(Resource.Id.consultarBioseguridad);
            consultarBioseguridad.Click += ConsultarBioseguridad_Click;
            bioseguridad = (Button)FindViewById(Resource.Id.bioseguridad);
            bioseguridad.Click += Bioseguridad_Click;
            salir = (ImageView)FindViewById(Resource.Id.salir);
            salir.Click += Salir_Click;

            PermisosGPS();

            //Se verifica si el usuario cerro la app en medio de un recorrido
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            string Actividad = prefs.GetString("Actividad", "");
            int idVisita = prefs.GetInt("idVisita", 0);
            if (Actividad != "" && idVisita != 0)
            {
                Intent intent = new Intent(this, typeof(NuevoRecorridoActivity));
                intent.PutExtra("opcion", "crear");
                StartActivity(intent);
            }
        }

        private bool isMyServiceRunning(System.Type cls)
        {
            ActivityManager manager = (ActivityManager)GetSystemService(Context.ActivityService);

            foreach (var service in manager.GetRunningServices(int.MaxValue))
            {
                if (service.Service.ClassName.Equals(Java.Lang.Class.FromType(cls).CanonicalName))
                {
                    return true;
                }
            }
            return false;
        }

        protected override void OnResume()
        {
            base.OnResume(); // Always call the superclass first.

            //Se muestra una alerta de permisos permanentes de GPS apagado
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();
            int gps = prefs.GetInt("GPS", 0);
            if (gps == 1)
            {
                Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
                terminar.SetMessage("Es necesario habilitar el \"Permiso de ubicación\" con la opción \"Permitir todo el tiempo\", desde los permisos de la App.");
                terminar.SetTitle("Permisos");

                terminar.SetPositiveButton("Ok", (alert, args) =>
                {

                });

                terminar.Show();
                //Se apaga bandera de GPS
                editor.PutInt("GPS", 0);
                editor.Commit();
            }

            string intentFilter = prefs.GetString("intentFilter", "");

            if (!isMyServiceRunning(typeof(FilterService)) && intentFilter != "")
            {
                /*Servicio de Filtrado*/
                serviceIntent = new Intent(this, typeof(FilterService));

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    StartForegroundService(serviceIntent);
                }
                else
                {
                    StartService(serviceIntent);
                }
            }
        }

        private void PermisosGPS()
        {
            //Verificacion de permisos de locacion
            if (this.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                //Validación de la disponibilidad del servicio de localización
                if (IsGooglePlayServicesInstalled())
                {

                }
                else
                {
                    ToastFragment.ShowMakeText(this, "Location services no disponible");
                }
            }
            else
            {
                //Solicitud de permisos de acceso al GPS, en caso de que el manifest no tenga efecto, pasa con aplicaciones con control de usuarios.
                this.RequestPermissions(new String[] { Manifest.Permission.AccessCoarseLocation, Manifest.Permission.AccessFineLocation, 
                    Manifest.Permission.Camera, Manifest.Permission.ReadExternalStorage, Manifest.Permission.WriteExternalStorage }, 0);
            }
        }

        private void Sincronizar_Click(object sender, System.EventArgs e)
        {
            //Se guarda infromacion en intent de sincronizacion para evitar enviar varios services seguidos
            ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
            ISharedPreferencesEditor editor = prefs.Edit();

            string intentSincronizacion = prefs.GetString("intentSincronizacion", "");

            if ( !isMyServiceRunning(typeof(SincronizacionService)) )
            {
                intentSincronizacion = "";
            }


            if (intentSincronizacion == "")
            {
                editor.PutString("intentSincronizacion", "inicio");
                editor.Commit();

                //LLAMA AL SERVICIO DE SINCRONIZACION
                sincronizacion = new Intent(this, typeof(SincronizacionService));

                if (Android.OS.Build.VERSION.SdkInt >= Android.OS.BuildVersionCodes.O)
                {
                    StartForegroundService(sincronizacion);
                }
                else
                {
                    StartService(sincronizacion);
                }

                //Se guarda el ultimo intent de sincronizacion
                string stringIntnt = sincronizacion.ToURI();
                editor.PutString("intentSincronizacion", stringIntnt);
                editor.Commit();
            }
        }

        private void ConsultarVisitas_Click(object sender, System.EventArgs e)
        {
            //Verificacion de permisos de locacion
            if (this.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                Intent intent = new Intent(this, typeof(NuevoRecorridoActivity));
                intent.PutExtra("opcion", "mostrar");
                StartActivity(intent);
            }
            else
            {
                ToastFragment.ShowMakeText(this, "No puede acceder a esta actividad debido a que no ha proporcionado permisos de ubicación, cámara y acceso a archivos, por favor, proporcione los permisos manualmente");
            }
        }

        private void NuevoRecorrido_Click(object sender, System.EventArgs e)
        {
            //Verificacion de permisos de locacion
            if (this.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                Intent intent = new Intent(this, typeof(NuevoRecorridoActivity));
                intent.PutExtra("opcion", "crear");
                StartActivity(intent);
            }
            else
            {
                ToastFragment.ShowMakeText(this, "No puede acceder a esta actividad debido a que no ha proporcionado permisos de ubicación, cámara y acceso a archivos, por favor, proporcione los permisos manualmente");
            }
        }

        private void Precipitaciones_Click(object sender, EventArgs e)
        {
            Intent intent = new Intent(this, typeof(NuevoRecorridoActivity));
            intent.PutExtra("opcion", "precipitaciones");
            StartActivity(intent);
        }

        private void ConsultarBioseguridad_Click(object sender, EventArgs e)
        {
            //Verificacion de permisos de locacion
            if (this.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                Intent intent = new Intent(this, typeof(NuevoRecorridoActivity));
                intent.PutExtra("opcion", "consultarBioseguridad");
                StartActivity(intent);
            }
            else
            {
                ToastFragment.ShowMakeText(this, "No puede acceder a esta actividad debido a que no ha proporcionado permisos de ubicación, cámara y acceso a archivos, por favor, proporcione los permisos manualmente");
            }
        }

        private void Bioseguridad_Click(object sender, EventArgs e)
        {
            //Verificacion de permisos de locacion
            if (this.CheckSelfPermission(Manifest.Permission.AccessFineLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.AccessCoarseLocation) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.Camera) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.ReadExternalStorage) == Permission.Granted &&
                this.CheckSelfPermission(Manifest.Permission.WriteExternalStorage) == Permission.Granted)
            {
                Intent intent = new Intent(this, typeof(NuevoRecorridoActivity));
                intent.PutExtra("opcion", "bioseguridad");
                StartActivity(intent);
            }
            else
            {
                ToastFragment.ShowMakeText(this, "No puede acceder a esta actividad debido a que no ha proporcionado permisos de ubicación, cámara y acceso a archivos, por favor, proporcione los permisos manualmente");
            }
        }

        bool IsGooglePlayServicesInstalled()
        {
            var queryResult = GoogleApiAvailability.Instance.IsGooglePlayServicesAvailable(this);
            if (queryResult == ConnectionResult.Success)
            {
                return true;
            }

            if (GoogleApiAvailability.Instance.IsUserResolvableError(queryResult))
            {
                var errorString = GoogleApiAvailability.Instance.GetErrorString(queryResult);
            }

            return false;
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        private void Salir_Click(object sender, EventArgs e)
        {
            Android.Support.V7.App.AlertDialog.Builder terminar = new Android.Support.V7.App.AlertDialog.Builder(this);
            terminar.SetMessage("¿Está seguro que desea cerrar sesión?");
            terminar.SetTitle("Cerrar Sesión");

            terminar.SetPositiveButton("Si", (alert, args) =>
            {
                //Se limpia la sesion
                ISharedPreferences prefs = PreferenceManager.GetDefaultSharedPreferences(this);
                ISharedPreferencesEditor editor = prefs.Edit();
                editor.PutInt("idUsuario", 0);
                editor.Commit();

                //Se invoka el login
                Intent intent = new Intent(this, typeof(LoginActivity));
                StartActivity(intent);

                Finish();
            });

            terminar.SetNegativeButton("No", (alert, args) =>
            {

            });

            terminar.Show();
        }
    }
}